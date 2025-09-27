// Decompiled with JetBrains decompiler
// Type: FPSCapSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

#nullable disable
public class FPSCapSetting : FloatSetting, IExposedSetting
{
  public override void ApplyValue() => Application.targetFrameRate = Mathf.RoundToInt(this.Value);

  public string GetDisplayName() => "Max Framerate";

  public string GetCategory() => "Graphics";

  protected override float GetDefaultValue() => 400f;

  protected override float2 GetMinMaxValue() => new float2(30f, 600f);
}
