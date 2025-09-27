// Decompiled with JetBrains decompiler
// Type: SettingsTABS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine.InputSystem;
using Zorro.UI;

#nullable disable
public class SettingsTABS : TABS<SettingsTABSButton>
{
  public SharedSettingsMenu SettingsMenu;
  public InputActionReference RightAction;
  public InputActionReference LeftAction;

  public override void OnSelected(SettingsTABSButton button)
  {
    this.SettingsMenu.ShowSettings(button.category);
  }

  private void Update()
  {
    if (this.RightAction.action.WasPressedThisFrame())
    {
      this.SelectNext();
    }
    else
    {
      if (!this.LeftAction.action.WasPressedThisFrame())
        return;
      this.SelectPrevious();
    }
  }
}
