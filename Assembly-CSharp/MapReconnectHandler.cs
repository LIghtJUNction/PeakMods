// Decompiled with JetBrains decompiler
// Type: MapReconnectHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using Zorro.Core;
using Zorro.PhotonUtility;

#nullable disable
public class MapReconnectHandler : MonoBehaviourPunCallbacks
{
  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (newPlayer.IsLocal || !PhotonNetwork.IsMasterClient)
      return;
    RaiseEventOptions eventOptions = RaiseEventOptions.Default;
    eventOptions.TargetActors = new int[1]
    {
      newPlayer.ActorNumber
    };
    CustomCommands<CustomCommandType>.SendPackage((CustomCommandPackage<CustomCommandType>) new SyncMapHandlerDebugCommandPackage(Singleton<MapHandler>.Instance.GetCurrentSegment(), new int[1]
    {
      newPlayer.ActorNumber
    }), eventOptions);
  }
}
