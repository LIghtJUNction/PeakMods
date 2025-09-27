// Decompiled with JetBrains decompiler
// Type: UIPlayerNames
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Settings;

#nullable disable
public class UIPlayerNames : MonoBehaviour
{
  private int indexCounter;
  public PlayerName[] playerNameText;
  public Sprite[] audioSprites;
  public Sprite mutedAudioSprite;
  public CannibalismSetting localCannibalismSetting;
  public float audioImageTimeoutMax = 1f;
  public static float CANNIBAL_HUNGER_THRESHOLD = 0.7f;

  public int Init(CharacterInteractible characterInteractable)
  {
    ++this.indexCounter;
    this.playerNameText[this.indexCounter - 1].characterInteractable = characterInteractable;
    this.playerNameText[this.indexCounter - 1].text.text = characterInteractable.GetName();
    for (int index = 0; index < this.playerNameText.Length; ++index)
      this.playerNameText[index].gameObject.SetActive(false);
    this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
    return this.indexCounter - 1;
  }

  public void UpdateName(int index, Vector3 position, bool visible, int speakingAmplitude)
  {
    if (!(bool) (Object) Character.localCharacter || index >= this.playerNameText.Length)
      return;
    this.playerNameText[index].transform.position = MainCamera.instance.cam.WorldToScreenPoint(position);
    if (visible)
    {
      if (this.CanCannibalize(this.playerNameText[index].characterInteractable.character))
        this.playerNameText[index].characterInteractable.character.refs.customization.BecomeChicken();
      this.playerNameText[index].gameObject.SetActive(true);
      this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 1f, Time.deltaTime * 5f);
      if ((bool) (Object) this.playerNameText[index].characterInteractable && (double) AudioLevels.GetPlayerLevel(this.playerNameText[index].characterInteractable.character.photonView.OwnerActorNr) == 0.0)
      {
        this.playerNameText[index].audioImage.sprite = this.mutedAudioSprite;
        return;
      }
      if (speakingAmplitude <= 0)
      {
        this.playerNameText[index].audioImageTimeout -= Time.deltaTime;
        if ((double) this.playerNameText[index].audioImageTimeout <= 0.0)
          this.playerNameText[index].audioImage.sprite = this.audioSprites[0];
      }
      else
      {
        this.playerNameText[index].audioImage.sprite = this.audioSprites[Mathf.Clamp(speakingAmplitude, 0, this.audioSprites.Length - 1)];
        this.playerNameText[index].audioImageTimeout = this.audioImageTimeoutMax;
      }
    }
    else
    {
      this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 0.0f, Time.deltaTime * 5f);
      if ((double) this.playerNameText[index].group.alpha < 0.0099999997764825821 && this.playerNameText[index].gameObject.activeSelf)
      {
        this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
        this.playerNameText[index].gameObject.SetActive(false);
      }
    }
    if (!Character.localCharacter.data.fullyPassedOut && (double) Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) >= (double) UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD || !this.playerNameText[index].gameObject.activeSelf)
      return;
    this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
  }

  private bool CanCannibalize(Character otherCharacter)
  {
    return !otherCharacter.refs.customization.isCannibalizable && (double) Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) >= (double) UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD && Character.localCharacter.data.fullyConscious && this.localCannibalismSetting.Value == OffOnMode.ON && otherCharacter.data.cannibalismPermitted;
  }

  public void DisableName(int index)
  {
    if (!(bool) (Object) this.playerNameText[index])
      return;
    this.playerNameText[index].gameObject.SetActive(false);
  }
}
