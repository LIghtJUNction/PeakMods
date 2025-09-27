using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Containers API 封装，仅实现 List Containers 示例。
    /// </summary>
    public class OpenAIContainersApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIContainersApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for containers.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Containers;
    }
}
