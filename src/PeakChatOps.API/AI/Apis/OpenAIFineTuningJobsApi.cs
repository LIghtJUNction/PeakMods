using System;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.API.AI
{
    /// <summary>
    /// OpenAI Fine-Tuning Jobs API 封装：列出所有微调任务。
    /// </summary>
    public class OpenAIFineTuningJobsApi
    {
        private readonly OpenAIClient _client;

        public OpenAIFineTuningJobsApi(OpenAIClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取所有微调任务列表。
        /// </summary>
        /// <param name="limit">可选，返回数量限制</param>
        /// <param name="metadata">可选，元数据筛选</param>
        /// <returns>响应 JSON 字符串</returns>
        public string GetListFineTuningJobsEndpoint(int? limit = null, string? metadata = null)
        {
            // 构建查询参数
            string endpoint = "fine_tuning/jobs";
            if (limit.HasValue || !string.IsNullOrEmpty(metadata))
            {
                endpoint += "?";
                if (limit.HasValue)
                {
                    endpoint += $"limit={limit.Value}";
                }
                if (!string.IsNullOrEmpty(metadata))
                {
                    if (limit.HasValue) endpoint += "&";
                    endpoint += $"metadata={Uri.EscapeDataString(metadata)}";
                }
            }
            return endpoint;
        }
    }
}
