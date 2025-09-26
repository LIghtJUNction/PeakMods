using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Moderations API 封装，仅实现 List Moderations 示例。
    /// </summary>
    public class OpenAIModerationsApi
    {
        private readonly OpenAIClient _client;

        public OpenAIModerationsApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Moderations 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListModerationsEndpoint() => "moderations";
    }
}
