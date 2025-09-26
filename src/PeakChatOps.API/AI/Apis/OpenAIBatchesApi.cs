using System;
using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI;

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
        /// 获取 List Batches 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        /// <param name="limit">可选，返回数量限制</param>
        public string GetListBatchesEndpoint(int? limit = null)
        {
            string endpoint = "batches";
            if (limit.HasValue)
            {
                endpoint += $"?limit={limit.Value}";
            }
            return endpoint;
        }
    }
}
