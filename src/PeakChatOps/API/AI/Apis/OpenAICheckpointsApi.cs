using System.Net.Http;

using Newtonsoft.Json;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Fine-Tuning Checkpoints API 封装，仅实现 List Checkpoints 示例。
    /// </summary>
    public class OpenAICheckpointsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAICheckpointsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for checkpoints.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Checkpoints;
    }
}
