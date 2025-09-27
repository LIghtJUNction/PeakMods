// Decompiled with JetBrains decompiler
// Type: SyncLavaRisingPackage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

#nullable disable
public class SyncLavaRisingPackage : CustomCommandPackage<CustomCommandType>
{
  public bool Started;
  public bool Ended;
  public float Time;
  public float TimeWaited;

  public SyncLavaRisingPackage()
  {
  }

  public SyncLavaRisingPackage(bool started, bool ended, float time, float timeWaited)
  {
    this.Started = started;
    this.Ended = ended;
    this.Time = time;
    this.TimeWaited = timeWaited;
  }

  protected override void SerializeData(BinarySerializer binarySerializer)
  {
    binarySerializer.WriteBool(this.Started);
    binarySerializer.WriteBool(this.Ended);
    binarySerializer.WriteFloat(this.Time);
    binarySerializer.WriteFloat(this.TimeWaited);
  }

  public override void DeserializeData(BinaryDeserializer binaryDeserializer)
  {
    this.Started = binaryDeserializer.ReadBool();
    this.Ended = binaryDeserializer.ReadBool();
    this.Time = binaryDeserializer.ReadFloat();
    this.TimeWaited = binaryDeserializer.ReadFloat();
  }

  public override CustomCommandType GetCommandType() => CustomCommandType.SyncLavaRising;

  public override SendOptions GetSendOptions() => SendOptions.SendReliable;
}
