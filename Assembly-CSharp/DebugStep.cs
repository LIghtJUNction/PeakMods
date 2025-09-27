// Decompiled with JetBrains decompiler
// Type: DebugStep
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DebugStep : MonoBehaviour
{
  public DebugStep.StepType stepType;

  private void FixedUpdate()
  {
    if (this.stepType != DebugStep.StepType.FixedUpdate)
      return;
    Debug.Break();
  }

  private void Update()
  {
    if (this.stepType != DebugStep.StepType.Update)
      return;
    Debug.Break();
  }

  public enum StepType
  {
    Update,
    FixedUpdate,
  }
}
