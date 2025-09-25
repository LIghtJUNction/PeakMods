// 命令自动注册接口（如未全局定义则本地定义，防止找不到类型报错）

using System;
using System.Collections.Generic;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using System.Linq;
using System.Reflection;
using System.IO;
using BepInEx;

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
            string output;
            var metas = new List<PCOCommandAttribute>();

            // 从当前域已加载的程序集收集 PCOCommandAttribute
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); } catch { continue; }
                foreach (var t in types)
                {
                    try
                    {
                        var attr = t.GetCustomAttribute<PCOCommandAttribute>();
                        if (attr != null) metas.Add(attr);
                    }
                    catch { }
                }
            }

            // 同时扫描插件目录下的 dll（若尚未加载）以便包含插件命令
            try
            {
                var pluginsDir = Paths.PluginPath;
                foreach (var dir in Directory.EnumerateDirectories(pluginsDir))
                {
                    foreach (var dll in Directory.EnumerateFiles(dir, "*.dll"))
                    {
                        try
                        {
                            var asm = Assembly.LoadFrom(dll);
                            Type[] types;
                            try { types = asm.GetTypes(); } catch { continue; }
                            foreach (var t in types)
                            {
                                try
                                {
                                    var attr = t.GetCustomAttribute<PCOCommandAttribute>();
                                    if (attr != null) metas.Add(attr);
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            if (metas.Count == 0)
            {
                output = "无可用命令。";
            }
            else
            {
                var lines = new List<string>();
                // 标题
                lines.Add("<b><color=#59A6FF>可用命令：</color></b>");
                foreach (var meta in metas.Distinct(new PCOCommandAttributeComparer()))
                {
                    // 构建富文本：命令名（蓝色粗体）、描述（淡色）、帮助信息（小号斜体灰色）
                    var namePart = $"<b><color=#59A6FF>/{meta.Name}</color></b>";
                    var descPart = string.IsNullOrWhiteSpace(meta.Description) ? "" : $" <color=#DED9C2>{meta.Description}</color>";
                    var helpPart = string.IsNullOrWhiteSpace(meta.HelpInfo) ? "" : $"\n  <size=90%><i><color=#C0C0C0>{meta.HelpInfo}</color></i></size>";
                    lines.Add(namePart + descPart + helpPart);
                }
                output = string.Join("\n", lines);
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

