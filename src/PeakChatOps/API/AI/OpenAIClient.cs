using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace PeakChatOps.API.AI
{
    /// <summary>
    /// OpenAI API 通用低级封装，支持自定义 endpoint、apiKey、请求体和响应体。
    /// </summary>

    public class OpenAIClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // 新增：AI生成相关参数
        public int MaxTokens { get; set; } = 1024; // 默认最大token
        public double Temperature { get; set; } = 0.7; // 默认采样温度
        public double TopP { get; set; } = 1.0; // 默认top_p
        public int N { get; set; } = 1; // 默认生成数量

        /// <summary>
        /// 构造函数，支持传入AI生成参数
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        /// <param name="baseUrl">API基础地址</param>
        /// <param name="maxTokens">最大token数</param>
        /// <param name="temperature">采样温度</param>
        /// <param name="topP">top_p</param>
        /// <param name="n">生成数量</param>
        public OpenAIClient(
            string apiKey,
            string baseUrl = "https://api.openai.com/v1/",
            int maxTokens = 1024,
            double temperature = 0.7,
            double topP = 1.0,
            int n = 1)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl.TrimEnd('/') + "/";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            MaxTokens = maxTokens;
            Temperature = temperature;
            TopP = topP;
            N = n;
        }

        /// <summary>
        /// 发送通用 POST 请求到 OpenAI API（异步，返回 UniTask）。
        /// </summary>
        /// <param name="endpoint">如 "chat/completions"</param>
        /// <param name="jsonBody">请求体 JSON 字符串</param>
        /// <returns>响应 JSON 字符串</returns>
        public async UniTask<string> PostAsync(string endpoint, string jsonBody)
        {
            var url = _baseUrl + endpoint.TrimStart('/');
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content).AsUniTask();
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().AsUniTask();
        }

        /// <summary>
        /// 辅助：生成带max_tokens等参数的chat/completions请求体（JSON字符串）
        /// </summary>
        /// <param name="model">模型名</param>
        /// <param name="messages">消息数组（OpenAI格式）</param>
        /// <returns>请求体JSON</returns>
        public string BuildChatCompletionsBody(string model, object messages)
        {
            // 兼容性更好：用 Newtonsoft.Json 序列化
            string messagesJson = JsonConvert.SerializeObject(messages);
            return $"{{\n" +
                   $"  \"model\": \"{model}\",\n" +
                   $"  \"messages\": {messagesJson},\n" +
                   $"  \"max_tokens\": {MaxTokens},\n" +
                   $"  \"temperature\": {Temperature},\n" +
                   $"  \"top_p\": {TopP},\n" +
                   $"  \"n\": {N}\n" +
                   $"}}";
        }

        /// <summary>
        /// 发送通用 GET 请求到 OpenAI API（异步，返回 UniTask）。
        /// </summary>
        /// <param name="endpoint">如 "models"</param>
        /// <returns>响应 JSON 字符串</returns>
        public async UniTask<string> GetAsync(string endpoint)
        {
            var url = _baseUrl + endpoint.TrimStart('/');
            var response = await _httpClient.GetAsync(url).AsUniTask();
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().AsUniTask();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
