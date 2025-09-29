using System;
using PeakChatOps.Commands;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using System.Threading;
using PeakChatOps.UI;

namespace PeakChatOps.Core.MsgChain;

// 流程 - > CentralCmdRouter -> ChatMessageChain(负责启动AIChatMessageChain) -> AIChatMessageChain
public static class CentralCmdRouter
{
    private static CancellationTokenSource _busCts = new();
    public static CancellationTokenSource GetOrCreateBusCts() => _busCts;
    static CentralCmdRouter()
    {
        // 初始化命令链和消息链
        Cmdx.LoadPCmd();
        Cmdx.Prefix = PeakChatOpsPlugin.CmdPrefix.Value;

        // 只订阅命令相关
        DevLog.UI("[DebugUI] CentralCmdRouter static ctor: subscribing to command buses and starting runners");
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://", RouteCommandAsync);
        DevLog.UI("[DebugUI] Subscribed CmdMessageBus -> cmd:// -> RouteCommandAsync");
        EventBusRegistry.CmdExecResultBus.Subscribe("cmd://", HandleCmdExecResultAsync);
        DevLog.UI("[DebugUI] Subscribed CmdExecResultBus -> cmd:// -> HandleCmdExecResultAsync");
        // Command channels: consume incoming commands and also handle cmd execution results
        EventBusRegistry.CmdMessageBus.RunAsync("cmd://", _busCts.Token).Forget();
        DevLog.UI("[DebugUI] Runner started for CmdMessageBus -> cmd://");
        EventBusRegistry.CmdExecResultBus.RunAsync("cmd://", _busCts.Token).Forget();
        DevLog.UI("[DebugUI] Runner started for CmdExecResultBus -> cmd://");
        // 加载消息链
        DevLog.UI("[DebugUI] CentralCmdRouter static ctor: initializing ChatMessageChain");
        ChatMessageChain.EnsureInitialized();
    }

    // Ensure this class is initialized (triggers static constructor). Call from plugin Awake.
    public static void EnsureInitialized() { /* intentionally empty */ }

    // 二次分发并执行命令 // 路由
    private static async UniTask RouteCommandAsync(CmdMessageEvent evt)
    {
        DevLog.UI($"[DebugUI] RouteCommandAsync called. Command={(evt == null ? "<null>" : evt.Command)} ArgsCount={(evt == null ? 0 : evt.Args?.Length ?? 0)} UserId={(evt == null ? "<null>" : evt.UserId)}");
        try
        {
            if (evt == null || string.IsNullOrWhiteSpace(evt.Command))
            {
                DevLog.UI("[DebugUI] HandleCmdAsync -> empty or null command");
                var errEmpty = new CmdExecResultEvent(evt?.Command ?? string.Empty, evt?.Args ?? Array.Empty<string>(), evt?.UserId ?? string.Empty, stdout: null, stderr: "command not found", success: false);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEmpty);
                return;
            }

            // display a simple local UI line for the command (coloring moved to router level)
            try { PeakChatOpsUI.Instance.AddMessage($"<color=#59A6FF>> {evt.Command}</color> {(evt.Args != null ? string.Join(" ", evt.Args) : string.Empty)}"); } catch { }

            var raw = evt.Command.Trim();
            var candidates = new System.Collections.Generic.List<string>
            {
                $"cmd://{raw}",
                $"cmd://{raw.ToLowerInvariant()}",
                $"cmd://{raw.ToUpperInvariant()}"
            };
            if (raw.Length > 0)
            {
                var firstCap = char.ToUpperInvariant(raw[0]) + (raw.Length > 1 ? raw.Substring(1) : string.Empty);
                var firstLower = char.ToLowerInvariant(raw[0]) + (raw.Length > 1 ? raw.Substring(1) : string.Empty);
                candidates.Add($"cmd://{firstCap}");
                candidates.Add($"cmd://{firstLower}");
            }

            Func<CmdMessageEvent, UniTask> handlerToCall = null;
            string matchedChannel = null;
            foreach (var candidate in candidates)
            {
                if (EventBusRegistry.CmdMessageBus.TryGetHandler(candidate, out var handler))
                {
                    handlerToCall = handler;
                    matchedChannel = candidate;
                    break;
                }
            }

            if (handlerToCall != null)
            {
                DevLog.UI($"[DebugUI] RouteCommandAsync -> invoking handler for channel '{matchedChannel}'");
                await handlerToCall(evt);
            }
            else
            {
                DevLog.UI($"[DebugUI] RouteCommandAsync -> no handler found for command '{evt.Command}'");
                var notFound = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: "command not found", success: false);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", notFound);
            }
        }
        catch (Exception ex)
        {
            DevLog.UI($"[DebugUI] RouteCommandAsync -> exception invoking handler: {ex.Message}");
            var err = new CmdExecResultEvent(evt?.Command ?? string.Empty, evt?.Args ?? Array.Empty<string>(), evt?.UserId ?? string.Empty, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", err);
        }

        await UniTask.CompletedTask;
    }

    // 处理命令结果
    private static UniTask HandleCmdExecResultAsync(CmdExecResultEvent evt)
    {
        DevLog.UI($"[DebugUI] HandleCmdExecResultAsync called. Command={(evt==null?"<null>":evt.Command)} Success={(evt==null?"<null>":evt.Success.ToString())}");
        string colorHex = evt.Success ? "#32CD32" : "#FF4500"; // 成功为石灰绿，失败为橙红色
        string statusText = evt.Success ? "Success" : "Error";
        string richText = $"<color={colorHex}>[Cmd {statusText}]</color>: {evt.Stdout ?? evt.Stderr}";
        DevLog.UI($"[DebugUI] HandleCmdExecResultAsync -> AddMessage: '{richText}'");
    PeakChatOpsUI.Instance.AddMessage(richText);
    return UniTask.CompletedTask;
    }

    // diy 任何输出样式
    public static UniTask HandleAnyMessageAsync(string anyText)
    {
    DevLog.UI($"[DebugUI] HandleAnyMessageAsync -> AddMessage: '{anyText}'");
    PeakChatOpsUI.Instance.AddMessage(anyText);
    return UniTask.CompletedTask;
    }

}
