#if DEBUG
using System.Linq;
using System.Collections.Generic;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using Photon.Pun;

#nullable enable
namespace PeakChatOps.Commands;


[PCOCommand("dev", "开发者命令", "用法: /dev mock player <消息>\n模拟其他玩家发送消息。")]

public class DevCommand
{
    // 注册
    public DevCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://dev", Handle);
        DevLog.UI("[Cmd] DevCommand subscribed to cmd://dev");
    }
    
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            DevLog.File("[DevCommand] Handle started");
            
            var args = evt.Args;
            DevLog.File($"[DevCommand] args.Length={args.Length}");
            
            if (args.Length == 0)
            {
                await PublishResult(evt, args, false, "用法: /dev mock player <消息>");
                return;
            }

            var action = args[0].ToLowerInvariant();
            DevLog.File($"[DevCommand] action={action}");
            
            if (action != "mock")
            {
                await PublishResult(evt, args, false, $"未知子命令: {args[0]}");
                return;
            }

            if (args.Length < 3)
            {
                await PublishResult(evt, args, false, "用法: /dev mock player <消息>");
                return;
            }

            var target = args[1].ToLowerInvariant();
            DevLog.File($"[DevCommand] target={target}");
            
            if (target != "player")
            {
                await PublishResult(evt, args, false, $"暂不支持的 mock 类型: {args[1]}");
                return;
            }

            DevLog.File($"[DevCommand] Getting player info...");
            var nickname = PhotonNetwork.LocalPlayer?.NickName ?? "MockPlayer";
            var message = string.Join(" ", args.Skip(2));
            var userId = PhotonNetwork.LocalPlayer?.UserId ?? "DEV-MOCK";
            DevLog.File($"[DevCommand] nickname={nickname}, message={message}, userId={userId}");

            // 不需要切换线程，我们已经在主线程上了（由 Unity/Photon 回调触发）
            DevLog.File($"[DevCommand] Creating ChatMessageEvent...");
            var mockChatEvent = new ChatMessageEvent(
                nickname,
                message,
                userId,
                false,
                new Dictionary<string, object> { { "mock", true } }
            );

            DevLog.File($"[DevCommand] Publishing mock message: sender={nickname}, msg={message}");
            
            if (EventBusRegistry.ChatMessageBus == null)
            {
                DevLog.File($"[DevCommand] ERROR: ChatMessageBus is null!");
                await PublishResult(evt, args, false, "ChatMessageBus 未初始化");
                return;
            }
            
            await EventBusRegistry.ChatMessageBus.Publish("sander://other", mockChatEvent);
            DevLog.File($"[DevCommand] Mock message published successfully");

            await PublishResult(evt, args, true, $"已模拟玩家 {nickname} 发送: {message}");
        }
        catch (System.Exception ex)
        {
            DevLog.File($"[DevCommand] Exception in Handle: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await PublishResult(evt, evt.Args, false, $"错误: {ex.Message}");
        }
    }

    private static async UniTask PublishResult(CmdMessageEvent evt, string[] args, bool success, string message)
    {
        var result = new CmdExecResultEvent(
            evt.Command,
            args,
            evt.UserId,
            success ? message : null,
            success ? null : message,
            success);

        await EventBusRegistry.CmdExecResultBus.Publish("cmd://", result);
    }

}

#endif