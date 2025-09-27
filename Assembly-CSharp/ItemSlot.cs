// Decompiled with JetBrains decompiler
// Type: ItemSlot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class ItemSlot
{
  public Item prefab;
  public ItemInstanceData data;
  public byte itemSlotID;

  public ItemSlot(byte slotID) => this.itemSlotID = slotID;

  public virtual bool IsEmpty() => (UnityEngine.Object) this.prefab == (UnityEngine.Object) null;

  public void SetItem(Item itemPrefab, ItemInstanceData itemData)
  {
    this.data = itemData;
    this.prefab = itemPrefab;
  }

  public virtual void EmptyOut() => this.prefab = (Item) null;

  public override string ToString()
  {
    return $"Slot ({this.itemSlotID}): {((UnityEngine.Object) this.prefab == (UnityEngine.Object) null ? (object) "null" : (object) this.prefab.name)}";
  }

  public virtual string GetPrefabName()
  {
    return (UnityEngine.Object) this.prefab == (UnityEngine.Object) null ? "" : this.prefab.gameObject.name;
  }
}
