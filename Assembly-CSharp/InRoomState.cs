// Decompiled with JetBrains decompiler
// Type: InRoomState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.PhotonUtility;

#nullable disable
public class InRoomState : ConnectionState
{
  public bool hasLoadedCustomization;
  public string verifiedLobby;

  public override void Enter()
  {
    base.Enter();
    this.verifiedLobby = (string) null;
    this.hasLoadedCustomization = false;
    CommandListener commandListener = CustomCommands<CustomCommandType>.SpawnCommandListener<CommandListener>();
    commandListener.RegisterPackage<SyncPersistentPlayerDataPackage>(new SyncPersistentPlayerDataPackage());
    commandListener.RegisterPackage<SyncMapHandlerDebugCommandPackage>(new SyncMapHandlerDebugCommandPackage());
    commandListener.RegisterPackage<SyncLavaRisingPackage>(new SyncLavaRisingPackage());
    GameHandler.ClearAllStatuses();
    GameHandler.GetService<RichPresenceService>().Dirty();
  }
}
