// Decompiled with JetBrains decompiler
// Type: SharedSettingsMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Settings;

#nullable disable
public class SharedSettingsMenu : MonoBehaviour
{
  [SerializeField]
  private SettingsTABS m_tabs;
  public GameObject m_settingsCellPrefab;
  public Transform m_settingsContentParent;
  private List<IExposedSetting> settings;
  private readonly List<SettingsUICell> m_spawnedCells = new List<SettingsUICell>();
  private Coroutine m_fadeInCoroutine;

  private void OnEnable()
  {
    this.RefreshSettings();
    if (!((UnityEngine.Object) this.m_tabs.selectedButton != (UnityEngine.Object) null))
      return;
    this.m_tabs.Select(this.m_tabs.selectedButton);
  }

  private void RefreshSettings()
  {
    if (!((UnityEngine.Object) GameHandler.Instance != (UnityEngine.Object) null))
      return;
    this.settings = GameHandler.Instance.SettingsHandler.GetSettingsThatImplements<IExposedSetting>();
  }

  public void ShowSettings(SettingsCategory category)
  {
    if (this.m_fadeInCoroutine != null)
    {
      this.StopCoroutine(this.m_fadeInCoroutine);
      this.m_fadeInCoroutine = (Coroutine) null;
    }
    foreach (Component spawnedCell in this.m_spawnedCells)
      UnityEngine.Object.Destroy((UnityEngine.Object) spawnedCell.gameObject);
    this.m_spawnedCells.Clear();
    this.RefreshSettings();
    foreach (IExposedSetting exposedSetting in this.settings.Where<IExposedSetting>((Func<IExposedSetting, bool>) (setting => setting.GetCategory() == category.ToString())).Where<IExposedSetting>((Func<IExposedSetting, bool>) (setting =>
    {
      IConditionalSetting conditionalSetting = setting as IConditionalSetting;
      return true;
    })))
    {
      SettingsUICell component = UnityEngine.Object.Instantiate<GameObject>(this.m_settingsCellPrefab, this.m_settingsContentParent).GetComponent<SettingsUICell>();
      if (exposedSetting is IConditionalSetting conditionalSetting && !conditionalSetting.ShouldShow())
        component.ShouldntShow();
      this.m_spawnedCells.Add(component);
      component.Setup<Setting>(exposedSetting as Setting);
    }
    this.m_fadeInCoroutine = this.StartCoroutine(this.FadeInCells());
  }

  private IEnumerator FadeInCells()
  {
    int i = 0;
    foreach (SettingsUICell spawnedCell in this.m_spawnedCells)
    {
      spawnedCell.FadeIn();
      yield return (object) new WaitForSecondsRealtime(0.05f);
      ++i;
    }
    this.m_fadeInCoroutine = (Coroutine) null;
  }

  public GameObject GetDefaultSelection()
  {
    foreach (SettingsUICell spawnedCell in this.m_spawnedCells)
    {
      if (spawnedCell.gameObject.activeSelf)
      {
        GameObject selectable = spawnedCell.GetSelectable();
        if ((bool) (UnityEngine.Object) selectable)
          return selectable;
      }
    }
    return (GameObject) null;
  }
}
