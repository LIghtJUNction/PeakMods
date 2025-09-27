// Decompiled with JetBrains decompiler
// Type: SettingsTABSButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.UI;

#nullable disable
public class SettingsTABSButton : TAB_Button
{
  public SettingsCategory category;
  public GameObject SelectedGraphic;

  private void Update()
  {
    this.text.color = Color.Lerp(this.text.color, this.Selected ? Color.black : Color.white, Time.unscaledDeltaTime * 7f);
    this.SelectedGraphic.gameObject.SetActive(this.Selected);
  }
}
