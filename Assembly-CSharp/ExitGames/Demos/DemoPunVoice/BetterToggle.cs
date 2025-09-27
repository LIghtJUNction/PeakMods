// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.BetterToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof (Toggle))]
[DisallowMultipleComponent]
public class BetterToggle : MonoBehaviour
{
  private Toggle toggle;

  public static event BetterToggle.OnToggle ToggleValueChanged;

  private void Start()
  {
    this.toggle = this.GetComponent<Toggle>();
    this.toggle.onValueChanged.AddListener((UnityAction<bool>) (_param1 => this.OnToggleValueChanged()));
  }

  public void OnToggleValueChanged()
  {
    if (BetterToggle.ToggleValueChanged == null)
      return;
    BetterToggle.ToggleValueChanged(this.toggle);
  }

  public delegate void OnToggle(Toggle toggle);
}
