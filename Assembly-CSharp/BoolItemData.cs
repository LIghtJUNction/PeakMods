// Decompiled with JetBrains decompiler
// Type: BoolItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core.Serizalization;

#nullable disable
public class BoolItemData : DataEntryValue
{
  public bool Value;

  public override void SerializeValue(BinarySerializer serializer)
  {
    serializer.WriteBool(this.Value);
  }

  public override void DeserializeValue(BinaryDeserializer deserializer)
  {
    this.Value = deserializer.ReadBool();
  }

  public override string ToString() => this.Value.ToString();
}
