// Decompiled with JetBrains decompiler
// Type: SyncMapHandlerDebugCommandPackage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

#nullable disable
public class SyncMapHandlerDebugCommandPackage : CustomCommandPackage<CustomCommandType>
{
  public int[] PlayerToTeleport;
  public Segment Segment;

  public SyncMapHandlerDebugCommandPackage()
  {
  }

  public SyncMapHandlerDebugCommandPackage(Segment segment, int[] playersToTeleport)
  {
    this.Segment = segment;
    this.PlayerToTeleport = playersToTeleport;
  }

  protected override void SerializeData(BinarySerializer binarySerializer)
  {
    binarySerializer.WriteByte((byte) this.Segment);
    binarySerializer.WriteByte((byte) this.PlayerToTeleport.Length);
    foreach (int num in this.PlayerToTeleport)
      binarySerializer.WriteInt(num);
  }

  public override void DeserializeData(BinaryDeserializer binaryDeserializer)
  {
    this.Segment = (Segment) binaryDeserializer.ReadByte();
    byte length = binaryDeserializer.ReadByte();
    this.PlayerToTeleport = new int[(int) length];
    for (int index = 0; index < (int) length; ++index)
      this.PlayerToTeleport[index] = binaryDeserializer.ReadInt();
  }

  public override CustomCommandType GetCommandType()
  {
    return CustomCommandType.SyncMapHandlerDebugCommand;
  }

  public override SendOptions GetSendOptions() => SendOptions.SendReliable;
}
