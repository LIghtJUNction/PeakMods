using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using Steamworks;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("invite", "邀请玩家", "用法: /invite <玩家ID>\n邀请指定的玩家加入游戏。")]
public class InviteCommand
{
    // 注册
    public InviteCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://invite", Handle);
        DevLog.UI("[Cmd] InviteCommand subscribed to cmd://invite");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var args = evt.Args ?? Array.Empty<string>();
            var res = "已打开邀请界面，请选择要邀请的玩家。";
            InviteFriends();
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: res, stderr: null, success: true);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
    public static void InviteFriends()
    {
      CSteamID lobbyID;
      if (!GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out lobbyID))
        return;
      SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
    }






}







//    CSteamID lobbyID;
//    if (!GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out lobbyID))
//      return;
//    SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
