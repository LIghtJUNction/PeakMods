using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Containers API 封装，仅实现 List Containers 示例。
    /// </summary>
    public class OpenAIContainersApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIContainersApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Containers 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListContainersEndpoint() => "containers";
    }
}
