using System;
using System.Collections.Generic;

namespace PeakChatOps.API.AI.Requests
{
    // 这里可继续扩展 Embeddings、FineTune、VectorStore 等请求体结构
    // 示例：
    [Serializable]
    public class OpenAIEmbeddingRequest
    {
        public string? model;
        public List<string> input = new List<string>();
        public string? user;
    }

    [Serializable]
    public class OpenAIFineTuneRequest
    {
        public string? training_file;
        public string? model;
        public int? n_epochs;
        public float? learning_rate_multiplier;
        public int? batch_size;
        public string? validation_file;
        public string? suffix;
        // 其它参数可扩展
    }

    [Serializable]
    public class OpenAIVectorStoreCreateRequest
    {
        public string? name;
        // 可扩展更多参数
    }
}
