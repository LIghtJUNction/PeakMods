// Decompiled with JetBrains decompiler
// Type: FloatItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Globalization;
using Zorro.Core.Serizalization;

#nullable disable
public class FloatItemData : DataEntryValue
{
  public float Value;

  public override void SerializeValue(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.Value);
  }

  public override void DeserializeValue(BinaryDeserializer deserializer)
  {
    this.Value = deserializer.ReadFloat();
  }

  public override string ToString()
  {
    return this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
