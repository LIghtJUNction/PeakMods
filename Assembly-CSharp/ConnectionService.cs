// Decompiled with JetBrains decompiler
// Type: ConnectionService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class ConnectionService : GameService
{
  public ConnectionService.ConnectionServiceStateMachine StateMachine;

  public ConnectionService()
  {
    this.StateMachine = new ConnectionService.ConnectionServiceStateMachine();
    this.StateMachine.RegisterState((ConnectionState) new DefaultConnectionState());
    this.StateMachine.RegisterState((ConnectionState) new JoinSpecificRoomState());
    this.StateMachine.RegisterState((ConnectionState) new InRoomState());
    this.StateMachine.RegisterState((ConnectionState) new HostState());
    this.StateMachine.SwitchState<DefaultConnectionState>();
  }

  public class ConnectionServiceStateMachine : Zorro.Core.StateMachine<ConnectionState>
  {
  }
}
