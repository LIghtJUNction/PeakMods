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
     // 静态保存所有命令元数据，供Help等调用
     public static readonly List<PCOCommandAttribute> CommandMetas = new List<PCOCommandAttribute>();
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
                if (t.IsAbstract || t.IsInterface) return;
                var ctor = t.GetConstructor(Type.EmptyTypes);
                if (ctor == null) return;
                Activator.CreateInstance(t);
                // 收集命令元数据
                var attr = t.GetCustomAttribute<PCOCommandAttribute>();
                if (attr != null)
                {
                    CommandMetas.Add(attr);
                }
                DevLog.UI($"[Cmdx] Instantiated command class: {t.FullName}");
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogWarning($"Failed to instantiate command class '{t.FullName}': {ex.Message}");
            }
        }
        // 清空元数据
        CommandMetas.Clear();
        // 命令执行器+加载器
        TryInstantiate(typeof(Cmdx));
        TryInstantiate(typeof(EchoCommand));
        TryInstantiate(typeof(ExitCommand));
        TryInstantiate(typeof(HideCommand));
        TryInstantiate(typeof(HelpCommand));
        TryInstantiate(typeof(SyncCommand));
        TryInstantiate(typeof(PingCommand));
        TryInstantiate(typeof(AICommand));

        // 通过反射加载插件目录下的 dll 并实例化带 PCOCommand 特性的类型以触发其构造函数，并收集元数据
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
                                    // TryInstantiate已收集元数据
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
