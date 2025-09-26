using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// OpenAI Fine-Tuning Checkpoints API 封装，仅实现 List Checkpoints 示例。
    /// </summary>
    public class OpenAICheckpointsApi
    {
        private readonly PeakChatOps.API.AI.OpenAIClient _client;

        public OpenAICheckpointsApi(PeakChatOps.API.AI.OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取 List Fine-Tuning Checkpoints 的 endpoint 路径（同步，无状态机）。
        /// </summary>
        public string GetListCheckpointsEndpoint() => "fine_tuning/checkpoints";
    }
}
