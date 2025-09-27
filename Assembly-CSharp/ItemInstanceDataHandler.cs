// Decompiled with JetBrains decompiler
// Type: ItemInstanceDataHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
public class ItemInstanceDataHandler : RetrievableSingleton<ItemInstanceDataHandler>
{
  private Dictionary<Guid, ItemInstanceData> m_instanceData = new Dictionary<Guid, ItemInstanceData>();

  public IEnumerable<ItemInstanceData> GetAllItemInstances()
  {
    return (IEnumerable<ItemInstanceData>) this.m_instanceData.Values;
  }

  protected override void OnCreated()
  {
    base.OnCreated();
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
  }

  public static void AddInstanceData(ItemInstanceData instanceData)
  {
    if (!RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryAdd(instanceData.guid, instanceData))
      throw new Exception($"Adding item instance with duplicate guid: {instanceData.guid}");
  }

  public static bool TryGetInstanceData(Guid guid, out ItemInstanceData o)
  {
    ItemInstanceData itemInstanceData;
    if (RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryGetValue(guid, out itemInstanceData))
    {
      o = itemInstanceData;
      return true;
    }
    o = (ItemInstanceData) null;
    return false;
  }
}
