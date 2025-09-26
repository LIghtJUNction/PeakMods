using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Completions API 封装，仅实现 List Completions 示例。
    /// </summary>
    public class OpenAICompletionsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAICompletionsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Completions 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListCompletionsEndpoint() => "completions";
    }
}
