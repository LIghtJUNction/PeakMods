using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 简单的主线程调度器，支持在任意线程安全地调度到Unity主线程执行。
    /// </summary>
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;
        private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private static bool _initialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (_initialized) return;
            var go = new GameObject("MainThreadDispatcher");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<MainThreadDispatcher>();
            _initialized = true;
        }

        public static void Run(Action action)
        {
            if (!_initialized)
                Init();
            _actions.Enqueue(action);
        }

        void Update()
        {
            while (_actions.TryDequeue(out var action))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    // 异常时丢弃该Action，不做任何重试或重复入队
                    Debug.LogError($"[MainThreadDispatcher] 调用Action异常已丢弃: {ex}");
                }
            }
        }
    }
}
