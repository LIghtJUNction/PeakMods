using System;

using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI
{
    /// <summary>
    /// OpenAI Batches API 封装：列出所有批处理任务。
    /// </summary>
    public class OpenAIBatchesApi
    {
        private readonly OpenAIClient _client;

        public OpenAIBatchesApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Batches 的 endpoint 路径（异步，立即返回）。
        /// </summary>
        /// <param name="limit">可选，返回数量限制</param>
        public UniTask<string> GetListBatchesEndpoint(int? limit = null)
        {
            // For now return base path; callers can append query if needed.
            var endpoint = OpenAIEndpoint.Batches.ToPath();
            if (limit.HasValue)
            {
                endpoint += $"?limit={limit.Value}";
            }
            return UniTask.FromResult(endpoint);
        }
    }
}
