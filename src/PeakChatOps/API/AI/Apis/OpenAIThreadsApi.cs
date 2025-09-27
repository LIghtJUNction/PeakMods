using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Threads API 封装，仅实现 List Threads 示例。
    /// </summary>
    public class OpenAIThreadsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIThreadsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for threads.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Threads;
    }
}
