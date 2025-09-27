// Decompiled with JetBrains decompiler
// Type: SyncPersistentPlayerDataPackage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

#nullable disable
public class SyncPersistentPlayerDataPackage : CustomCommandPackage<CustomCommandType>
{
  public int ActorNumber;

  public PersistentPlayerData Data { get; set; }

  protected override void SerializeData(BinarySerializer binarySerializer)
  {
    binarySerializer.WriteInt(this.ActorNumber);
    this.Data.Serialize(binarySerializer);
  }

  public override void DeserializeData(BinaryDeserializer binaryDeserializer)
  {
    this.ActorNumber = binaryDeserializer.ReadInt();
    this.Data = IBinarySerializable.DeserializeClass<PersistentPlayerData>(binaryDeserializer);
  }

  public override CustomCommandType GetCommandType() => CustomCommandType.SyncPersistentPlayerData;

  public override SendOptions GetSendOptions() => SendOptions.SendReliable;
}
