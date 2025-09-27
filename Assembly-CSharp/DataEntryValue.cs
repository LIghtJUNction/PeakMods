// Decompiled with JetBrains decompiler
// Type: DataEntryValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using Zorro.Core.Serizalization;

#nullable disable
public abstract class DataEntryValue : IBinarySerializable
{
  public void Serialize(BinarySerializer serializer) => this.SerializeValue(serializer);

  public void Deserialize(BinaryDeserializer deserializer) => this.DeserializeValue(deserializer);

  public virtual void Init()
  {
  }

  public abstract void SerializeValue(BinarySerializer serializer);

  public abstract void DeserializeValue(BinaryDeserializer deserializer);

  public static byte GetTypeValue(System.Type type)
  {
    if (type == typeof (IntItemData))
      return 1;
    if (type == typeof (OptionableIntItemData))
      return 2;
    if (type == typeof (BoolItemData))
      return 3;
    if (type == typeof (FloatItemData))
      return 4;
    if (type == typeof (OptionableBoolItemData))
      return 5;
    if (type == typeof (BackpackData))
      return 6;
    return type == typeof (ColorItemData) ? (byte) 7 : (byte) 0;
  }

  public static DataEntryValue GetNewFromValue(byte value)
  {
    switch (value)
    {
      case 1:
        return (DataEntryValue) new IntItemData();
      case 2:
        return (DataEntryValue) new OptionableIntItemData();
      case 3:
        return (DataEntryValue) new BoolItemData();
      case 4:
        return (DataEntryValue) new FloatItemData();
      case 5:
        return (DataEntryValue) new OptionableBoolItemData();
      case 6:
        return (DataEntryValue) new BackpackData();
      case 7:
        return (DataEntryValue) new ColorItemData();
      default:
        throw new NotImplementedException();
    }
  }
}
