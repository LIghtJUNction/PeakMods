// Decompiled with JetBrains decompiler
// Type: ExtraFovSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using Zorro.Settings;

#nullable disable
public class ExtraFovSetting : FloatSetting, IExposedSetting
{
  public override void ApplyValue()
  {
  }

  protected override float GetDefaultValue() => 40f;

  protected override float2 GetMinMaxValue() => new float2(0.0f, 50f);

  public string GetDisplayName() => "Climbing extra field of view";

  public string GetCategory() => "General";
}
