// Decompiled with JetBrains decompiler
// Type: SettingsUICell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

#nullable disable
public class SettingsUICell : MonoBehaviour
{
  public Transform m_settingsContentParent;
  public TextMeshProUGUI m_text;
  public LocalizedText localizedText;
  public GameObject onlyOnMainMenu;
  private bool m_fadeIn;
  private CanvasGroup m_canvasGroup;
  public SFX_Instance fadeInSFX;
  private bool disable;

  public void Setup<T>(T setting) where T : Setting
  {
    this.m_canvasGroup = this.GetComponent<CanvasGroup>();
    this.m_canvasGroup.alpha = 0.0f;
    if (setting is IExposedSetting exposedSetting)
      this.localizedText.SetIndex(exposedSetting.GetDisplayName());
    SettingInputUICell component = Object.Instantiate<GameObject>(setting.GetSettingUICell(), this.m_settingsContentParent).GetComponent<SettingInputUICell>();
    component.disable = this.disable;
    if (this.disable)
      this.onlyOnMainMenu.SetActive(true);
    component.Setup((Setting) setting, (ISettingHandler) GameHandler.Instance.SettingsHandler);
  }

  public void FadeIn()
  {
    this.m_fadeIn = true;
    if (!(bool) (Object) this.fadeInSFX)
      return;
    this.fadeInSFX.Play();
  }

  private void Update()
  {
    if (!this.m_fadeIn)
      return;
    this.m_canvasGroup.alpha = Mathf.Lerp(this.m_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 10f);
  }

  public GameObject GetSelectable()
  {
    Button componentInChildren1 = this.m_settingsContentParent.GetComponentInChildren<Button>();
    if ((Object) componentInChildren1 != (Object) null)
      return componentInChildren1.gameObject;
    Slider componentInChildren2 = this.m_settingsContentParent.GetComponentInChildren<Slider>();
    if ((Object) componentInChildren2 != (Object) null)
      return componentInChildren2.gameObject;
    TMP_Dropdown componentInChildren3 = this.m_settingsContentParent.GetComponentInChildren<TMP_Dropdown>();
    return (Object) componentInChildren3 != (Object) null ? componentInChildren3.gameObject : (GameObject) null;
  }

  public void ShouldntShow() => this.disable = true;
}
