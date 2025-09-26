using System;
using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI.Requests;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Vector Store API 封装：创建向量存储。
    /// </summary>
    public class OpenAIVectorStoreApi
    {
        private readonly OpenAIClient _client;

        public OpenAIVectorStoreApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 Create Vector Store 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetCreateVectorStoreEndpoint() => "vector_stores";
    }
}
