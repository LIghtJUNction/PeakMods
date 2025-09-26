using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Embeddings API 封装，仅实现 List Embeddings 示例。
    /// </summary>
    public class OpenAIEmbeddingsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIEmbeddingsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Embeddings 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListEmbeddingsEndpoint() => "embeddings";
    }
}
