#nullable enable
using System;
using System.Collections.Generic;

namespace PeakChatOps.API.AI.Responses
{
    // 1. 列表型响应结构
    [Serializable]
    public class OpenAIListResponse<T>
    {
        public string? @object;
        public List<T> data = new List<T>();
    }

    // 2. 模型对象结构
    [Serializable]
    public class OpenAIModel
    {
        public string? id;
        public string? @object;
        public long created;
        public string? owned_by;
    }

    // 3. 消息对象结构
    [Serializable]
    public class OpenAIMessage
    {
        public string? id;
        public string? @object;
        public long created_at;
        public string? assistant_id;
        public string? thread_id;
        public string? role;
        public List<OpenAIMessageContent> content = new List<OpenAIMessageContent>();
        public List<object> attachments = new List<object>();
        public Dictionary<string, object> metadata = new Dictionary<string, object>();
    }

    [Serializable]
    public class OpenAIMessageContent
    {
        public string? type;
        public OpenAIMessageText? text;
    }

    [Serializable]
    public class OpenAIMessageText
    {
        public string? value;
        public List<object> annotations = new List<object>();
    }

    // 4. 错误响应结构
    [Serializable]
    public class OpenAIErrorResponse
    {
        public OpenAIError? error;
    }

    [Serializable]
    public class OpenAIError
    {
        public string? message;
        public string? type;
        public string? param;
        public string? code;
    }

    // 5. 批量/Batch 响应结构
    [Serializable]
    public class OpenAIBatchResponse
    {
        public string? id;
        public string? custom_id;
        public OpenAIBatchResponseDetail? response;
        public OpenAIError? error;
    }

    [Serializable]
    public class OpenAIBatchResponseDetail
    {
        public int status_code;
        public string? request_id;
        public object? body;
    }
}
