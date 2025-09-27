// Decompiled with JetBrains decompiler
// Type: ColorItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public class ColorItemData : DataEntryValue
{
  public Color Value;

  public override void SerializeValue(BinarySerializer serializer)
  {
    serializer.WriteFloat4(new float4(this.Value.r, this.Value.g, this.Value.b, this.Value.a));
  }

  public override void DeserializeValue(BinaryDeserializer deserializer)
  {
    float4 float4 = deserializer.ReadFloat4();
    this.Value = new Color(float4.x, float4.y, float4.z, float4.w);
  }

  public override string ToString() => this.Value.ToString();
}
