using System;
using System.Collections.Concurrent;

using System.Threading;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.API
{
    /// <summary>
    /// 轻量的基于 UniTask 的事件总线（发布/订阅 + 异步消费循环）。
    /// 设计目标：易用、线程安全的发布，且消费在主线程运行以安全调用 Unity API。
    /// 
    /// 使用示例：
    /// var bus = new UniEventBus<YourEvent>();
    /// bus.Publish(evt); // 可以在任意线程
    /// await bus.RunAsync(ct); // 在主线程中运行消费循环（例如在 Start 中）
    /// 
    /// 或者订阅回调：
    /// bus.Subscribe(e => { /* handle */ return UniTask.CompletedTask; });
    /// </summary>
    
    public class UniEventBus<T>
    {
        // 每个通道一个队列和一个 handler
        private readonly ConcurrentDictionary<string, ConcurrentQueue<T>> _queues = new();
        private readonly ConcurrentDictionary<string, Func<T, UniTask>> _handlers = new();
        // waiters per channel for consumers waiting for next item
        private readonly ConcurrentDictionary<string, UniTaskCompletionSource<T>> _waiters = new();

        /// <summary>
        /// 发布事件到指定通道（线程安全，异步可等待）。
        /// </summary>
        public UniTask Publish(string channel, T ev)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            // If there's a consumer actively waiting for the next item, deliver it directly
            // and avoid enqueuing to prevent the same item being processed twice.
            if (_waiters.TryRemove(channel, out var waiter))
            {
                try { waiter.TrySetResult(ev); } catch { }
                return UniTask.CompletedTask;
            }

            var queue = _queues.GetOrAdd(channel, _ => new ConcurrentQueue<T>());
            queue.Enqueue(ev);

            // Return completed task to avoid introducing extra await/yield state machine
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 注册/覆盖指定通道的 handler。后注册会覆盖前一个。
        /// </summary>
        public void Subscribe(string channel, Func<T, UniTask> handler)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[channel] = handler;
        }

        /// <summary>
        /// 注销指定通道的 handler。
        /// </summary>
        public void Unsubscribe(string channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            _handlers.TryRemove(channel, out _);
        }

        /// <summary>
        /// 等待并返回指定通道的下一条事件（可取消）。
        /// </summary>
        public UniTask<T> WaitForNextAsync(string channel, CancellationToken ct = default)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            var queue = _queues.GetOrAdd(channel, _ => new ConcurrentQueue<T>());
            if (queue.TryDequeue(out var item))
                return UniTask.FromResult(item);

            var tcs = new UniTaskCompletionSource<T>();
            _waiters[channel] = tcs;

            if (ct.CanBeCanceled)
            {
                // register cancellation to fault the task
                ct.Register(() =>
                {
                    if (_waiters.TryRemove(channel, out var w))
                    {
                        try { w.TrySetException(new OperationCanceledException()); } catch { }
                    }
                });
            }

            return tcs.Task;
        }

        /// <summary>
        /// 主动消费指定通道的所有事件（异步循环，仅一对一）。
        /// </summary>
        // RunAsync 提供了一个内置的消费循环，使用 UniTask 在调用线程（通常为主线程）上执行 handler。
        // 调用者可以直接通过 `await bus.RunAsync(channel, ct)` 启动消费循环，替代仓库中的 Runner。
        public async UniTask RunAsync(string channel, CancellationToken ct = default)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            while (!ct.IsCancellationRequested)
            {
                T ev;
                try
                {
                    ev = await WaitForNextAsync(channel, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (TryGetHandler(channel, out var handler) && handler != null)
                {
                    try
                    {
                        await handler(ev).AttachExternalCancellation(ct);
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception ex)
                    {
                        // API layer shouldn't assume DevLog exists; rethrow so caller can observe or catch.
                        try { System.Diagnostics.Debug.WriteLine($"[UniEventBus] handler threw: {ex}"); } catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Try to get the registered handler for a channel.
        /// Runtime code may use this to obtain the handler and await it safely in the runtime assembly.
        /// </summary>
        public bool TryGetHandler(string channel, out Func<T, UniTask> handler)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            return _handlers.TryGetValue(channel, out handler);
        }
    }
}
