// Decompiled with JetBrains decompiler
// Type: AudioLevelSlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class AudioLevelSlider : MonoBehaviour
{
  public TextMeshProUGUI playerName;
  public TextMeshProUGUI percent;
  public Photon.Realtime.Player player;
  public Slider slider;
  public Image bar;
  public Gradient barGradient;
  public Sprite[] audioSprites;
  public Sprite mutedAudioSprite;
  public Image icon;

  private void Update()
  {
    Photon.Realtime.Player player = this.player;
  }

  private void Awake()
  {
    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
  }

  public void Init(Photon.Realtime.Player newPlayer)
  {
    this.player = newPlayer;
    Photon.Realtime.Player player = this.player;
    bool flag = this.player != null && !this.player.IsLocal;
    this.gameObject.SetActive(flag);
    if (flag)
    {
      this.playerName.text = this.player.NickName;
      this.slider.SetValueWithoutNotify(AudioLevels.GetPlayerLevel(this.player.ActorNumber));
    }
    this.bar.color = this.barGradient.Evaluate(this.slider.value);
    this.percent.text = Mathf.RoundToInt(this.slider.value * 200f).ToString() + "%";
  }

  private void OnSliderChanged(float newValue)
  {
    if (this.player == null)
      return;
    AudioLevels.SetPlayerLevel(this.player.ActorNumber, newValue);
    this.icon.sprite = (double) newValue == 0.0 ? this.mutedAudioSprite : this.audioSprites[Mathf.FloorToInt(newValue * 2.99f)];
    this.bar.color = this.barGradient.Evaluate(newValue);
    EventSystem.current.SetSelectedGameObject((GameObject) null);
    this.percent.text = Mathf.RoundToInt(newValue * 200f).ToString() + "%";
  }
}
