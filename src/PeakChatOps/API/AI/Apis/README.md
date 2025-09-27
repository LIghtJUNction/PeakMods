# OpenAI Apis 目录说明

本文件夹包含 OpenAI 相关 API 封装类，每个类对应 OpenAI 平台的一个 REST API 资源，便于在 Unity/BepInEx/Mono 环境下直接调用。

## 当前包含的 API 封装

共 5 个 API 封装类：

1. **OpenAIBatchesApi**
   - 批处理任务相关接口（/v1/batches）
2. **OpenAIFineTuningJobsApi**
   - 微调任务相关接口（/v1/fine_tuning/jobs）
3. **OpenAIModelsApi**
   - 模型管理相关接口（/v1/models）
4. **OpenAIProjectApiKeysApi**
   - 项目 API Key 管理接口（/v1/organization/projects/{project_id}/api_keys）
5. **OpenAIVectorStoreApi**
   - 向量存储相关接口（/v1/vector_stores）

---

每个 API 类均采用 UniTask 异步返回，参数和请求体结构已按 OpenAI 最新规范设计，便于类型安全和扩展。

如需扩展其它 OpenAI API，请在本目录下新增对应类，并遵循同样的命名和结构风格。
