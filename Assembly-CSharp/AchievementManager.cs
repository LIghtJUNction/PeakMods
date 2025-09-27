// Decompiled with JetBrains decompiler
// Type: AchievementManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[ConsoleClassCustomizer("Achievements")]
public class AchievementManager : Singleton<AchievementManager>
{
  public List<AchievementData> allAchievements;
  [SerializeField]
  private ACHIEVEMENTTYPE debugAchievement;
  public const int MAX_GUIDEBOOK_PAGES = 8;
  public const int DISC_THROW_DISTANCE_REQUIREMENT = 100;
  public const float MAX_MESA_HEAT_PERCENTAGE = 0.1f;
  public const float MAX_ALPINE_COLD_PERCENTAGE = 0.2f;
  private List<AchievementManager.RunBasedAchievementData> runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>();
  public SerializableRunBasedValues runBasedValueData;
  public bool useDebugAscent;
  public int debugAscent;
  public const int TOTAL_GUIDEBOOK_PAGES = 8;
  public const string STEAMSTAT_GUIDEBOOK_PREFIX = "ReadGuidebookPage_";
  private const float ONE_HOUR_IN_SECONDS = 3600f;
  private const int FRUITSNEEDEDFORACHIEVEMENT = 5;
  private const int MUSHROOMSNEEDEDFORACHIEVEMENT = 4;
  private ACHIEVEMENTTYPE[] ascentAchievements = new ACHIEVEMENTTYPE[7]
  {
    ACHIEVEMENTTYPE.Ascent1,
    ACHIEVEMENTTYPE.Ascent2,
    ACHIEVEMENTTYPE.Ascent3,
    ACHIEVEMENTTYPE.Ascent4,
    ACHIEVEMENTTYPE.Ascent5,
    ACHIEVEMENTTYPE.Ascent6,
    ACHIEVEMENTTYPE.Ascent7
  };

  public void DebugGetAchievement() => this.ThrowAchievement(this.debugAchievement);

  public void DebugGetAllAchievements()
  {
    foreach (ACHIEVEMENTTYPE type in Enum.GetValues(typeof (ACHIEVEMENTTYPE)))
      this.ThrowAchievement(type);
  }

  public bool gotStats { get; private set; }

