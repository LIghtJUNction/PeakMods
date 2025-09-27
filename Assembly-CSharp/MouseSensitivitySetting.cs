// Decompiled with JetBrains decompiler
// Type: MouseSensitivitySetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using Zorro.Settings;

#nullable disable
public class MouseSensitivitySetting : FloatSetting, IExposedSetting
{
  public override void ApplyValue()
  {
  }

  protected override float GetDefaultValue() => 2f;

  protected override float2 GetMinMaxValue() => new float2(0.1f, 5f);

  public string GetDisplayName() => "Mouse Sensitivity";

  public string GetCategory() => "General";
}
