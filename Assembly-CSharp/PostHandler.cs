// Decompiled with JetBrains decompiler
// Type: PostHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using HorizonBasedAmbientOcclusion.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Settings;

#nullable disable
public class PostHandler : MonoBehaviour
{
  public AOSetting AOSetting;
  public Volume volume;

  private void Start()
  {
    this.AOSetting = GameHandler.Instance.SettingsHandler.GetSetting<AOSetting>();
  }

  private void LateUpdate()
  {
    HBAO component;
    if (!this.volume.sharedProfile.TryGet<HBAO>(out component))
      return;
    component.active = this.AOSetting.Value == OffOnMode.ON;
  }
}
