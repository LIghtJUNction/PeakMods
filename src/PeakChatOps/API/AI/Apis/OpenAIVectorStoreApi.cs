using System;

using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI.Requests;
using Newtonsoft.Json;
using PeakChatOps.API.AI.Apis;

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
        /// Endpoint enum for vector stores.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.VectorStores;
    }
}
