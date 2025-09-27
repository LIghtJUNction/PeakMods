using System;
using System.Net.Http;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

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
        /// 获取 List Service Accounts 的 endpoint 路径（异步，立即返回）。
        /// </summary>
        public UniTask<string> GetListServiceAccountsEndpoint(string projectId)
            => UniTask.FromResult(OpenAIEndpoint.ServiceAccounts.ToPath().Replace("{projectId}", Uri.EscapeDataString(projectId)));
    }
}
