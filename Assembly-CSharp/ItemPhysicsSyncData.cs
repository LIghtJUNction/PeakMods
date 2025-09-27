// Decompiled with JetBrains decompiler
// Type: ItemPhysicsSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public struct ItemPhysicsSyncData : IBinarySerializable
{
  public float3 position;
  public Quaternion rotation;
  public float3 linearVelocity;
  public float3 angularVelocity;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat3(this.position);
    serializer.WriteQuaternion(this.rotation);
    serializer.WriteHalf3((half3) this.linearVelocity);
    serializer.WriteHalf3((half3) this.angularVelocity);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.position = deserializer.ReadFloat3();
    this.rotation = deserializer.ReadQuaternion();
    this.linearVelocity = (float3) deserializer.ReadHalf3();
    this.angularVelocity = (float3) deserializer.ReadHalf3();
  }
}
