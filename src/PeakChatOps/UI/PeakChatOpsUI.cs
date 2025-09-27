// ...existing code...
using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PeakChatOps.UI;

public partial class PeakChatOpsUI : MonoBehaviour
{
    public static PeakChatOpsUI instance;

    // 内存消息模型（流式缓冲存放于 model）
    public class MessageModel
    {
        public string Id = Guid.NewGuid().ToString();
        public StringBuilder Buffer = new StringBuilder();
        public bool IsStreaming = true;
        public bool IsPinned = false;
        // TODO: add metadata (sender, color, timestamps) and UI handle reference if needed
    }

    // 存放所有消息/流（包括已完成的）
    private readonly Dictionary<string, MessageModel> _messages = new Dictionary<string, MessageModel>();

    // 同步添加一条富文本消息到聊天窗口（快捷方式：创建 model 并立刻 Finish）
    public void AddMessage(string message)
    {
        var id = StartStream();
        AppendToStream(id, message);
        FinishStream(id);
    }

    // 异步添加一条富文本消息到聊天窗口（保证在主线程）
    public async UniTask AddMessageAsync(string message)
    {
        await UniTask.SwitchToMainThread();
        AddMessage(message);
    }

    // Start a streaming message and 返回 messageId（用于后续 Append/Finish）
    public string StartStream(string initialText = "")
    {
        var model = new MessageModel();
        if (!string.IsNullOrEmpty(initialText))
            model.Buffer.Append(initialText);
        _messages[model.Id] = model;
        return model.Id;
    }

    // 向指定 message 追加内容（若 message 不存在则忽略）
    public void AppendToStream(string messageId, string chunk)
    {
        if (string.IsNullOrEmpty(messageId) || chunk == null) return;
        if (!_messages.TryGetValue(messageId, out var model)) return;
        model.Buffer.Append(chunk);
        // TODO: 标记需要刷新 UI（比如触发可见单元刷新的事件）
    }

    // 标记 message 完成（Finish -> 立即 flush 并触发 OnMessageFinished）
    public void FinishStream(string messageId)
    {
        if (string.IsNullOrEmpty(messageId)) return;
        if (!_messages.TryGetValue(messageId, out var model)) return;
        model.IsStreaming = false;
        // TODO: Flush to UI immediately & trigger OnMessageFinished
    }

    // 直接 Finish 并回收（有时需要强制完成并回收资源）
    public void FinishAndReleaseStream(string messageId)
    {
        FinishStream(messageId);
        // TODO: 在确保布局/渲染完成后回收 UI 资源并从字典移除
        _messages.Remove(messageId);
    }
    
    // 移除指定消息（若消息不可见或已被回收，仍应从 model 中移除）
    public bool RemoveMessage(string messageId)
    {
        if (string.IsNullOrEmpty(messageId)) return false;
        return _messages.Remove(messageId);
    }

    // 可选查询接口
    public bool TryGetMessageModel(string messageId, out MessageModel model) => _messages.TryGetValue(messageId, out model);
}
// ...existing code...