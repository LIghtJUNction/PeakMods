using Xunit;
using PeakChatOps.API.AI;
using PeakChatOps.API.AI.Apis;
using System.Threading.Tasks;

namespace PeakChatOps.API.Tests
{
    public class OpenAIModerationsApiTests
    {
        private readonly OpenAIClient _client;
        private readonly OpenAIModerationsApi _api;

        public OpenAIModerationsApiTests()
        {
            // 请替换为你的真实API Key，或用Mock替换
            _client = new OpenAIClient("YOUR_API_KEY");
            _api = new OpenAIModerationsApi(_client);
        }

        [Fact]
        public async Task ListModerationsAsync_ReturnsJson()
        {
            var result = await _api.ListModerationsAsync();
            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Contains("object", result); // 简单断言返回内容
        }
    }
}
