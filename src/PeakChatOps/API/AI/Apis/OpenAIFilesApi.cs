using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Files API 封装，仅实现 List Files 示例。
    /// </summary>
    public class OpenAIFilesApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIFilesApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for files.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Files;
    }
}
