using System;
using System.Collections.Generic;

namespace PeakChatOps.API.AI.Requests
{
    /// <summary>
    /// OpenAI 通用请求体结构（以 Batch API、Chat Completions 为例，可扩展）
    /// </summary>
    [Serializable]
    public class OpenAIBatchRequest
    {
        public string? custom_id;
        public string? method; // "POST"
        public string? url;    // 例如 /v1/chat/completions
        public object? body;   // 具体 API 请求体
    }

    [Serializable]
    public class OpenAIChatCompletionRequest
    {
        public string? model;
        public List<OpenAIChatMessage> messages = new List<OpenAIChatMessage>();
        public float? temperature;
        public int? max_tokens;
        public float? top_p;
        public float? presence_penalty;
        public float? frequency_penalty;
        public Dictionary<string, object>? logit_bias;
        public string? user;
        // 其它参数可按需扩展
    }

    [Serializable]
    public class OpenAIChatMessage
    {
        public string? role; // "system" | "user" | "assistant"
        public string? content;
        // function_call、tool_calls等高级参数可扩展
    }

    // 可扩展更多请求体结构，如 Embeddings、Fine-tune、Vector Store 等
}
