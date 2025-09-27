using System;

using Cysharp.Threading.Tasks;
using PeakChatOps.API.AI.Apis;

namespace PeakChatOps.API.AI.Apis
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
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.Models;
    }
}
