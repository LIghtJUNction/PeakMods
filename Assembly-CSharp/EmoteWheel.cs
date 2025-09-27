// Decompiled with JetBrains decompiler
// Type: EmoteWheel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
public class EmoteWheel : UIWheel
{
  public EmoteWheelSlice[] slices;
  public EmoteWheelData[] data;
  public TextMeshProUGUI selectedEmoteName;
  private EmoteWheelData chosenEmoteData;

  public void OnEnable() => this.InitWheel();

  public void OnDisable() => this.Choose();

  public void InitWheel()
  {
    this.chosenEmoteData = (EmoteWheelData) null;
    for (int index = 0; index < this.slices.Length; ++index)
      this.slices[index].Init(this.data[index], this);
    this.selectedEmoteName.text = "";
  }

  public void Choose()
  {
    if (!((Object) this.chosenEmoteData != (Object) null))
      return;
    Character.localCharacter.refs.animations.PlayEmote(this.chosenEmoteData.anim);
  }

  public void Hover(EmoteWheelData emoteWheelData)
  {
    this.selectedEmoteName.text = LocalizedText.GetText(emoteWheelData.emoteName);
    this.chosenEmoteData = emoteWheelData;
  }

  public void Dehover(EmoteWheelData emoteWheelData)
  {
    if (!((Object) this.chosenEmoteData == (Object) emoteWheelData))
      return;
    this.selectedEmoteName.text = "";
    this.chosenEmoteData = (EmoteWheelData) null;
  }

  protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
  {
    float num1 = 0.0f;
    EmoteWheelSlice emoteWheelSlice = (EmoteWheelSlice) null;
    if ((double) gamepadVector.sqrMagnitude >= 0.5)
    {
      for (int index = 0; index < this.slices.Length; ++index)
      {
        float num2 = Vector3.Angle((Vector3) gamepadVector, this.slices[index].GetUpVector());
        if ((Object) emoteWheelSlice == (Object) null || (double) num2 < (double) num1)
        {
          emoteWheelSlice = this.slices[index];
          num1 = num2;
        }
      }
    }
    if ((Object) emoteWheelSlice != (Object) null)
    {
      EventSystem.current.SetSelectedGameObject(emoteWheelSlice.button.gameObject);
      emoteWheelSlice.Hover();
    }
    else
    {
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      this.Dehover(this.chosenEmoteData);
    }
  }
}
