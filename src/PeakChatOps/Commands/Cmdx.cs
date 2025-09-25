using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
#nullable enable
using BepInEx;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("cmdx", "命令执行器", "用法: /cmdx <cmd> <args>\n执行指定的命令。")]
public class Cmdx
{
    // 全局命令前缀（用于外部引用）
    public static string Prefix = PeakChatOpsPlugin.CmdPrefix.Value;
    public Cmdx()
    {
        // 当通过中央路由发送到 cmd://cmdx 时，Cmdx 的 Handle 会被调用
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://cmdx", Handle);
        DevLog.UI("[Cmdx] Subscribed to cmd://cmdx");
    }

    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            // 合并 command/args：如果命令被当作第一个参数传入则使用它
            var args = evt.Args ?? Array.Empty<string>();
            // 兼容 null，保证 args 不为 null
            if (args.Length == 0 && !string.IsNullOrWhiteSpace(evt.Command))
            {
                // treat evt.Command as the subcommand when dispatched through route
                args = new string[] { evt.Command };
                CmdMessageEvent cmdEvent = new CmdMessageEvent(evt.Command, args, "cmdx");
                await EventBusRegistry.CmdMessageBus.Publish("cmd://", cmdEvent);
                return;
            }
            else if (args.Length == 0)
            {
                var errEvt = new CmdExecResultEvent(evt.Command, Array.Empty<string>(), evt.UserId, stdout: null, stderr: "用法: /cmdx <cmd> <args>\n请提供要执行的命令。", success: false);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
                return;
            }

                var subCommand = args[0];
                var subArgs = args.Skip(1).ToArray();
                CmdMessageEvent subCmdEvent = new CmdMessageEvent(subCommand, subArgs, "cmdx");
                await EventBusRegistry.CmdMessageBus.Publish("cmd://", subCmdEvent);
                return;

        }

        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }

    // removed obsolete Main() per new design (no backward compatibility required)

    public static void LoadPCmd()
    {
        // Helper: 尝试实例化类型以触发其构造函数（构造函数应负责订阅事件总线）
        void TryInstantiate(Type t)
        {
            try
            {
                // 仅对公开的、非抽象、可实例化类型尝试
                if (t.IsAbstract || t.IsInterface) return;
                var ctor = t.GetConstructor(Type.EmptyTypes);
                if (ctor == null) return; // 无无参构造则跳过
                Activator.CreateInstance(t);
                DevLog.UI($"[Cmdx] Instantiated command class: {t.FullName}");
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogWarning($"Failed to instantiate command class '{t.FullName}': {ex.Message}");
            }
        }
        // 命令执行器+加载器
        TryInstantiate(typeof(Cmdx));
        // 快速入手命令开发的示例命令
        TryInstantiate(typeof(EchoCommand));
        // 基础命令
        TryInstantiate(typeof(ExitCommand));
        // 没啥用的命令
        TryInstantiate(typeof(HideCommand));
        // 帮助命令
        TryInstantiate(typeof(HelpCommand));
        // 有用但是不多的同步命令
        TryInstantiate(typeof(SyncCommand));
        // 看看其他人有没有安装相同插件/版本的命令
        TryInstantiate(typeof(PingCommand));


        // 通过反射加载插件目录下的 dll 并实例化带 PCOCommand 特性的类型以触发其构造函数
        string pluginsDir = Paths.PluginPath;
        try
        {
            foreach (var dir in Directory.EnumerateDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                if (string.Equals(dirName, "PeakChatOps", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!dirName.StartsWith("PeakChatOps", StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (var dll in Directory.EnumerateFiles(dir, "*.dll"))
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(dll);
                        foreach (var type in asm.GetTypes())
                        {
                            try
                            {
                                var attr = type.GetCustomAttribute<PCOCommandAttribute>();
                                if (attr != null)
                                {
                                    TryInstantiate(type);
                                }
                            }
                            catch (Exception exType)
                            {
                                PeakChatOpsPlugin.Logger.LogWarning($"Error processing type {type.FullName} in {dll}: {exType.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PeakChatOpsPlugin.Logger.LogWarning($"Failed to load assembly {dll}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            PeakChatOpsPlugin.Logger.LogError($"Error enumerating plugin directories: {ex.Message}");
        }
    }

}
