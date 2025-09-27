// Decompiled with JetBrains decompiler
// Type: OnNetworkStart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;

#nullable disable
public abstract class OnNetworkStart : MonoBehaviourPunCallbacks
{
  private bool hasRunNetworkStart;

  private void Start() => this.TryRunningNetworkStart();

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    this.TryRunningNetworkStart();
  }

  private void TryRunningNetworkStart()
  {
    if (this.hasRunNetworkStart || !PhotonNetwork.InRoom)
      return;
    this.NetworkStart();
    this.hasRunNetworkStart = true;
  }

  public abstract void NetworkStart();
}
