// Decompiled with JetBrains decompiler
// Type: UI_Interaction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class UI_Interaction : MonoBehaviour
{
  private TextMeshProUGUI text;
  private IInteractible current;

  private void Start() => this.text = this.GetComponentInChildren<TextMeshProUGUI>();

  private void Update() => this.OnChange();

  private void OnChange()
  {
    this.current = Interaction.instance.currentHovered;
    if (this.current != null)
      this.text.text = this.current.GetInteractionText();
    else
      this.text.text = "";
  }
}
