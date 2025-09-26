using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PeakChatOps.API;

namespace PeakChatOps.Core
{
    // Runtime-side runner which consumes events from the API UniEventBus and awaits handlers
    // This keeps async/await state machines in the runtime assembly (which references Unity).
    public static class UniEventBusRunner
    {
        /// <param name="useBackground">是否在后台线程执行handler（如需后台耗时处理可设为true）</param>
        public static async UniTask RunChannelLoop<T>(UniEventBus<T> bus, string channel, CancellationToken ct = default, bool useBackground = false)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            while (!ct.IsCancellationRequested)
            {
                T ev;
                try
                {
                    ev = await bus.WaitForNextAsync(channel, ct);
                    DevLog.UI($"[DebugUI] Runner dequeued event on channel '{channel}': {ev?.ToString() ?? "<null>"}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (bus.TryGetHandler(channel, out var handler))
                {
                    DevLog.UI($"[DebugUI] Runner invoking handler for channel '{channel}'");
                    try
                    {
                        if (useBackground)
                        {
                            // 用AsyncUtil在后台线程执行handler
                            var (_, error) = await AsyncUtil.RunInBackground(() => {
                                handler(ev).Forget(); // fire and forget
                                return true;
                            });
                            if (error != null)
                                DevLog.UI($"[DebugUI] Runner handler for channel '{channel}' threw an exception: {error}");
                        }
                        else
                        {
                            await handler(ev).AttachExternalCancellation(ct);
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        DevLog.UI($"[DebugUI] Runner handler for channel '{channel}' threw an exception: {ex}");
                    }
                }
                else
                {
                    DevLog.UI($"[DebugUI] Runner found no handler for channel '{channel}'");
                }
            }
        }
    }
}
