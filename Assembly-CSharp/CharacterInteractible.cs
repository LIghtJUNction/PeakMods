// Decompiled with JetBrains decompiler
// Type: CharacterInteractible
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterInteractible : MonoBehaviour, IInteractibleConstant, IInteractible
{
  public Character character;
  private CannibalismSetting localCannibalismSetting;

  private void Start()
  {
    this.character = this.GetComponent<Character>();
    this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
  }

  public Vector3 Center() => this.character.Center;

  public string GetInteractionText()
  {
    if (this.CarriedByLocalCharacter())
      return LocalizedText.GetText("DROP").Replace("#", this.GetName());
    if (this.IsCannibal())
      return LocalizedText.GetText("EAT");
    return this.CanBeCarried() ? LocalizedText.GetText("CARRY").Replace("#", this.GetName()) : "";
  }

  private bool IsCannibal() => this.character.refs.customization.isCannibalizable;

  public string GetSecondaryInteractionText()
  {
    return this.HasItemCanUseOnFriend() ? this.GetItemPrompt(Character.localCharacter.data.currentItem) : "";
  }

  public string GetItemPrompt(Item item)
  {
    return LocalizedText.GetText(item.UIData.secondaryInteractPrompt).Replace("#targetchar", this.GetName());
  }

  public string GetName() => this.character.characterName;

  private bool CarriedByLocalCharacter()
  {
    return (bool) (Object) this.character.data.carrier && (Object) this.character.data.carrier == (Object) Character.localCharacter;
  }

  private bool CanBeCarried()
  {
    return this.character.data.fullyPassedOut && !this.character.data.dead && !(bool) (Object) this.character.data.carrier;
  }

  private bool HasItemCanUseOnFriend()
  {
    return !this.character.data.dead && (Object) this.character != (Object) Character.localCharacter && (bool) (Object) Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.canUseOnFriend;
  }

  public Transform GetTransform() => this.character.GetBodypart(BodypartType.Torso).transform;

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }

  public void Interact(Character interactor)
  {
    if (this.CarriedByLocalCharacter())
    {
      interactor.refs.carriying.Drop(this.character);
    }
    else
    {
      if (this.IsCannibal() || !this.CanBeCarried())
        return;
      interactor.refs.carriying.StartCarry(this.character);
    }
  }

  public bool IsInteractible(Character interactor)
  {
    return this.IsPrimaryInteractible(interactor) || this.IsSecondaryInteractible(interactor);
  }

  public bool IsPrimaryInteractible(Character interactor)
  {
    return this.character.refs.customization.isCannibalizable || this.CarriedByLocalCharacter() || this.CanBeCarried();
  }

  public bool IsSecondaryInteractible(Character interactor)
  {
    return this.HasItemCanUseOnFriend() && (this.character.data.fullyPassedOut || (double) Vector3.Angle(HelperFunctions.ZeroY(this.character.data.lookDirection), -HelperFunctions.ZeroY(interactor.data.lookDirection)) <= (double) Interaction.instance.maxCharacterInteractAngle);
  }

  private void GetEaten(Character eater)
  {
    if (!eater.IsLocal)
      return;
    this.character.DieInstantly();
    eater.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f);
    eater.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.1f);
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
  }

  public bool IsConstantlyInteractable(Character interactor)
  {
    return this.character.refs.customization.isCannibalizable;
  }

  public float GetInteractTime(Character interactor) => 3f;

  public void Interact_CastFinished(Character interactor)
  {
    if (!interactor.IsLocal || !this.character.refs.customization.isCannibalizable)
      return;
    this.GetEaten(interactor);
  }

  public void CancelCast(Character interactor)
  {
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public bool holdOnFinish { get; }
}
