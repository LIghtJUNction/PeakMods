// Decompiled with JetBrains decompiler
// Type: DefaultConnectionState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DefaultConnectionState : ConnectionState
{
  public override void Enter()
  {
    base.Enter();
    if (Time.frameCount <= 3)
      return;
    GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
  }
}
