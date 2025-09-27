using System.Net.Http;

using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Audio API 封装，仅实现 List Audio 示例。
    /// </summary>
    public class OpenAIAudioApi
    {
        private readonly OpenAIClient _client;

        public OpenAIAudioApi(OpenAIClient client)
        {
            _client = client;
        }

    /// <summary>
    /// OpenAI Audio API endpoint enum. Use <see cref="OpenAIEndpointExtensions.ToPath"/> to get the path string.
    /// </summary>
    public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Audio;
    }
}
