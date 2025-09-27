using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Embeddings API 封装，仅实现 List Embeddings 示例。
    /// </summary>
    public class OpenAIEmbeddingsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIEmbeddingsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for embeddings.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Embeddings;
    }
}
