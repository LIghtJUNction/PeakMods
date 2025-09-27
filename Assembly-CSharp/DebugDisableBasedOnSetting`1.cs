// Decompiled with JetBrains decompiler
// Type: DebugDisableBasedOnSetting`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Settings;

#nullable disable
public class DebugDisableBasedOnSetting<T> : MonoBehaviour where T : OffOnSetting
{
  public GameObject target;
  private T settings;

  private void Update()
  {
    if ((object) this.settings == null && (Object) GameHandler.Instance != (Object) null)
      this.settings = GameHandler.Instance.SettingsHandler.GetSetting<T>();
    if ((object) this.settings == null)
      return;
    this.target.SetActive(this.settings.Value == OffOnMode.OFF);
  }
}
