// Decompiled with JetBrains decompiler
// Type: BadgeManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

#nullable disable
public class BadgeManager : MonoBehaviour
{
  private BadgeUI _selectedBadge;
  public GameObject badgePopup;
  public Animator badgePopupAnim;
  public TextMeshProUGUI badgePopupName;
  public TextMeshProUGUI badgePopupDescription;
  public BadgeData[] badgeData;
  private BadgeUI[] badges;
  public bool initBadgesOnEnable;

  public BadgeUI selectedBadge
  {
    get => this._selectedBadge;
    set
    {
      this._selectedBadge = value;
      if ((UnityEngine.Object) this._selectedBadge != (UnityEngine.Object) null && (UnityEngine.Object) this._selectedBadge.data != (UnityEngine.Object) null)
      {
        if (this._selectedBadge.data.IsLocked)
        {
          this.badgePopupName.text = "???";
          this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName));
        }
        else
        {
          this.badgePopupName.text = LocalizedText.GetText(LocalizedText.GetNameIndex(this._selectedBadge.data.displayName));
          this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName));
        }
      }
      else
      {
        this.badgePopupName.text = "???";
        this.badgePopupDescription.text = LocalizedText.GetText("BADGELOCKED");
      }
      this.badgePopupAnim.Play("Popup", 0, 0.0f);
    }
  }

  public void InheritData(BadgeManager other)
  {
    this.badgeData = new BadgeData[other.badgeData.Length];
    other.badgeData.CopyTo((Array) this.badgeData, 0);
  }

  private void OnEnable()
  {
    this.selectedBadge = (BadgeUI) null;
    if (!this.initBadgesOnEnable)
      return;
    this.InitBadges();
  }

  public BadgeData GetBadgeData(ACHIEVEMENTTYPE achievementType)
  {
    foreach (BadgeData badgeData in this.badgeData)
    {
      if (!((UnityEngine.Object) badgeData == (UnityEngine.Object) null) && badgeData.linkedAchievement == achievementType)
        return badgeData;
    }
    return (BadgeData) null;
  }

  private void InitBadges()
  {
    this.badges = this.GetComponentsInChildren<BadgeUI>();
    for (int index = 0; index < this.badges.Length; ++index)
    {
      if (index < this.badgeData.Length)
        this.badges[index].Init(this.badgeData[index]);
      else
        this.badges[index].Init((BadgeData) null);
    }
  }

  private void Update()
  {
    this.badgePopup.SetActive((UnityEngine.Object) this.selectedBadge != (UnityEngine.Object) null);
    if (!(bool) (UnityEngine.Object) this.selectedBadge)
      return;
    this.badgePopup.transform.position = this.selectedBadge.transform.position;
  }

  public void AddAllToCSV()
  {
    for (int index = 0; index < this.badgeData.Length; ++index)
      this.badgeData[index].AddToCSV();
  }
}
