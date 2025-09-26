using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Files API 封装，仅实现 List Files 示例。
    /// </summary>
    public class OpenAIFilesApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAIFilesApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Files 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListFilesEndpoint() => "files";
    }
}
