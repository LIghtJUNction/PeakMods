// Decompiled with JetBrains decompiler
// Type: DayNightProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "DayNightProfile", menuName = "Scriptable Objects/DayNightProfile")]
public class DayNightProfile : ScriptableObject
{
  public float sunIntensity;
  public float moonIntensity;
  public Gradient sunGradient;
  public Gradient skyTopGradient;
  public Gradient skyMidGradient;
  public Gradient skyBottomGradient;
  public Gradient fogGradient;
  public ShaderParameters[] globalShaderFloats;
  public AnimatedShaderParameters[] animatedGlobalShaderFloats;
}
