// Decompiled with JetBrains decompiler
// Type: ItemInstanceDataDebugPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core.CLI;

#nullable disable
public class ItemInstanceDataDebugPage : DebugPage
{
  private Dictionary<Guid, ItemInstanceDataUICell> m_spawnedCells = new Dictionary<Guid, ItemInstanceDataUICell>();
  private ScrollView ScrollView;

  public ItemInstanceDataDebugPage()
  {
    this.ScrollView = new ScrollView();
    this.Add((VisualElement) this.ScrollView);
  }

  public override void Update()
  {
    base.Update();
    List<ItemInstanceData> itemInstanceDataList = new List<ItemInstanceData>();
    foreach (Player allPlayer in PlayerHandler.GetAllPlayers())
    {
      foreach (ItemSlot itemSlot in allPlayer.itemSlots)
      {
        if (!itemSlot.IsEmpty())
          itemInstanceDataList.Add(itemSlot.data);
      }
    }
    foreach (ItemInstanceData data in itemInstanceDataList)
    {
      DataEntryValue dataEntryValue;
      if (!this.m_spawnedCells.ContainsKey(data.guid) && (data.data.Count != 1 || !data.data.TryGetValue(DataEntryKey.ItemUses, out dataEntryValue) || !(dataEntryValue is OptionableIntItemData optionableIntItemData) || optionableIntItemData.HasData))
      {
        ItemInstanceDataUICell child = new ItemInstanceDataUICell(data);
        this.ScrollView.Add((VisualElement) child);
        this.m_spawnedCells.Add(data.guid, child);
      }
    }
    foreach (KeyValuePair<Guid, ItemInstanceDataUICell> spawnedCell in this.m_spawnedCells)
      spawnedCell.Value.Update();
  }
}