  public void InitRunBasedValues()
  {
    this.runBasedValueData = new SerializableRunBasedValues();
    this.runBasedValueData.PrimeExistingAchievements();
    this.runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>()
    {
      new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.KnotTyingBadge, RUNBASEDVALUETYPE.RopePlaced, 100),
      new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.ClutchBadge, RUNBASEDVALUETYPE.ScoutsResurrected, 3),
      new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.PlundererBadge, RUNBASEDVALUETYPE.LuggageOpened, 15),
      new AchievementManager.RunBasedAchievementData(ACHIEVEMENTTYPE.FirstAidBadge, RUNBASEDVALUETYPE.FriendsHealedAmount, 20)
    };
  }

  [ConsoleCommand]
  public static void ClearAchievements()
  {
    Singleton<AchievementManager>.Instance.ResetAllUserStats();
  }

  [ContextMenu("RESET ALL DATA")]
  private void ResetAllUserStats()
  {
    SteamUserStats.ResetAllStats(true);
    this.StoreUserStats();
    this.InitRunBasedValues();
  }

  private void Start()
  {
    this.StartCoroutine(this.GetUserStatsRoutine());
    this.InitRunBasedValues();
    this.SubscribeToEvents();
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    this.UnsubscribeFromEvents();
  }

  private IEnumerator GetUserStatsRoutine()
  {
    while ((UnityEngine.Object) SteamManager.Instance == (UnityEngine.Object) null)
    {
      Debug.Log((object) "Waiting for steam manager");
      yield return (object) null;
    }
    while (!SteamManager.Initialized)
      yield return (object) null;
    while (!this.gotStats)
    {
      SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
      yield return (object) new WaitForSeconds(2f);
    }
  }

  private void StoreUserStats() => this.StartCoroutine(this.StoreUserStatsRoutine());

  private IEnumerator StoreUserStatsRoutine()
  {
    while (!SteamManager.Initialized)
      yield return (object) null;
    SteamUserStats.StoreStats();
  }

  public int GetMaxAscent()
  {
    if (this.useDebugAscent)
      return this.debugAscent;
    int num;
    return Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num) ? num : 0;
  }

  public bool AllBaseAchievementsUnlocked()
  {
    for (int index = 1; index <= 32 /*0x20*/; ++index)
    {
      if (!Singleton<AchievementManager>.Instance.IsAchievementUnlocked((ACHIEVEMENTTYPE) index))
        return false;
    }
    return true;
  }

  public bool IsAchievementUnlocked(ACHIEVEMENTTYPE achievementType)
  {
    if (!SteamManager.Initialized)
      return false;
    bool pbAchieved;
    SteamUserStats.GetAchievement(achievementType.ToString(), out pbAchieved);
    return pbAchieved;
  }

  private void CheckRunBasedAchievement(RUNBASEDVALUETYPE type)
  {
    foreach (AchievementManager.RunBasedAchievementData basedAchievement in this.runBasedAchievements)
    {
      if (basedAchievement.valueType == type && basedAchievement.IsAchieved())
        this.ThrowAchievement(basedAchievement.achievementType);
    }
  }

  private void CheckNewAchievements()
  {
    foreach (ACHIEVEMENTTYPE achievementtype in Enum.GetValues(typeof (ACHIEVEMENTTYPE)))
    {
      bool pbAchieved;
      if (!this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Contains(achievementtype) && SteamUserStats.GetAchievement(achievementtype.ToString(), out pbAchieved) && pbAchieved)
      {
        Debug.Log((object) ("EARNED ACHIEVEMENT: " + achievementtype.ToString()));
        if (!this.runBasedValueData.achievementsEarnedThisRun.Contains(achievementtype))
          this.runBasedValueData.achievementsEarnedThisRun.Add(achievementtype);
        this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
      }
    }
  }

  public void SetSteamStat(STEAMSTATTYPE steamStatType, int value)
  {
    if (!SteamManager.Initialized)
      return;
    SteamUserStats.SetStat(steamStatType.ToString(), value);
    this.StoreUserStats();
    this.CheckNewAchievements();
  }

  public bool GetSteamStatInt(STEAMSTATTYPE steamStatType, out int value)
  {
    if (SteamManager.Initialized)
      return SteamUserStats.GetStat(steamStatType.ToString(), out value);
    value = -1;
    return false;
  }

  public int GetNextPage()
  {
    if (!SteamManager.Initialized)
      return 0;
    int nextPage = 0;
    for (int index = 0; index < 8; ++index)
    {
      int pData;
      SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), out pData);
      if (pData != 1)
        return nextPage;
      ++nextPage;
    }
    return nextPage;
  }

  public int GetTotalPagesSeen()
  {
    if (!SteamManager.Initialized)
      return 0;
    int totalPagesSeen = 0;
    for (int index = 0; index < 8; ++index)
    {
      int pData;
      SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), out pData);
      if (pData == 1)
        ++totalPagesSeen;
    }
    return totalPagesSeen;
  }

  public bool SeenGuidebookPage(int index)
  {
    int pData;
    SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), out pData);
    return pData == 1;
  }

  public void TriggerSeenGuidebookPage(int index)
  {
    SteamUserStats.SetStat("ReadGuidebookPage_" + index.ToString(), 1);
    this.StoreUserStats();
    int totalPagesSeen = this.GetTotalPagesSeen();
    Debug.Log((object) ("Total pages seen: " + totalPagesSeen.ToString()));
    this.SetSteamStat(STEAMSTATTYPE.TotalPagesRead, totalPagesSeen);
    if (totalPagesSeen >= 8)
      this.ThrowAchievement(ACHIEVEMENTTYPE.BookwormBadge);
    this.StoreUserStats();
    Debug.Log((object) ("Saw page " + index.ToString()));
  }

  public void IncrementSteamStat(STEAMSTATTYPE steamStatType, int value)
  {
    try
    {
      if (!SteamManager.Initialized)
        return;
      int pData;
      SteamUserStats.GetStat(steamStatType.ToString(), out pData);
      int nData = pData + value;
      SteamUserStats.SetStat(steamStatType.ToString(), nData);
      this.StoreUserStats();
      this.CheckNewAchievements();
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
  }

  [ConsoleCommand]
  internal static void Grant(ACHIEVEMENTTYPE type)
  {
    Singleton<AchievementManager>.Instance.ThrowAchievement(type);
  }

  [ConsoleCommand]
  internal static void GrantAscentLevel(int level)
  {
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, level);
  }

  internal void ThrowAchievement(ACHIEVEMENTTYPE type)
  {
    try
    {
      if (!SteamManager.Initialized)
        return;
      bool pbAchieved;
      SteamUserStats.GetAchievement(type.ToString(), out pbAchieved);
      if (!pbAchieved && !this.runBasedValueData.achievementsEarnedThisRun.Contains(type))
      {
        this.runBasedValueData.achievementsEarnedThisRun.Add(type);
        Debug.Log((object) ("Throwing achievement: " + type.ToString()));
        GlobalEvents.OnAchievementThrown(type);
        SteamUserStats.SetAchievement(type.ToString());
      }
      this.StoreUserStats();
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
  }

  public void SetRunBasedInt(RUNBASEDVALUETYPE type, int value)
  {
    this.runBasedValueData.runBasedInts[type] = value;
    this.CheckRunBasedAchievement(type);
  }

  public int GetRunBasedInt(RUNBASEDVALUETYPE type)
  {
    if (!this.runBasedValueData.runBasedInts.ContainsKey(type))
      this.SetRunBasedInt(type, 0);
    try
    {
      return this.runBasedValueData.runBasedInts[type];
    }
    catch
    {
      Debug.LogError((object) $"Tried to retrieve run based int {type} that is not an int.");
    }
    return 0;
  }

  public void AddToRunBasedInt(RUNBASEDVALUETYPE type, int valueToAdd)
  {
    int runBasedInt = this.GetRunBasedInt(type);
    this.SetRunBasedInt(type, runBasedInt + valueToAdd);
  }

  public void SetRunBasedFloat(RUNBASEDVALUETYPE type, float value)
  {
    this.runBasedValueData.runBasedFloats[type] = value;
    this.CheckRunBasedAchievement(type);
  }

  public float GetRunBasedFloat(RUNBASEDVALUETYPE type)
  {
    if (!this.runBasedValueData.runBasedFloats.ContainsKey(type))
      this.SetRunBasedFloat(type, 0.0f);
    try
    {
      return Convert.ToSingle(this.runBasedValueData.runBasedFloats[type]);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Tried to retrieve run based float {type} that is not a float.\n{ex.ToString()}");
    }
    return 0.0f;
  }

  public void AddToRunBasedFloat(RUNBASEDVALUETYPE type, float valueToAdd)
  {
    float runBasedFloat = this.GetRunBasedFloat(type);
    this.SetRunBasedFloat(type, runBasedFloat + valueToAdd);
  }

  private void SubscribeToEvents()
  {
    GlobalEvents.OnItemRequested += new Action<Item, Character>(this.TestRequestedItem);
    GlobalEvents.OnItemConsumed += new Action<Item, Character>(this.TestItemConsumed);
    GlobalEvents.OnRespawnChestOpened += new Action<RespawnChest, Character>(this.TestRespawnChestOpened);
    GlobalEvents.OnLuggageOpened += new Action<Luggage, Character>(this.TestLuggageOpened);
    GlobalEvents.OnLocalCharacterWonRun += new Action(this.TestWonRun);
    GlobalEvents.OnCharacterPassedOut += new Action<Character>(this.TestCharacterPassedOut);
    GlobalEvents.OnSomeoneWonRun += new Action(this.TestSomeoneWonRun);
    GlobalEvents.OnRunEnded += new Action(this.TestRunEnded);
    Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsRecieved));
  }

  private void UnsubscribeFromEvents()
  {
    GlobalEvents.OnItemRequested -= new Action<Item, Character>(this.TestRequestedItem);
    GlobalEvents.OnItemConsumed -= new Action<Item, Character>(this.TestItemConsumed);
    GlobalEvents.OnRespawnChestOpened -= new Action<RespawnChest, Character>(this.TestRespawnChestOpened);
    GlobalEvents.OnLuggageOpened -= new Action<Luggage, Character>(this.TestLuggageOpened);
    GlobalEvents.OnLocalCharacterWonRun -= new Action(this.TestWonRun);
    GlobalEvents.OnCharacterPassedOut -= new Action<Character>(this.TestCharacterPassedOut);
    GlobalEvents.OnSomeoneWonRun -= new Action(this.TestSomeoneWonRun);
    GlobalEvents.OnRunEnded -= new Action(this.TestRunEnded);
  }

  private void OnUserStatsRecieved(UserStatsReceived_t result)
  {
    if (result.m_eResult == EResult.k_EResultFail)
      return;
    this.gotStats = true;
  }

  private void TestRequestedItem(Item item, Character character)
  {
    if (!character.IsLocal || !item.itemTags.HasFlag((Enum) Item.ItemTags.Mystical))
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.EsotericaBadge);
  }

  private void TestItemConsumed(Item item, Character character)
  {
    if (!character.IsLocal)
      return;
    if (item.itemTags.HasFlag((Enum) Item.ItemTags.Berry))
      this.AddToRunBasedFruitsEaten(item.itemID);
    if (item.itemTags.HasFlag((Enum) Item.ItemTags.Bird) && !Character.localCharacter.data.cannibalismPermitted)
      this.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
    if (item.itemTags.HasFlag((Enum) Item.ItemTags.PackagedFood))
      this.AddToRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten, 1);
    if (item.itemTags.HasFlag((Enum) Item.ItemTags.Mushroom) && (UnityEngine.Object) item.GetComponent<Action_InflictPoison>() == (UnityEngine.Object) null)
      this.AddToNonToxicMushroomsEaten(item.itemID);
    if (!item.itemTags.HasFlag((Enum) Item.ItemTags.GourmandRequirement) || item.GetData<IntItemData>(DataEntryKey.CookedAmount).Value < 1)
      return;
    this.AddToGourmandRequirementsEaten(item.itemID);
  }

  private void TestRespawnChestOpened(RespawnChest chest, Character opener)
  {
    if (!opener.IsLocal)
      return;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (allCharacter.data.dead || allCharacter.data.fullyPassedOut)
        this.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
    }
  }

  private void TestLuggageOpened(Luggage luggage, Character opener)
  {
    if (!opener.IsLocal)
      return;
    this.AddToRunBasedInt(RUNBASEDVALUETYPE.LuggageOpened, 1);
  }

  public void TestTimeAchievements()
  {
    int num1 = Mathf.FloorToInt(RunManager.Instance.timeSinceRunStarted);
    if ((double) num1 <= 3600.0)
    {
      Debug.Log((object) "Sub one hour!!");
      this.ThrowAchievement(ACHIEVEMENTTYPE.SpeedClimberBadge);
    }
    int num2;
    this.GetSteamStatInt(STEAMSTATTYPE.BestTime, out num2);
    if (num1 >= num2)
      return;
    this.SetSteamStat(STEAMSTATTYPE.BestTime, num1);
  }

  private void TestRunEnded() => this.TestAscentAchievements();

  public void TestCoolCucumberAchievement()
  {
    if ((double) this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa) > 0.10000000149011612)
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.CoolCucumberBadge);
  }

  public void TestBundledUpAchievement()
  {
    if ((double) this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine) > 0.20000000298023224)
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.BundledUpBadge);
  }

  private void TestWonRun()
  {
    this.ThrowAchievement(ACHIEVEMENTTYPE.PeakBadge);
    this.IncrementSteamStat(STEAMSTATTYPE.TimesPeaked, 1);
    if (Character.AllCharacters.Count == 1)
      this.ThrowAchievement(ACHIEVEMENTTYPE.LoneWolfBadge);
    if ((double) this.GetRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken) == 0.0)
      this.ThrowAchievement(ACHIEVEMENTTYPE.BalloonBadge);
    if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten) == 0)
      this.ThrowAchievement(ACHIEVEMENTTYPE.NaturalistBadge);
    if (this.GetRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut) == 0)
      this.ThrowAchievement(ACHIEVEMENTTYPE.SurvivalistBadge);
    if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced) == 0)
      this.ThrowAchievement(ACHIEVEMENTTYPE.LeaveNoTraceBadge);
    if (this.HasBingBong(Character.localCharacter))
      GameUtils.instance.ThrowBingBongAchievement();
    if (this.runBasedValueData.gourmandRequirementsEaten.Count < 4)
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.GourmandBadge);
  }

  private bool HasBingBong(Character character)
  {
    if ((bool) (UnityEngine.Object) character.data.currentItem && character.data.currentItem.itemTags.HasFlag((Enum) Item.ItemTags.BingBong))
      return true;
    foreach (ItemSlot itemSlot in character.player.itemSlots)
    {
      if (itemSlot != null && (UnityEngine.Object) itemSlot.prefab != (UnityEngine.Object) null && itemSlot.prefab.itemTags.HasFlag((Enum) Item.ItemTags.BingBong))
        return true;
    }
    BackpackData backpackData;
    if (!character.player.backpackSlot.IsEmpty() && character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
    {
      foreach (ItemSlot itemSlot in backpackData.itemSlots)
      {
        if (itemSlot != null && (UnityEngine.Object) itemSlot.prefab != (UnityEngine.Object) null && itemSlot.prefab.itemTags.HasFlag((Enum) Item.ItemTags.BingBong))
          return true;
      }
    }
    return false;
  }

  private void TestCharacterPassedOut(Character character)
  {
    if (!character.IsLocal)
      return;
    this.AddToRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut, 1);
  }

  private void TestSomeoneWonRun()
  {
    if (Character.localCharacter.refs.stats.lost)
    {
      Debug.Log((object) "YOU TRIED");
      this.ThrowAchievement(ACHIEVEMENTTYPE.TriedYourBestBadge);
    }
    this.TryCompleteAscent();
  }

  private void TryCompleteAscent()
  {
    int maxAscent;
    if (!this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out maxAscent))
      return;
    if (Ascents.currentAscent >= maxAscent)
    {
      for (; maxAscent <= Ascents.currentAscent; ++maxAscent)
      {
        Debug.Log((object) ("Completed Ascent: " + maxAscent.ToString()));
        if (!this.runBasedValueData.completedAscentsThisRun.Contains(maxAscent))
          this.runBasedValueData.completedAscentsThisRun.Add(maxAscent);
      }
      this.SetSteamStat(STEAMSTATTYPE.MaxAscent, Ascents.currentAscent + 1);
    }
    this.TestAscentAchievements(maxAscent);
  }

  private void AddToRunBasedFruitsEaten(ushort value)
  {
    if (this.runBasedValueData.runBasedFruitsEaten.Contains(value))
      return;
    this.runBasedValueData.runBasedFruitsEaten.Add(value);
    if (this.runBasedValueData.runBasedFruitsEaten.Count < 5)
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.ForagingBadge);
  }

  private void AddToNonToxicMushroomsEaten(ushort value)
  {
    Debug.Log((object) ("Local player ate non toxic mushroom: " + value.ToString()));
    if (this.runBasedValueData.nonToxicMushroomsEaten.Contains(value))
      return;
    this.runBasedValueData.nonToxicMushroomsEaten.Add(value);
    if (this.runBasedValueData.nonToxicMushroomsEaten.Count < 4)
      return;
    this.ThrowAchievement(ACHIEVEMENTTYPE.MycologyBadge);
  }

  private void AddToGourmandRequirementsEaten(ushort value)
  {
    if (this.runBasedValueData.gourmandRequirementsEaten.Contains(value))
      return;
    Debug.Log((object) ("ATE GOURMAND REQUIREMENT: " + value.ToString()));
    this.runBasedValueData.gourmandRequirementsEaten.Add(value);
  }

  internal void RecordMaxHeight(int meters)
  {
    if (meters < 25)
      return;
    int runBasedInt = this.GetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached);
    if (meters < runBasedInt + 5)
      return;
    this.IncrementSteamStat(STEAMSTATTYPE.HeightClimbed, meters - runBasedInt);
    this.SetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached, meters);
  }

  internal void TestAscentAchievements(int maxAscent = -1)
  {
    if (maxAscent == -1)
      this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out maxAscent);
    for (int index = 0; index <= maxAscent - 2 && index < this.ascentAchievements.Length; ++index)
      this.ThrowAchievement(this.ascentAchievements[index]);
  }

  private class RunBasedAchievementData
  {
    public ACHIEVEMENTTYPE achievementType;
    public RUNBASEDVALUETYPE valueType;
    public int requiredValue;

    public RunBasedAchievementData(
      ACHIEVEMENTTYPE achievementType,
      RUNBASEDVALUETYPE valueType,
      int requiredValue)
    {
      this.achievementType = achievementType;
      this.valueType = valueType;
      this.requiredValue = requiredValue;
    }

    public bool IsAchieved()
    {
      try
      {
        bool flag = false;
        if ((double) Singleton<AchievementManager>.Instance.GetRunBasedFloat(this.valueType) >= (double) this.requiredValue)
          flag = true;
        if ((double) Singleton<AchievementManager>.Instance.GetRunBasedInt(this.valueType) >= (double) this.requiredValue)
          flag = true;
        return flag;
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
      return false;
    }
  }
}
