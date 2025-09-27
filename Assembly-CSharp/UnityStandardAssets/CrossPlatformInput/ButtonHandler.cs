// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CrossPlatformInput.ButtonHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CrossPlatformInput;

public class ButtonHandler : MonoBehaviour
{
  public string Name;

  private void OnEnable()
  {
  }

  public void SetDownState() => CrossPlatformInputManager.SetButtonDown(this.Name);

  public void SetUpState() => CrossPlatformInputManager.SetButtonUp(this.Name);

  public void SetAxisPositiveState() => CrossPlatformInputManager.SetAxisPositive(this.Name);

  public void SetAxisNeutralState() => CrossPlatformInputManager.SetAxisZero(this.Name);

  public void SetAxisNegativeState() => CrossPlatformInputManager.SetAxisNegative(this.Name);

  public void Update()
  {
  }
}
