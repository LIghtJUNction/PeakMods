// Decompiled with JetBrains decompiler
// Type: EndScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class EndScreen : MenuWindow
{
  public static EndScreen instance;
  public CanvasGroup canvasGroup;
  public AscentData ascentData;
  public bool debug;
  public TMP_Text endTime;
  public EndScreenScoutWindow[] scoutWindows;
  public Color[] debugColors;
  public BadgeData[] debugBadgeUnlocks;
  public BadgeUI badge;
  public Transform badgeParentTF;
  public Transform[] scoutLines;
  public Image[] scouts;
  public Image[] scoutsAtPeak;
  public int pipCount = 100;
  public float waitTime = 5f;
  public RectTransform timelinePanel;
  public Image pip;
  public Image deadPip;
  public Image passedOutPip;
  public Image revivedPip;
  public Material passedOutMaterial;
  public GameObject peakBanner;
  public GameObject deadBanner;
  public GameObject yourFriendsWonBanner;
  public GameObject buttons;
  public WaitingForPlayersUI WaitingForPlayersUI;
  public Button nextButton;
  public Button returnToAirportButton;
  public Material eyesMaterial;
  private bool selectedBadge;
  public GameObject cosmeticUnlockObject;
  public Animator cosmeticUnlockAnimator;
  public TMP_Text cosmeticUnlockTitle;
  public Button cosmeticNextButton;
  public RawImage cosmeticUnlockIcon;
  public GameObject ascentsUnlockObject;
  public Animator ascentsUnlockAnimator;
  public Button ascentsNextButton;
  public GameObject promotionUnlockObject;
  public Animator promotionUnlockAnimator;
  public TMP_Text promotionUnlockTitle;
  public TMP_Text promotionNextAscentUnlockText;
  public Button promotionNextButton;
  public Image promotionUnlockIcon;
  private bool inPopupView;
  private Image[] oldPip = new Image[4];

  public override bool openOnStart => false;

  public override Selectable objectToSelectOnOpen => (Selectable) null;

  public override bool selectOnOpen => false;

  private void Awake() => EndScreen.instance = this;

  protected override void Start()
  {
    base.Start();
    this.StartCoroutine(this.EndSequenceRoutine());
  }

  protected override void Initialize()
  {
    this.nextButton.onClick.AddListener(new UnityAction(this.Next));
    this.cosmeticNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
    this.ascentsNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
    this.promotionNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
  }

  private void Next()
  {
    this.WaitingForPlayersUI.gameObject.SetActive(true);
    Singleton<GameOverHandler>.Instance.LocalPlayerHasClosedEndScreen();
  }

  private IEnumerator EndSequenceRoutine()
  {
    EndScreen endScreen = this;
    UIInputHandler.SetSelectedObject((GameObject) null);
    endScreen.canvasGroup.alpha = 0.0f;
    endScreen.canvasGroup.DOFade(1f, 1f);
    List<Character> allCharacters = Character.AllCharacters;
    for (int index = 0; index < endScreen.scoutWindows.Length; ++index)
    {
      if (index < allCharacters.Count)
      {
        endScreen.scoutWindows[index].gameObject.SetActive(true);
        endScreen.scoutWindows[index].Init(allCharacters[index]);
      }
      else
        endScreen.scoutWindows[index].gameObject.SetActive(false);
    }
    endScreen.endTime.gameObject.SetActive(false);
    endScreen.buttons.SetActive(false);
    endScreen.peakBanner.SetActive(Character.localCharacter.refs.stats.won);
    endScreen.yourFriendsWonBanner.SetActive(!Character.localCharacter.refs.stats.won && Character.localCharacter.refs.stats.somebodyElseWon);
    endScreen.deadBanner.SetActive(!Character.localCharacter.refs.stats.won && !Character.localCharacter.refs.stats.somebodyElseWon);
    endScreen.cosmeticUnlockObject.SetActive(false);
    yield return (object) new WaitForSeconds(2f);
    try
    {
      endScreen.endTime.text = endScreen.GetTimeString(RunManager.Instance.timeSinceRunStarted);
      endScreen.endTime.gameObject.SetActive(true);
    }
    catch (Exception ex)
    {
      Console.WriteLine((object) ex);
    }
    if (Character.localCharacter.refs.stats.won)
      Singleton<AchievementManager>.Instance.TestTimeAchievements();
    yield return (object) new WaitForSeconds(1f);
    yield return (object) endScreen.StartCoroutine(endScreen.TimelineRoutine(allCharacters));
    yield return (object) new WaitForSeconds(0.25f);
    List<int> completedAscentsThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.completedAscentsThisRun;
    yield return (object) endScreen.StartCoroutine(endScreen.AscentRoutine(completedAscentsThisRun));
    yield return (object) new WaitForSeconds(0.25f);
    endScreen.selectedBadge = false;
    yield return (object) endScreen.StartCoroutine(endScreen.BadgeRoutine());
    endScreen.buttons.SetActive(true);
    if (!endScreen.selectedBadge)
      UIInputHandler.SetSelectedObject(endScreen.returnToAirportButton.gameObject);
  }

  private string GetTimeString(float totalSeconds)
  {
    int num = Mathf.FloorToInt(totalSeconds);
    return $"{num / 3600}:{num % 3600 / 60:00}:{num % 60:00}";
  }

  private IEnumerator TimelineRoutine(List<Character> allCharacters)
  {
    for (int index = 0; index < this.scouts.Length; ++index)
    {
      this.scouts[index].gameObject.SetActive(false);
      this.scoutsAtPeak[index].gameObject.SetActive(false);
    }
    if (this.debug)
    {
      for (int index = 0; index < this.scouts.Length; ++index)
      {
        this.scouts[index].color = this.debugColors[index];
        this.scoutsAtPeak[index].color = this.debugColors[index];
      }
    }
    else
    {
      for (int index = 0; index < allCharacters.Count; ++index)
      {
        if (index < this.scouts.Length)
        {
          Color playerColor = allCharacters[index].refs.customization.PlayerColor with
          {
            a = 1f
          };
          this.scouts[index].color = playerColor;
          this.scoutsAtPeak[index].color = this.scouts[index].color;
        }
      }
    }
    yield return (object) new WaitForSeconds(0.1f);
    List<List<EndScreen.TimelineInfo>> timelineInfos = new List<List<EndScreen.TimelineInfo>>();
    if (this.debug)
    {
      timelineInfos.Add(new List<EndScreen.TimelineInfo>());
      timelineInfos.Add(new List<EndScreen.TimelineInfo>());
      timelineInfos.Add(new List<EndScreen.TimelineInfo>());
      timelineInfos.Add(new List<EndScreen.TimelineInfo>());
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      float num4 = 0.0f;
      int num5 = UnityEngine.Random.Range(10, this.pipCount - 10);
      for (int index = 0; index < this.pipCount; ++index)
      {
        float nudge = (float) index / ((float) this.pipCount - 1f);
        EndScreen.TimelineInfo timelineInfo1 = new EndScreen.TimelineInfo();
        num1 += this.GetRandom(nudge) * 0.15f * nudge;
        timelineInfo1.height = Mathf.Clamp01(nudge + num1);
        timelineInfo1.time = nudge;
        timelineInfos[0].Add(timelineInfo1);
        EndScreen.TimelineInfo timelineInfo2 = new EndScreen.TimelineInfo();
        num2 += this.GetRandom(nudge) * 0.15f * nudge;
        timelineInfo2.height = Mathf.Clamp01(nudge + num2);
        timelineInfo2.time = nudge;
        timelineInfos[1].Add(timelineInfo2);
        EndScreen.TimelineInfo timelineInfo3 = new EndScreen.TimelineInfo();
        num3 += this.GetRandom(nudge) * 0.15f * nudge;
        timelineInfo3.height = Mathf.Clamp01(nudge + num3);
        timelineInfo3.time = nudge;
        timelineInfos[2].Add(timelineInfo3);
        EndScreen.TimelineInfo timelineInfo4 = new EndScreen.TimelineInfo();
        num4 += this.GetRandom(nudge) * 0.15f * nudge;
        timelineInfo4.height = Mathf.Clamp01(nudge + num4);
        timelineInfo4.time = nudge;
        if (index == num5)
          timelineInfo4.died = true;
        if (index > num5)
          timelineInfo4.dead = true;
        timelineInfos[3].Add(timelineInfo4);
      }
    }
    else
    {
      for (int index = 0; index < allCharacters.Count; ++index)
      {
        if ((UnityEngine.Object) allCharacters[index] != (UnityEngine.Object) null)
          timelineInfos.Add(allCharacters[index].refs.stats.timelineInfo);
      }
    }
    for (int index = 0; index < timelineInfos.Count; ++index)
    {
      if (index < this.scouts.Length)
        this.scouts[index].gameObject.SetActive(true);
    }
    int longestCount = 1;
    for (int index = 0; index < timelineInfos.Count; ++index)
    {
      if (timelineInfos[index].Count > longestCount)
        longestCount = timelineInfos[index].Count;
    }
    float startTime = 100000f;
    float maxTime = 0.0f;
    maxTime = Character.localCharacter.refs.stats.GetFinalTimelineInfo().time;
    startTime = Character.localCharacter.refs.stats.GetFirstTimelineInfo().time;
    maxTime -= startTime;
    if ((double) maxTime == 0.0)
      maxTime = 1f;
    float yieldTime = Mathf.Min(this.waitTime * Time.deltaTime / (float) longestCount, 0.2f);
    int j;
    for (j = 0; j < longestCount; ++j)
    {
      for (int index = 0; index < timelineInfos.Count; ++index)
      {
        if (j < timelineInfos[index].Count)
        {
          this.DrawPip(index, timelineInfos[index][j], maxTime, startTime, this.scouts[index].color);
          if (!timelineInfos[index][j].dead && !timelineInfos[index][j].died)
            this.scoutWindows[index].UpdateAltitude(CharacterStats.UnitsToMeters(timelineInfos[index][j].height));
        }
      }
      yield return (object) new WaitForSeconds(yieldTime * 0.33f);
    }
    for (j = 0; j < timelineInfos.Count; ++j)
    {
      Debug.Log((object) $"Checking timeline info {j}, has infos: {timelineInfos[j].Count}");
      if (timelineInfos[j].Count > 0)
      {
        this.CheckPeak(j, timelineInfos[j][timelineInfos[j].Count - 1]);
        yield return (object) new WaitForSeconds(0.25f);
      }
    }
  }

  private List<BadgeData> GetBadgeUnlocks()
  {
    List<BadgeData> badgeUnlocks = new List<BadgeData>();
    foreach (ACHIEVEMENTTYPE achievementType in Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun)
    {
      BadgeData badgeData = GUIManager.instance.mainBadgeManager.GetBadgeData(achievementType);
      if ((UnityEngine.Object) badgeData != (UnityEngine.Object) null)
        badgeUnlocks.Add(badgeData);
    }
    return badgeUnlocks;
  }

  private IEnumerator AscentRoutine(List<int> completedAscentsThisRun)
  {
    if (completedAscentsThisRun.Count > 0 && completedAscentsThisRun[0] == 0)
      yield return (object) this.AscentsUnlockRoutine();
    for (int i = 0; i < completedAscentsThisRun.Count; ++i)
    {
      yield return (object) new WaitForSeconds(0.5f);
      yield return (object) this.PromotionUnlockRoutine(completedAscentsThisRun[i]);
    }
    yield return (object) null;
  }

  private IEnumerator BadgeRoutine()
  {
    EndScreen endScreen = this;
    BadgeManager bm = endScreen.GetComponent<BadgeManager>();
    bm.InheritData(GUIManager.instance.mainBadgeManager);
    List<BadgeData> badgeUnlocks = endScreen.GetBadgeUnlocks();
    List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun;
    bool flag = false;
    bool unlockedCrown = false;
    for (int index = 0; index < achievementsEarnedThisRun.Count; ++index)
    {
      if (achievementsEarnedThisRun[index] >= ACHIEVEMENTTYPE.TriedYourBestBadge && achievementsEarnedThisRun[index] <= ACHIEVEMENTTYPE.EnduranceBadge)
        flag = true;
    }
    if (flag && Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked())
      unlockedCrown = true;
    for (int i = 0; i < badgeUnlocks.Count; ++i)
    {
      BadgeUI newBadge = UnityEngine.Object.Instantiate<BadgeUI>(endScreen.badge, endScreen.badgeParentTF);
      newBadge.manager = bm;
      newBadge.Init(badgeUnlocks[i]);
      newBadge.canvasGroup.DOFade(1f, 0.2f);
      newBadge.transform.localScale = Vector3.one * 1.5f;
      newBadge.transform.DOScale(1f, 0.25f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutBack);
      CustomizationOption cosmetic;
      if (Singleton<Customization>.Instance.TryGetUnlockedCosmetic(badgeUnlocks[i], out cosmetic))
      {
        yield return (object) new WaitForSeconds(0.5f);
        yield return (object) endScreen.CosmeticUnlockRoutine(cosmetic);
      }
      if (i == 0)
      {
        UIInputHandler.SetSelectedObject(newBadge.gameObject);
        endScreen.selectedBadge = true;
      }
      yield return (object) new WaitForSeconds(0.5f);
      newBadge = (BadgeUI) null;
      cosmetic = (CustomizationOption) null;
    }
    if (unlockedCrown)
    {
      yield return (object) endScreen.CosmeticUnlockRoutine(Singleton<Customization>.Instance.crownHat);
      yield return (object) new WaitForSeconds(0.5f);
    }
  }

  public void PopupNext() => this.inPopupView = false;

  private IEnumerator CosmeticUnlockRoutine(CustomizationOption cosmetic)
  {
    this.cosmeticUnlockObject.SetActive(true);
    string text = LocalizedText.GetText("NEWHAT");
    if (cosmetic.type == Customization.Type.Accessory || cosmetic.type == Customization.Type.Eyes)
      text = LocalizedText.GetText("NEWLOOK");
    if (cosmetic.type == Customization.Type.Fit)
      text = LocalizedText.GetText("NEWFIT");
    this.cosmeticUnlockTitle.text = text;
    this.cosmeticUnlockIcon.texture = cosmetic.texture;
    Shadow component = this.cosmeticUnlockIcon.GetComponent<Shadow>();
    if ((bool) (UnityEngine.Object) component)
      component.enabled = cosmetic.type == Customization.Type.Eyes;
    this.cosmeticUnlockIcon.material = cosmetic.type == Customization.Type.Eyes ? this.eyesMaterial : (Material) null;
    this.inPopupView = true;
    while (this.inPopupView)
    {
      UIInputHandler.SetSelectedObject(this.cosmeticNextButton.gameObject);
      yield return (object) null;
    }
    this.cosmeticUnlockAnimator.Play("Done", 0, 0.0f);
    yield return (object) new WaitForSeconds(0.25f);
    this.cosmeticUnlockObject.SetActive(false);
  }

  private IEnumerator AscentsUnlockRoutine()
  {
    this.ascentsUnlockObject.SetActive(true);
    this.inPopupView = true;
    while (this.inPopupView)
    {
      UIInputHandler.SetSelectedObject(this.ascentsNextButton.gameObject);
      yield return (object) null;
    }
    this.ascentsUnlockAnimator.Play("Done", 0, 0.0f);
    yield return (object) new WaitForSeconds(0.25f);
    this.ascentsUnlockObject.SetActive(false);
  }

  private IEnumerator PromotionUnlockRoutine(int ascent)
  {
    this.promotionUnlockObject.SetActive(true);
    this.promotionUnlockTitle.text = this.ascentData.ascents[ascent + 1].localizedReward;
    this.promotionNextAscentUnlockText.text = ascent >= this.ascentData.ascents.Count - 2 ? "" : LocalizedText.GetText("UNLOCKED").Replace("#", this.ascentData.ascents[ascent + 2].localizedTitle);
    this.promotionUnlockIcon.sprite = this.ascentData.ascents[ascent + 1].sashSprite;
    this.inPopupView = true;
    while (this.inPopupView)
    {
      UIInputHandler.SetSelectedObject(this.promotionNextButton.gameObject);
      yield return (object) null;
    }
    this.promotionUnlockAnimator.Play("Done", 0, 0.0f);
    yield return (object) new WaitForSeconds(0.25f);
    this.promotionUnlockObject.SetActive(false);
    if (ascent + 1 == 8)
      yield return (object) this.CosmeticUnlockRoutine(Singleton<Customization>.Instance.goatHat);
  }

  private float GetRandom(float nudge) => UnityEngine.Random.Range(nudge - 1f, 0.0f + nudge);

  public void DrawPip(
    int playerIndex,
    EndScreen.TimelineInfo heightTime,
    float maxTime,
    float startTime,
    Color color)
  {
    if (heightTime.dead)
      return;
    Image image = UnityEngine.Object.Instantiate<Image>(heightTime.revived ? this.revivedPip : (heightTime.justPassedOut ? this.passedOutPip : (heightTime.died ? this.deadPip : this.pip)), this.scoutLines[playerIndex]);
    image.color = color;
    image.transform.GetChild(0).GetComponent<Image>().color = image.color;
    float num = CharacterStats.peakHeightInUnits;
    if (this.debug)
      num = 1f;
    image.transform.localPosition = new Vector3(this.timelinePanel.sizeDelta.x * Mathf.Clamp01((heightTime.time - startTime) / maxTime), this.timelinePanel.sizeDelta.y * heightTime.height / num, 0.0f);
    image.transform.localPosition += Vector3.up * (float) playerIndex * 2f;
    this.scouts[playerIndex].transform.localPosition = image.transform.localPosition;
    if ((bool) (UnityEngine.Object) this.oldPip[playerIndex])
    {
      image.transform.right = this.oldPip[playerIndex].transform.position - image.transform.position;
      image.rectTransform.sizeDelta = new Vector2(Vector3.Distance(image.transform.position, this.oldPip[playerIndex].transform.position) / this.timelinePanel.lossyScale.x, 1.5f);
    }
    if (heightTime.died)
    {
      this.scouts[playerIndex].gameObject.SetActive(false);
      image.transform.GetChild(2).GetComponent<Image>().color = image.color;
      image.transform.GetChild(2).transform.rotation = Quaternion.identity;
    }
    if (heightTime.justPassedOut)
    {
      image.transform.GetChild(2).GetComponent<Image>().color = image.color;
      image.transform.GetChild(2).transform.rotation = Quaternion.identity;
    }
    else if (heightTime.passedOut)
      image.transform.GetChild(0).GetComponent<Image>().material = this.passedOutMaterial;
    if (heightTime.revived)
    {
      image.transform.GetChild(2).GetComponent<Image>().color = image.color;
      image.transform.GetChild(2).transform.rotation = Quaternion.identity;
      image.transform.GetChild(0).gameObject.SetActive(false);
      this.scouts[playerIndex].gameObject.SetActive(true);
    }
    this.oldPip[playerIndex] = image;
  }

  public void CheckPeak(int playerIndex, EndScreen.TimelineInfo timelineInfo)
  {
    if (playerIndex >= this.scouts.Length || (double) timelineInfo.time < 0.99000000953674316 || (double) timelineInfo.height < 1.0 || this.scoutsAtPeak[playerIndex].gameObject.activeSelf || timelineInfo.dead || !timelineInfo.won)
      return;
    this.scouts[playerIndex].gameObject.SetActive(false);
    this.scoutsAtPeak[playerIndex].gameObject.SetActive(true);
    this.scoutsAtPeak[playerIndex].transform.SetSiblingIndex(1);
    this.scoutsAtPeak[playerIndex].rectTransform.sizeDelta = (Vector2) Vector3.zero;
    this.scoutsAtPeak[playerIndex].rectTransform.DOSizeDelta((Vector2) (Vector3.one * 15f), 0.25f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutBack);
  }

  public void ReturnToAirport()
  {
    RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true));
  }

  public struct TimelineInfo
  {
    public float height;
    public float time;
    public bool died;
    public bool dead;
    public bool revived;
    public bool justPassedOut;
    public bool passedOut;
    public bool won;
  }
}
