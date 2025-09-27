// Decompiled with JetBrains decompiler
// Type: DisableIfPhotosensitive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Settings;

#nullable disable
public class DisableIfPhotosensitive : MonoBehaviour
{
  public GameObject objectToDisable;
  public GameObject objectToReplace;

  private void Start()
  {
    if (GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>().Value != OffOnMode.ON)
      return;
    this.objectToDisable.SetActive(false);
    if (!(bool) (Object) this.objectToReplace)
      return;
    this.objectToReplace.SetActive(true);
  }
}
