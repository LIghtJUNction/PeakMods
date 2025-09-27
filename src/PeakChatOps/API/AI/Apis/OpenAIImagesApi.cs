using System.Net.Http;
using Newtonsoft.Json;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Images API 封装，仅实现 List Images 示例。
    /// </summary>
    public class OpenAIImagesApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIImagesApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for images.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Images;
    }
}
