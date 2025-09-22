using BepInEx;
using BepInEx.Logging;
using PeakChatOps.Extra.Commands;

namespace PeakChatOps.Extra;

[BepInAutoPlugin]
[BepInDependency("com.github.LIghtJUNction.PeakChatOps", BepInDependency.DependencyFlags.HardDependency)]
partial class PeakChatOpsPlugin : BaseUnityPlugin
{

    private void Awake()
    {
        Logger.LogInfo($"{name} is loaded!");
        // 注册命令
        FuckCommand.Register();
    }
}