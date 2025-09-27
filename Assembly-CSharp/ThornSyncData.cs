// Decompiled with JetBrains decompiler
// Type: ThornSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Zorro.Core.Serizalization;

#nullable disable
public struct ThornSyncData : IBinarySerializable
{
  public List<ushort> stuckThornIndices;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteInt(this.stuckThornIndices.Count);
    for (int index = 0; index < this.stuckThornIndices.Count; ++index)
      serializer.WriteUshort(this.stuckThornIndices[index]);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.stuckThornIndices = new List<ushort>();
    int num = deserializer.ReadInt();
    for (ushort index = 0; (int) index < num; ++index)
      this.stuckThornIndices.Add(deserializer.ReadUShort());
  }
}
