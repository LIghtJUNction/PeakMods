using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Audio API 封装，仅实现 List Audio 示例。
    /// </summary>
    public class OpenAIAudioApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIAudioApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Audio 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListAudioEndpoint() => "audio";
    }
}
