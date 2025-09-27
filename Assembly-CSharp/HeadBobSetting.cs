// Decompiled with JetBrains decompiler
// Type: HeadBobSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class HeadBobSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
  public override void ApplyValue()
  {
  }

  protected override OffOnMode GetDefaultValue() => OffOnMode.OFF;

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Reduce Camera Bobbing";

  public string GetCategory() => "Accessibility";

  public bool ShouldShow() => !PhotonNetwork.InRoom;
}
