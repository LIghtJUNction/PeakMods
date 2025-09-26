using System;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.API.AI
{
    /// <summary>
    /// OpenAI Models API 封装：列出所有可用模型。
    /// </summary>
    public class OpenAIModelsApi
    {
        private readonly OpenAIClient _client;

        public OpenAIModelsApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取所有可用模型列表。
        /// </summary>
        /// <returns>响应 JSON 字符串</returns>
        public string GetListModelsEndpoint()
        {
            // GET https://api.openai.com/v1/models
            return "models";
        }
    }
}
