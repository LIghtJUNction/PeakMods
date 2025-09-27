using System.Net.Http;
using Newtonsoft.Json;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Runs API 封装，仅实现 List Runs 示例。
    /// </summary>
    public class OpenAIRunsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIRunsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for runs.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Runs;
    }
}
