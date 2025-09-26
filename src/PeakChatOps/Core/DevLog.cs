using System.Diagnostics;

using HarmonyLib.Tools;

namespace PeakChatOps.Core
{
    public static class DevLog
    {
        // Only compile calls when DEBUGUI is defined; method body still compiles but calls removed otherwise
        [Conditional("DEBUGUI")]
        public static void UI(string message)
        {
            try
            {
                HarmonyFileLog.Enabled = true;
                PeakChatOpsPlugin.Logger.LogDebug(message);
                // 判空，只有UI实例存在时才调度
                if (PeakOpsUI.instance != null)
                    MainThreadDispatcher.Run(() => PeakOpsUI.instance.AddMessage("<color=#FFA500>[DevLog]</color> " + message));
            }
            catch { }
        }
    }
}
