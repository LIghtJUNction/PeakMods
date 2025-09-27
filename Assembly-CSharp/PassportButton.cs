// Decompiled with JetBrains decompiler
// Type: PassportButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class PassportButton : MonoBehaviour
{
  public Button button;
  public PassportManager manager;
  public RawImage icon;
  public RawImage lockedIcon;
  public Image border;
  private CustomizationOption currentOption;
  private int currentIndex;
  public Material eyeMaterial;

  public void SetButton(CustomizationOption option, int index)
  {
    if ((Object) option != (Object) null)
    {
      this.gameObject.SetActive(true);
      if (option.IsLocked && !this.manager.testUnlockAll)
      {
        this.lockedIcon.gameObject.SetActive(true);
        this.icon.gameObject.SetActive(false);
      }
      else
      {
        this.lockedIcon.gameObject.SetActive(false);
        this.icon.gameObject.SetActive(true);
        this.icon.texture = option.texture;
        if (option.type == Customization.Type.Skin)
          this.icon.color = option.color;
        else
          this.icon.color = Color.white;
        if (option.type == Customization.Type.Eyes)
          this.icon.material = this.eyeMaterial;
        else
          this.icon.material = (Material) null;
      }
    }
    else
      this.gameObject.SetActive(false);
    this.currentOption = option;
    this.currentIndex = index;
  }

  public void Click()
  {
    if (this.currentOption.IsLocked && !this.manager.testUnlockAll)
      return;
    this.manager.SetOption(this.currentOption, this.currentIndex);
  }
}
