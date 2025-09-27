using System;

namespace PeakChatOps.API.AI.Apis
{
    /// <summary>
    /// Centralized enum for OpenAI endpoint identifiers. Use <see cref="ToPath"/> to get the string path.
    /// </summary>
    public enum OpenAIEndpoint
    {
        Assistants,
        Audio,
        Batches,
        ChatCompletions,
        Checkpoints,
        Completions,
        Containers,
        Embeddings,
        Files,
        FineTuningJobs,
        Images,
        Models,
        Moderations,
        ProjectApiKeys,
        Runs,
        ServiceAccounts,
        Threads,
        Users,
        VectorStores
    }

    public static class OpenAIEndpointExtensions
    {
        public static string ToPath(this OpenAIEndpoint e)
        {
            return e switch
            {
                OpenAIEndpoint.Assistants => "assistants",
                OpenAIEndpoint.Audio => "audio",
                OpenAIEndpoint.Batches => "batches",
                OpenAIEndpoint.ChatCompletions => "chat/completions",
                OpenAIEndpoint.Checkpoints => "fine_tuning/checkpoints",
                OpenAIEndpoint.Completions => "completions",
                OpenAIEndpoint.Containers => "containers",
                OpenAIEndpoint.Embeddings => "embeddings",
                OpenAIEndpoint.Files => "files",
                OpenAIEndpoint.FineTuningJobs => "fine_tuning/jobs",
                OpenAIEndpoint.Images => "images",
                OpenAIEndpoint.Models => "models",
                OpenAIEndpoint.Moderations => "moderations",
                OpenAIEndpoint.ProjectApiKeys => "organization/projects/{projectId}/api_keys",
                OpenAIEndpoint.Runs => "runs",
                OpenAIEndpoint.ServiceAccounts => "organization/projects/{projectId}/service_accounts",
                OpenAIEndpoint.Threads => "threads",
                OpenAIEndpoint.Users => "organization/projects/{projectId}/users",
                OpenAIEndpoint.VectorStores => "vector_stores",
                _ => throw new ArgumentOutOfRangeException(nameof(e), e, null)
            };
        }
    }
}
