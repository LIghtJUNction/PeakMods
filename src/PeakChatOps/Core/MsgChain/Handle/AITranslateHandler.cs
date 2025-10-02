using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.Core.MsgChain.Handle;
public static class AITranslateHandler
{
    /// <summary>
    /// If AI auto-translate is enabled, publish the message to AI translate chain and return true.
    /// Otherwise return false.
    /// </summary>
    public static bool TryHandleAutoTranslate(ChatMessageEvent evt)
    {
        try
        {
            if (evt == null || string.IsNullOrWhiteSpace(evt.Message)) return false;
            if (!PeakChatOpsPlugin.config.AiAutoTranslate.Value) return false;

            var aiEvt = new AIChatMessageEvent(
                sender: evt.Sender,
                message: evt.Message,
                userId: evt.UserId,
                role: AIChatRole.user
            );
            EventBusRegistry.AIChatMessageBus.Publish("ai://translate", aiEvt).Forget();
            return true;
        }
        catch (Exception ex)
        {
            DevLog.UI($"[AITranslateHandler] error: {ex}");
            return false;
        }
    }
}

