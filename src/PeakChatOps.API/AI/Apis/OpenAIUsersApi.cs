using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Users API 封装，仅实现 List Users 示例。
    /// </summary>
    public class OpenAIUsersApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIUsersApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Users 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListUsersEndpoint(string projectId)
            => $"organization/projects/{projectId}/users";
    }
}
