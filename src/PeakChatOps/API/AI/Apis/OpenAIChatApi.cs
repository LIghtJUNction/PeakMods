using System.Net.Http;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Chat API 封装，支持 /v1/chat/completions。
    /// </summary>
    public class OpenAIChatApi
    {
        private readonly OpenAIClient _client;

        public OpenAIChatApi(OpenAIClient client)
        {
            _client = client;
        }
        

        /// <summary>
        /// Endpoint enum for chat completions.
        /// </summary>
        public static OpenAIEndpoint Endpoint => OpenAIEndpoint.ChatCompletions;


        /// <summary>
        /// 发送 chat/completions 请求（异步），返回原始响应字符串。
        /// </summary>
        public async UniTask<string> CreateChatCompletionAsync(string model, List<Dictionary<string, object>> messages, int maxTokens = 128)
        {
            var body = new
            {
                model = model,
                messages = messages,
                max_tokens = maxTokens
            };
            var json = JsonConvert.SerializeObject(body);
            // PostAsync 已返回 UniTask<string>
            return await _client.PostAsync(Endpoint.ToPath(), json);
        }

    }
}
