using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Service Accounts API 封装，仅实现 List Service Accounts 示例。
    /// </summary>
    public class OpenAIServiceAccountsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIServiceAccountsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Service Accounts 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListServiceAccountsEndpoint(string projectId)
            => $"organization/projects/{projectId}/service_accounts";
    }
}
