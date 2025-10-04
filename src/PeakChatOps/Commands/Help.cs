// 命令自动注册接口（如未全局定义则本地定义，防止找不到类型报错）
using System;
using System.Collections.Generic;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using System.Linq;

namespace PeakChatOps.Commands;

[PCOCommand("help", "显示所有可用命令及说明", "用法: /help\n列出所有命令和用法说明。")]
public class HelpCommand
{
    public HelpCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://help", Handle);
        DevLog.UI("[Cmd] HelpCommand subscribed to cmd://help");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var metas = Cmdx.CommandMetas;
            string output;
            if (metas.Count == 0)
            {
                output = "无可用命令。";
            }
            else
            {
                // 使用异步构建以避免阻塞调用线程（可能包含较多命令）
                output = await BuildHelpLinesAsync(metas);
            }
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: output, stderr: null, success: true);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }

        await UniTask.CompletedTask;
    }

    // 命令帮助缓存
    private static int _lastMetasHash = 0;
    private static string _lastHelpOutput = null;

    private static UniTask<string> BuildHelpLinesAsync(List<PCOCommandAttribute> metas)
    {
        // 计算哈希（只用 Name/Description/HelpInfo）
        int hash = 17;
        foreach (var meta in metas)
        {
            hash = hash * 31 + (meta.Name?.GetHashCode() ?? 0);
            hash = hash * 31 + (meta.Description?.GetHashCode() ?? 0);
            hash = hash * 31 + (meta.HelpInfo?.GetHashCode() ?? 0);
        }

        if (_lastHelpOutput != null && hash == _lastMetasHash)
        {
            return UniTask.FromResult(_lastHelpOutput);
        }

        // Build help text synchronously on the caller thread to avoid thread/context issues.
        var lines = new List<string>();
        lines.Add("<b><color=#59A6FF>可用命令：</color></b>");
        foreach (var meta in metas.Distinct(new PCOCommandAttributeComparer()))
        {
            var namePart = $"<b><color=#59A6FF>/{meta.Name}</color></b>";
            var descPart = string.IsNullOrWhiteSpace(meta.Description) ? "" : $" <color=#DED9C2>{meta.Description}</color>";
            var helpPart = string.IsNullOrWhiteSpace(meta.HelpInfo) ? "" : $"\n  <size=90%><i><color=#C0C0C0>{meta.HelpInfo}</color></i></size>";
            lines.Add(namePart + descPart + helpPart);
        }
        var output = string.Join("\n", lines);

        _lastMetasHash = hash;
        _lastHelpOutput = output;
        return UniTask.FromResult(output);
    }

    // 简单的比较器：按 Name 去重
    internal class PCOCommandAttributeComparer : IEqualityComparer<PCOCommandAttribute>
    {
        public bool Equals(PCOCommandAttribute x, PCOCommandAttribute y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PCOCommandAttribute obj)
        {
            return (obj.Name ?? string.Empty).ToLowerInvariant().GetHashCode();
        }

    }

}