using System.Net.Http;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Moderations API 封装，仅实现 List Moderations 示例。
    /// </summary>
    public class OpenAIModerationsApi
    {
        private readonly OpenAIClient _client;

        public OpenAIModerationsApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Endpoint enum for moderations.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Moderations;
    }
}
