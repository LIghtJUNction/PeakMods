// Decompiled with JetBrains decompiler
// Type: RopeSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public struct RopeSyncData : IBinarySerializable
{
  public RopeSyncData.SegmentData[] segments;
  public bool isVisible;
  public bool updateVisualizerManually;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteBool(this.isVisible);
    serializer.WriteBool(this.updateVisualizerManually);
    if (this.segments == null)
    {
      serializer.WriteUshort((ushort) 0);
    }
    else
    {
      ushort length = (ushort) this.segments.Length;
      serializer.WriteUshort(length);
      for (int index = 0; index < (int) length; ++index)
        this.segments[index].Serialize(serializer);
    }
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.isVisible = deserializer.ReadBool();
    this.updateVisualizerManually = deserializer.ReadBool();
    ushort length = deserializer.ReadUShort();
    this.segments = new RopeSyncData.SegmentData[(int) length];
    for (int index = 0; index < (int) length; ++index)
      this.segments[index] = IBinarySerializable.Deserialize<RopeSyncData.SegmentData>(deserializer);
  }

  public struct SegmentData : IBinarySerializable
  {
    public float3 position;
    public Quaternion rotation;

    public void Serialize(BinarySerializer serializer)
    {
      serializer.WriteFloat3(this.position);
      serializer.WriteQuaternion(this.rotation);
    }

    public void Deserialize(BinaryDeserializer deserializer)
    {
      this.position = deserializer.ReadFloat3();
      this.rotation = deserializer.ReadQuaternion();
    }
  }
}
