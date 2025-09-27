// Decompiled with JetBrains decompiler
// Type: EndScreenScoutWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class EndScreenScoutWindow : MonoBehaviour
{
  public TMP_Text scoutName;
  public TMP_Text altitude;
  public float panelAlpha = 0.25f;
  public Image panel;

  public void Init(Character character)
  {
    if ((Object) character != (Object) null)
    {
      if (character.IsLocal)
        this.scoutName.fontStyle = FontStyles.Underline;
      this.scoutName.text = character.characterName;
      this.panel.color = character.refs.customization.PlayerColor with
      {
        a = this.panelAlpha
      };
      this.altitude.text = "0m";
    }
    else
      this.gameObject.SetActive(false);
  }

  public void UpdateAltitude(int m) => this.altitude.text = m.ToString() + nameof (m);
}
