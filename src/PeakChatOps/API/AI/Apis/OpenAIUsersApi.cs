using System;
using System.Net.Http;

using Newtonsoft.Json;
using PeakChatOps.API.AI.Apis;
using Cysharp.Threading.Tasks;

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
        /// 获取 List Users 的 endpoint 路径（异步，立即返回）。
        /// </summary>
        public UniTask<string> GetListUsersEndpoint(string projectId)
            => UniTask.FromResult(OpenAIEndpoint.Users.ToPath().Replace("{projectId}", Uri.EscapeDataString(projectId)));
    }
}
