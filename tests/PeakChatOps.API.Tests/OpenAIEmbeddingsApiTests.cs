using Xunit;
using PeakChatOps.API.AI;
using PeakChatOps.API.AI.Apis;
using System.Threading.Tasks;

namespace PeakChatOps.API.Tests
{
    public class OpenAIEmbeddingsApiTests
    {
        private readonly OpenAIClient _client;
        private readonly OpenAIEmbeddingsApi _api;

        public OpenAIEmbeddingsApiTests()
        {
            // 请替换为你的真实API Key，或用Mock替换
            _client = new OpenAIClient("YOUR_API_KEY");
            _api = new OpenAIEmbeddingsApi(_client);
        }

        [Fact]
        public async Task ListEmbeddingsAsync_ReturnsJson()
        {
            var result = await _api.ListEmbeddingsAsync();
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Contains("object", result); // 简单断言返回内容
        }
    }
}
