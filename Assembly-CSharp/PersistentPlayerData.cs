// Decompiled with JetBrains decompiler
// Type: PersistentPlayerData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core.Serizalization;

#nullable disable
public class PersistentPlayerData : IBinarySerializable
{
  public CharacterCustomizationData customizationData = new CharacterCustomizationData();

  public void Serialize(BinarySerializer serializer)
  {
    this.customizationData.Serialize(serializer);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.customizationData = IBinarySerializable.DeserializeClass<CharacterCustomizationData>(deserializer);
  }
}
