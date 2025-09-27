// Decompiled with JetBrains decompiler
// Type: MenuWindowTabbed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MenuWindowTabbed : MenuWindow
{
  protected List<MenuWindow> tabs = new List<MenuWindow>();
  private int currentTab;

  public virtual int startOnTab => 0;

  internal override void Open()
  {
    this.InitTabs();
    base.Open();
    this.SelectTab(this.startOnTab);
  }

  protected virtual void InitTabs()
  {
  }

  public void SelectTab(int index)
  {
    if (this.tabs.Count <= index || index < 0)
    {
      Debug.LogError((object) $"{this.gameObject.name} tried to select out of range tab: {index}");
    }
    else
    {
      for (int index1 = 0; index1 < this.tabs.Count; ++index1)
      {
        if (index1 == index)
          this.tabs[index1].Open();
        else
          this.tabs[index1].Close();
      }
      this.currentTab = index;
    }
  }

  public void SelectNextTab(bool forward)
  {
    this.currentTab += forward ? 1 : -1;
    if (this.currentTab >= this.tabs.Count)
      this.currentTab = 0;
    else if (this.currentTab < 0)
      this.currentTab = this.tabs.Count - 1;
    this.SelectTab(this.currentTab);
  }
}
