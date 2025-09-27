// Decompiled with JetBrains decompiler
// Type: BoardingPass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class BoardingPass : MenuWindow
{
  public TMP_Text playerName;
  public TMP_Text ascentTitle;
  public TMP_Text ascentDesc;
  public GameObject reward;
  public Image rewardImage;
  public TextMeshProUGUI rewardText;
  public Image[] players;
  private int _ascentIndex;
  private int maxAscent = 7;
  private int maxUnlockedAscent;
  public AirportCheckInKiosk kiosk;
  public Button incrementAscentButton;
  public Button decrementAscentButton;
  public Button startGameButton;
  public Button closeButton;
  public AscentData ascentData;
  public CanvasGroup canvasGroup;

  public override bool openOnStart => false;

  public override bool selectOnOpen => true;

  public override bool closeOnPause => true;

  public override bool closeOnUICancel => true;

  public override bool autoHideOnClose => false;

  public int ascentIndex
  {
    get => this._ascentIndex;
    set => this._ascentIndex = value;
  }

  public override Selectable objectToSelectOnOpen
  {
    get
    {
      if (this.decrementAscentButton.gameObject.activeInHierarchy)
        return (Selectable) this.decrementAscentButton;
      return this.incrementAscentButton.gameObject.activeInHierarchy ? (Selectable) this.incrementAscentButton : (Selectable) this.startGameButton;
    }
  }

  protected override void Initialize()
  {
    this.incrementAscentButton.onClick.AddListener(new UnityAction(this.IncrementAscent));
    this.decrementAscentButton.onClick.AddListener(new UnityAction(this.DecrementAscent));
    this.startGameButton.onClick.AddListener(new UnityAction(this.StartGame));
    this.closeButton.onClick.AddListener(new UnityAction(((MenuWindow) this).Close));
    this.UpdateAscent();
  }

  private void InitMaxAscent()
  {
    this.maxUnlockedAscent = 0;
    Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out this.maxUnlockedAscent);
  }

  protected override void OnOpen()
  {
    this.playerName.text = Character.localCharacter.characterName;
    List<Character> allCharacters = Character.AllCharacters;
    for (int index = 0; index < this.players.Length; ++index)
    {
      if (allCharacters.Count > index)
      {
        this.players[index].gameObject.SetActive(true);
        this.players[index].color = allCharacters[index].refs.customization.PlayerColor;
      }
      else
        this.players[index].gameObject.SetActive(false);
    }
    this.canvasGroup.alpha = 0.0f;
    this.canvasGroup.DOFade(1f, 0.5f);
    this.UpdateAscent();
  }

  protected override void OnClose()
  {
    this.canvasGroup.DOFade(0.0f, 0.2f);
    this.Invoke("HideIt", 0.2f);
  }

  private void HideIt() => this.Hide();

  private void UpdateAscent()
  {
    this.maxUnlockedAscent = Singleton<AchievementManager>.Instance.GetMaxAscent();
    this.incrementAscentButton.interactable = this.ascentIndex < Mathf.Min(this.maxAscent, this.maxUnlockedAscent);
    this.decrementAscentButton.interactable = this.ascentIndex > -1;
    this.ascentTitle.text = this.ascentData.ascents[this.ascentIndex + 1].localizedTitle;
    this.ascentDesc.text = this.ascentData.ascents[this.ascentIndex + 1].localizedDescription;
    if (this.ascentIndex >= 2)
    {
      TMP_Text ascentDesc = this.ascentDesc;
      ascentDesc.text = $"{ascentDesc.text}\n\n<alpha=#CC><size=70%>{LocalizedText.GetText("ANDALLOTHER")}";
    }
    if (this.ascentIndex == this.maxUnlockedAscent && this.ascentIndex > -1 && this.ascentIndex < 8)
    {
      this.reward.gameObject.SetActive(true);
      this.rewardText.text = this.ascentData.ascents[this.ascentIndex + 1].localizedReward;
      this.rewardImage.color = this.ascentData.ascents[this.ascentIndex + 1].color;
    }
    else
      this.reward.gameObject.SetActive(false);
  }

  public void IncrementAscent()
  {
    ++this.ascentIndex;
    this.UpdateAscent();
  }

  public void DecrementAscent()
  {
    --this.ascentIndex;
    this.UpdateAscent();
  }

  public void StartGame() => this.kiosk.StartGame(this.ascentIndex);
}
