using System;

using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Project API Keys API 封装：列出指定项目的所有 API Key。
    /// </summary>
    public class OpenAIProjectApiKeysApi
    {
        private readonly OpenAIClient _client;

        public OpenAIProjectApiKeysApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取指定项目的所有 API Key。
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="after">可选，分页游标</param>
        /// <param name="limit">可选，返回数量限制</param>
        /// <returns>响应 JSON 字符串</returns>
        public UniTask<string> GetListProjectApiKeysEndpoint(string projectId, string after = null, int? limit = null)
        {
            var endpoint = OpenAIEndpoint.ProjectApiKeys.ToPath().Replace("{projectId}", Uri.EscapeDataString(projectId));
            string query = "";
            if (!string.IsNullOrEmpty(after))
            {
                query += $"after={Uri.EscapeDataString(after)}";
            }
            if (limit.HasValue)
            {
                if (query.Length > 0) query += "&";
                query += $"limit={limit.Value}";
            }
            if (query.Length > 0)
            {
                endpoint += $"?{query}";
            }
            return UniTask.FromResult(endpoint);
        }
    }
}
