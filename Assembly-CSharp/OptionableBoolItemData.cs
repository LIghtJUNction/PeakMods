// Decompiled with JetBrains decompiler
// Type: OptionableBoolItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core.Serizalization;

#nullable disable
public class OptionableBoolItemData : DataEntryValue
{
  public bool HasData;
  public bool Value;

  public override void SerializeValue(BinarySerializer serializer)
  {
    serializer.WriteBool(this.HasData);
    if (!this.HasData)
      return;
    serializer.WriteBool(this.Value);
  }

  public override void DeserializeValue(BinaryDeserializer deserializer)
  {
    this.HasData = deserializer.ReadBool();
    if (!this.HasData)
      return;
    this.Value = deserializer.ReadBool();
  }

  public override string ToString() => !this.HasData ? "No Data" : this.Value.ToString();
}
