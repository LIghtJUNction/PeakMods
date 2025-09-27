// Decompiled with JetBrains decompiler
// Type: BugPhobia
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Settings;

#nullable disable
public class BugPhobia : MonoBehaviour
{
  public GameObject[] defaultGameObjects;
  public GameObject[] bugPhobiaGameObjects;
  private BugPhobiaSetting setting;
  public BingBongAudioSwitch bbas;

  private void Start()
  {
    this.setting = GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>();
    if (this.setting == null)
      return;
    for (int index = 0; index < this.bugPhobiaGameObjects.Length; ++index)
      this.bugPhobiaGameObjects[index].SetActive(this.setting.Value == OffOnMode.ON);
    for (int index = 0; index < this.defaultGameObjects.Length; ++index)
      this.defaultGameObjects[index].SetActive(this.setting.Value != OffOnMode.ON);
    if (!(bool) (Object) this.bbas)
      return;
    this.bbas.Init();
  }
}
