using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Assistants API 封装，仅实现 List Assistants 示例。
    /// </summary>
    public class OpenAIAssistantsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIAssistantsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Assistants 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListAssistantsEndpoint() => "assistants";
    }
}
