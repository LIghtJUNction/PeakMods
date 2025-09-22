using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System; // for StringSplitOptions
using PeakChatOps.Commands;

namespace PeakChatOps.Core;

public static class MsgHandlerChain
{
    #region 下行消息处理
    static MsgHandlerChain()
    {
        Cmdx.LoadPCmd();
        Cmdx.Prefix = PeakChatOpsPlugin.CmdPrefix.Value;
    }
    // 处理下行消息的链
    public static void IncomingMessageChain(MessageData msg)
    {
        // Nickname 颜色映射
        string colorHex = "#FFFFFF"; // 默认白色
        string lowerName = msg.Nickname?.ToLowerInvariant() ?? "";
        switch (lowerName)
        {
            case "system":
                colorHex = "#FFD700"; // 金色
                break;
            case "host":
                colorHex = "#00BFFF"; // 深天蓝
                break;
            case "cmd":
                colorHex = "#FF69B4"; // 粉色
                break;
            case "you":
                colorHex = "#7CFC00"; // 草绿色
                break;
            default:
                // 普通玩家：尝试从 Extra 取 color 字段
                if (msg.Extra != null && msg.Extra.TryGetValue("color", out var cObj) && cObj is string cStr)
                    colorHex = cStr;
                break;
        }
        // 富文本输出
        string richText = $"<color={colorHex}>[{msg.Nickname}]</color>: {msg.Message}";
        PeakOpsUI.instance.AddMessage(richText);
    }
    #endregion

    #region 上行消息处理
    // 处理上行消息的链
    public static void OutgoingMessageChain(byte eventCode, object[] payload)
    {
        PeakChatOpsPlugin.Logger.LogDebug($"[MsgHandlerChain.OutgoingMessageChain] called, eventCode={eventCode}, payload={string.Join(",", payload)}");
        if (IsLocalMessage(payload))
        {
            PeakChatOpsPlugin.Logger.LogDebug("[MsgHandlerChain.OutgoingMessageChain] Local message detected, not sending to network");
            // 指令消息
            IncomingMessageChain(new MessageData("CMD", payload[1]?.ToString() ?? "", "local", Character.localCharacter.data.dead));
            // 系统响应
            string input = payload[1]?.ToString() ?? $"{PeakChatOpsPlugin.CmdPrefix.Value}help";
            string result;
            if (input.StartsWith(Cmdx.Prefix))
            {
                var cmdName = input.Substring(Cmdx.Prefix.Length).Split(' ')[0];
                var cmd = Cmdx.GetCommand(cmdName);
                if (cmd != null)
                {
                    var args = input.Substring(Cmdx.Prefix.Length + cmdName.Length).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    result = cmd.Handler(args);
                }
                else
                {
                    result = $"未知命令: {cmdName}，输入{Cmdx.Prefix}help 查看所有命令。";
                }
            }
            else
            {
                result = "无效命令格式。";
            }
            IncomingMessageChain(new MessageData("system", result, "system", false));
            return; // 指令消息不发送到网络
        }

        // 发送给其他人
        PeakChatOpsPlugin.Logger.LogDebug("[MsgHandlerChain.OutgoingMessageChain] Sending to PhotonNetwork");
        PhotonNetwork.RaiseEvent(
            eventCode,
            payload,
            new RaiseEventOptions() { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable
        );
        // 本地发送一份给自己
        IncomingMessageChain(new MessageData("You", payload[1]?.ToString() ?? "", "local", Character.localCharacter.data.dead));
    }

    public static bool IsLocalMessage(object[] payload)
    {
        // 检查消息内容是否为本地命令
        // 消息以命令前缀开头
        if (payload.Length > 1 && payload[1] is string msg && msg.StartsWith(PeakChatOpsPlugin.CmdPrefix.Value))
        {
            return true;
        }

        return false;
    }
    #endregion
}

