using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Assistants API 封装，仅实现 List Assistants 示例。
    /// </summary>
    public class OpenAIAssistantsApi
    {
        private readonly OpenAIClient _client;

        public OpenAIAssistantsApi(OpenAIClient client)
        {
            _client = client;
        }

    /// <summary>
    /// Endpoint enum for assistants.
    /// </summary>
    public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Assistants;
    }
}
