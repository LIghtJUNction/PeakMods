// Decompiled with JetBrains decompiler
// Type: StatusSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Zorro.Core.Serizalization;

#nullable disable
public struct StatusSyncData : IBinarySerializable
{
  public List<float> statusList;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteInt(this.statusList.Count);
    for (int index = 0; index < this.statusList.Count; ++index)
      serializer.WriteFloat(this.statusList[index]);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    int num = deserializer.ReadInt();
    this.statusList = new List<float>();
    for (int index = 0; index < num; ++index)
      this.statusList.Add(deserializer.ReadFloat());
  }
}
