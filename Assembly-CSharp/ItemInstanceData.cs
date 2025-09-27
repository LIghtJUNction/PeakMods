// Decompiled with JetBrains decompiler
// Type: ItemInstanceData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Core.Serizalization;

#nullable disable
public class ItemInstanceData : IBinarySerializable
{
  public Guid guid;
  public Dictionary<DataEntryKey, DataEntryValue> data = new Dictionary<DataEntryKey, DataEntryValue>();

  public ItemInstanceData()
  {
  }

  public ItemInstanceData(Guid guid) => this.guid = guid;

  public void Serialize(BinarySerializer serializer)
  {
    List<KeyValuePair<DataEntryKey, DataEntryValue>> list = this.data.ToList<KeyValuePair<DataEntryKey, DataEntryValue>>();
    byte count = (byte) list.Count;
    serializer.WriteByte(count);
    foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in list)
    {
      DataEntryKey key = keyValuePair.Key;
      DataEntryValue dataEntryValue = keyValuePair.Value;
      serializer.WriteByte((byte) key);
      serializer.WriteByte(DataEntryValue.GetTypeValue(dataEntryValue.GetType()));
      dataEntryValue.Serialize(serializer);
    }
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    byte capacity = deserializer.ReadByte();
    this.data = new Dictionary<DataEntryKey, DataEntryValue>((int) capacity);
    for (int index = 0; index < (int) capacity; ++index)
    {
      DataEntryKey key = (DataEntryKey) deserializer.ReadByte();
      DataEntryValue newFromValue = DataEntryValue.GetNewFromValue(deserializer.ReadByte());
      newFromValue.Init();
      newFromValue.Deserialize(deserializer);
      this.data.Add(key, newFromValue);
    }
  }

  public bool HasData(DataEntryKey key) => this.data.ContainsKey(key);

  public bool TryGetDataEntry<T>(DataEntryKey key, out T value) where T : DataEntryValue
  {
    DataEntryValue dataEntryValue;
    int num = this.data.TryGetValue(key, out dataEntryValue) ? 1 : 0;
    if (num != 0)
    {
      value = (T) dataEntryValue;
      return num != 0;
    }
    value = default (T);
    return num != 0;
  }

  public T RegisterNewEntry<T>(DataEntryKey key) where T : DataEntryValue, new()
  {
    T obj = new T();
    obj.Init();
    this.data.Add(key, (DataEntryValue) obj);
    return obj;
  }

  public T RegisterEntry<T>(DataEntryKey key, T value) where T : DataEntryValue, new()
  {
    value.Init();
    this.data.Add(key, (DataEntryValue) value);
    return value;
  }
}
