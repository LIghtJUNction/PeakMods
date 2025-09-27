// Decompiled with JetBrains decompiler
// Type: BackpackReference
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public struct BackpackReference : IBinarySerializable
{
  public BackpackReference.BackpackType type;
  public PhotonView view;
  public Transform locationTransform;

  public static BackpackReference GetFromBackpackItem(Item item)
  {
    return new BackpackReference()
    {
      type = BackpackReference.BackpackType.Item,
      view = item.GetComponent<PhotonView>(),
      locationTransform = item.transform
    };
  }

  public static BackpackReference GetFromEquippedBackpack(Character character)
  {
    return new BackpackReference()
    {
      type = BackpackReference.BackpackType.Equipped,
      view = character.GetComponent<PhotonView>(),
      locationTransform = character.GetBodypart(BodypartType.Torso).transform
    };
  }

  public BackpackVisuals GetVisuals()
  {
    return this.type == BackpackReference.BackpackType.Item ? (BackpackVisuals) this.view.GetComponent<ItemBackpackVisuals>() : (BackpackVisuals) this.view.GetComponent<CharacterBackpackHandler>().backpackVisuals;
  }

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteByte((byte) this.type);
    serializer.WriteInt(this.view.ViewID);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.type = (BackpackReference.BackpackType) deserializer.ReadByte();
    this.view = PhotonView.Find(deserializer.ReadInt());
  }

  public ItemInstanceData GetItemInstanceData()
  {
    return this.type == BackpackReference.BackpackType.Item ? this.view.GetComponent<Item>().data : this.view.GetComponent<Character>().player.backpackSlot.data;
  }

  public BackpackData GetData()
  {
    if (this.type == BackpackReference.BackpackType.Item)
      return this.view.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
    BackpackData data;
    if (!this.view.GetComponent<Character>().player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out data))
      data = this.view.GetComponent<Character>().player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
    return data;
  }

  public bool IsOnMyBack() => this.type != BackpackReference.BackpackType.Item && this.view.IsMine;

  public bool TryGetBackpackItem(out Backpack backpack)
  {
    if (this.type == BackpackReference.BackpackType.Item)
    {
      backpack = this.view.GetComponent<Backpack>();
      return true;
    }
    backpack = (Backpack) null;
    return false;
  }

  public enum BackpackType : byte
  {
    Item,
    Equipped,
  }
}
