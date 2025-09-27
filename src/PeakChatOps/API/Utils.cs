using System;
using Photon.Pun;

namespace PeakChatOps.API;

public static class ChatApiUtil
{
    public static int NameToActorId(string name)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player != null && player.NickName == name)
                return player.ActorNumber;
        }
        return -1;
    }

}

