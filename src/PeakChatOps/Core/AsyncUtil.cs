
#nullable enable
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 业务层通用异步包装器，将同步耗时操作安全地放到线程池并捕获异常。
    /// </summary>
    public static class AsyncUtil
    {
        /// <summary>
        /// 在后台线程安全执行同步方法，返回 (结果, 异常)。
        /// </summary>
        public static async UniTask<(T result, Exception? error)> RunInBackground<T>(Func<T> func)
        {
            try
            {
                var result = await Task.Run(func);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (default!, ex);
            }
        }
    }
}
