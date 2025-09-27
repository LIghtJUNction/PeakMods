// Decompiled with JetBrains decompiler
// Type: CharacterAfflictions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

#nullable disable
[ConsoleClassCustomizer("Afflictions")]
public class CharacterAfflictions : MonoBehaviourPunCallbacks
{
  private Dictionary<CharacterAfflictions.STATUSTYPE, float> statusCaps = new Dictionary<CharacterAfflictions.STATUSTYPE, float>()
  {
    {
      CharacterAfflictions.STATUSTYPE.Injury,
      1f
    }
  };
  [SerializeField]
  public float[] currentStatuses;
  private float[] currentIncrementalStatuses;
  private float[] currentDecrementalStatuses;
  private float[] lastAddedStatus;
  private float[] lastAddedIncrementalStatus;
  public float poisonReductionPerSecond;
  public float poisonReductionCooldown;
  public float drowsyReductionPerSecond;
  public float drowsyReductionCooldown;
  public float hotReductionPerSecond;
  public float hotReductionCooldown;
  public float hungerPerSecond = 0.0005f;
  public float thornsReductionPerSecond;
  public float thornsReductionCooldown;
  public float nightColdPerSecond = 1f / 500f;
  public Character character;
  [SerializeReference]
  public List<Affliction> afflictionList = new List<Affliction>();
  [FormerlySerializedAs("headVFX")]
  public Transform headVfxTransform;
  [ColorUsage(false, true)]
  public Color colorInjury;
  [ColorUsage(false, true)]
  public Color colorCold;
  [ColorUsage(false, true)]
  public Color colorCrab;
  [ColorUsage(false, true)]
  public Color colorPoison;
  [ColorUsage(false, true)]
  public Color colorCurse;
  [ColorUsage(false, true)]
  public Color colorDrowsy;
  [ColorUsage(false, true)]
  public Color colorHot;
  [ColorUsage(false, true)]
  public Color colorThorns;
  public SFX_Instance injurySmall;
  public SFX_Instance injuryMid;
  public SFX_Instance injuryHeavy;
  public SFX_Instance injuryIce;
  public SFX_Instance injuryFire;
  public SFX_Instance injuryPoison;
  public SFX_Instance injuryHunger;
  public SFX_Instance injuryThorns;
  public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedStatus;
  public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedIncrementalStatus;
  private bool m_inAirport;
  public List<ThornOnMe> physicalThorns;
  private List<ushort> availableThornIndices = new List<ushort>();
  public ThornSyncData thornData;
  private float lastAddedPoison;
  public const float STATUS_INCREMENT = 0.025f;
  public const float MAX_TOTAL_STATUS = 2f;
  public CharacterAfflictions.STATUSTYPE debugStatusType;

  private void GetThorns()
  {
    this.physicalThorns = ((IEnumerable<ThornOnMe>) this.GetComponentsInChildren<ThornOnMe>(true)).ToList<ThornOnMe>();
  }

  private void Awake()
  {
    this.character = this.GetComponent<Character>();
    this.InitStatusArrays();
    this.InitThorns();
    this.m_inAirport = SceneManager.GetActiveScene().name == "Airport";
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    this.PushStatuses(newPlayer);
    this.PushThorns(newPlayer);
  }

  private void InitStatusArrays()
  {
    this.currentStatuses = new float[Enum.GetNames(typeof (CharacterAfflictions.STATUSTYPE)).Length];
    this.currentIncrementalStatuses = new float[this.currentStatuses.Length];
    this.currentDecrementalStatuses = new float[this.currentStatuses.Length];
    this.lastAddedStatus = new float[this.currentStatuses.Length];
    this.lastAddedIncrementalStatus = new float[this.currentStatuses.Length];
  }

  private void InitThorns()
  {
    this.availableThornIndices.Clear();
    this.thornData = new ThornSyncData()
    {
      stuckThornIndices = new List<ushort>()
    };
    for (ushort index = 0; (int) index < this.physicalThorns.Count; ++index)
    {
      if (!((UnityEngine.Object) this.physicalThorns[(int) index] == (UnityEngine.Object) null))
      {
        this.physicalThorns[(int) index].character = this.character;
        this.availableThornIndices.Add(index);
        this.physicalThorns[(int) index].gameObject.SetActive(false);
      }
    }
  }

  private void Update()
  {
    if (!this.character.IsLocal)
      return;
    for (int index = this.afflictionList.Count - 1; index >= 0; --index)
    {
      if (this.afflictionList[index].Tick())
        this.character.refs.afflictions.RemoveAffliction(this.afflictionList[index]);
    }
    this.UpdateNormalStatuses();
    this.UpdateThorns();
  }

  private void UpdateThorns()
  {
    for (int index = this.physicalThorns.Count - 1; index >= 0; --index)
    {
      if ((UnityEngine.Object) this.physicalThorns[index] != (UnityEngine.Object) null && this.physicalThorns[index].ShouldPopOut())
        this.RemoveThorn(this.physicalThorns[index]);
    }
  }

  internal void UpdateWeight()
  {
    int num1 = 0;
    int num2 = 0;
    float currentStatus = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
    for (int index = 0; index < this.character.player.itemSlots.Length; ++index)
    {
      ItemSlot itemSlot = this.character.player.itemSlots[index];
      if ((UnityEngine.Object) itemSlot.prefab != (UnityEngine.Object) null)
        num1 += itemSlot.prefab.CarryWeight;
    }
    BackpackSlot backpackSlot = this.character.player.backpackSlot;
    BackpackData backpackData;
    if (!backpackSlot.IsEmpty() && backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
    {
      for (int index = 0; index < backpackData.itemSlots.Length; ++index)
      {
        ItemSlot itemSlot = backpackData.itemSlots[index];
        if (!itemSlot.IsEmpty())
          num1 += itemSlot.prefab.CarryWeight;
      }
    }
    ItemSlot itemSlot1 = this.character.player.GetItemSlot((byte) 250);
    if (!itemSlot1.IsEmpty())
      num1 += itemSlot1.prefab.CarryWeight;
    if ((UnityEngine.Object) this.character.data.carriedPlayer != (UnityEngine.Object) null)
      num1 += 8;
    foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
    {
      if ((UnityEngine.Object) stickyItemComponent.stuckToCharacter == (UnityEngine.Object) this.character)
      {
        num1 += stickyItemComponent.addWeightToStuckPlayer;
        num2 += stickyItemComponent.addThornsToStuckPlayer;
      }
    }
    if ((bool) (UnityEngine.Object) this.character.data.currentStickyItem)
      num2 += this.character.data.currentStickyItem.addThornsToStuckPlayer;
    int num3 = num2 + this.GetTotalThornStatusIncrements();
    float num4 = 0.025f * (float) num3;
    if ((double) num4 > (double) currentStatus)
    {
      this.StatusSFX(CharacterAfflictions.STATUSTYPE.Thorns, num4 - currentStatus);
      if (this.character.IsLocal && (UnityEngine.Object) this.character == (UnityEngine.Object) Character.observedCharacter)
        GUIManager.instance.AddStatusFX(CharacterAfflictions.STATUSTYPE.Thorns, num4 - currentStatus);
      this.PlayParticle(CharacterAfflictions.STATUSTYPE.Thorns);
    }
    this.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0.025f * (float) num1);
    this.SetStatus(CharacterAfflictions.STATUSTYPE.Thorns, 0.025f * (float) num3);
  }

  private void UpdateNormalStatuses()
  {
    if (!this.character.IsLocal)
      return;
    if (Ascents.isNightCold && (bool) (UnityEngine.Object) Singleton<MountainProgressHandler>.Instance && Singleton<MountainProgressHandler>.Instance.maxProgressPointReached < 3 && (UnityEngine.Object) DayNightManager.instance != (UnityEngine.Object) null && (double) DayNightManager.instance.isDay < 0.5)
      this.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, Time.deltaTime * (1f - DayNightManager.instance.isDay) * Ascents.nightColdRate);
    if (this.character.data.fullyConscious)
      this.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, Time.deltaTime * this.hungerPerSecond * Ascents.hungerRateMultiplier);
    if ((double) this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) > 0.0 && (double) Time.time - (double) this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Poison) > (double) this.poisonReductionCooldown)
      this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, this.poisonReductionPerSecond * Time.deltaTime);
    if ((double) this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > 0.0 && (double) Time.time - (double) this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > (double) this.drowsyReductionCooldown)
      this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.drowsyReductionPerSecond * Time.deltaTime);
    if ((double) this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) <= 0.0 || (double) Time.time - (double) this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Hot) <= (double) this.hotReductionCooldown)
      return;
    this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, this.hotReductionPerSecond * Time.deltaTime);
  }

  public void AddAffliction(Affliction affliction, bool fromRPC = false)
  {
    if (this.character.data.isCarried)
      return;
    if (affliction == null)
    {
      Debug.LogError((object) "Trying to add null affliction");
    }
    else
    {
      if (!this.character.IsLocal && !fromRPC)
        return;
      if (affliction == null)
      {
        Debug.LogError((object) "Tried to apply null affliction! This is probably a big problem!");
      }
      else
      {
        Affliction incomingAffliction = affliction.Copy();
        Affliction affliction1;
        if (this.HasAfflictionType((IEnumerable<Affliction>) this.afflictionList, incomingAffliction.GetAfflictionType(), out affliction1))
        {
          affliction1.Stack(incomingAffliction);
        }
        else
        {
          this.afflictionList.Add(incomingAffliction);
          incomingAffliction.character = this.character;
          incomingAffliction.OnApplied();
          Debug.Log((object) $"Added {incomingAffliction.GetAfflictionType()} to {this.character.gameObject.name}");
        }
        if (fromRPC || !this.character.IsLocal)
          return;
        this.PushAfflictions();
      }
    }
  }

  public void RemoveAffliction(Affliction affliction, bool fromRPC = false)
  {
    if (!this.character.IsLocal && !fromRPC)
      return;
    this.afflictionList.Remove(affliction);
    affliction.OnRemoved();
    Debug.Log((object) $"Removed {affliction.GetAfflictionType()} to {this.character.gameObject.name}");
    if (fromRPC || !this.character.IsLocal)
      return;
    this.PushAfflictions();
  }

  public void RemoveAffliction(Affliction.AfflictionType afflictionType)
  {
    Affliction affliction;
    if (!this.HasAfflictionType(afflictionType, out affliction))
      return;
    this.RemoveAffliction(affliction);
  }

  public float GetCurrentStatus(CharacterAfflictions.STATUSTYPE statusType)
  {
    int index = (int) statusType;
    return this.currentStatuses.WithinRange<float>(index) ? this.currentStatuses[index] : 0.0f;
  }

  public float GetIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
  {
    return this.currentIncrementalStatuses[(int) statusType];
  }

  public float LastAddedStatus(CharacterAfflictions.STATUSTYPE statusType)
  {
    return this.lastAddedStatus[(int) statusType];
  }

  public float LastAddedIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
  {
    return this.lastAddedIncrementalStatus[(int) statusType];
  }

  public float statusSum
  {
    get
    {
      float statusSum = 0.0f;
      for (int index = 0; index < this.currentStatuses.Length; ++index)
        statusSum += this.currentStatuses[index];
      return statusSum;
    }
  }

  public void SetStatus(CharacterAfflictions.STATUSTYPE statusType, float amount)
  {
    if (this.character.isBot || !this.character.IsLocal)
      return;
    int index = (int) statusType;
    Mathf.FloorToInt(amount / 0.025f);
    this.currentStatuses[index] = amount;
    this.currentStatuses[index] = Mathf.Clamp(this.currentStatuses[index], 0.0f, this.GetStatusCap(statusType));
    this.currentIncrementalStatuses[index] = 0.0f;
    this.currentDecrementalStatuses[index] = 0.0f;
    this.character.ClampStamina();
    GUIManager.instance.bar.ChangeBar();
    this.PushStatuses();
  }

  public void AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
  {
    if ((double) amount > 0.0)
    {
      this.AddStatus(statusType, amount, fromRPC);
    }
    else
    {
      if ((double) amount >= 0.0)
        return;
      this.SubtractStatus(statusType, Mathf.Abs(amount), fromRPC);
    }
  }

  public void AddSunHeat(float amount)
  {
    Parasol component;
    if ((bool) (UnityEngine.Object) this.character.data.currentItem && this.character.data.currentItem.TryGetComponent<Parasol>(out component) && component.isOpen || this.character.data.wearingSunscreen)
      return;
    this.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, amount);
  }

  public bool AddStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
  {
    if (this.character.isBot || this.character.statusesLocked || (double) amount == 0.0 || this.m_inAirport || this.character.data.isInvincible)
      return false;
    float b = 2f - this.statusSum;
    if (!this.character.IsLocal && !fromRPC)
      return false;
    float currentStatus1 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot);
    float currentStatus2 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
    if (statusType == CharacterAfflictions.STATUSTYPE.Cold && (double) currentStatus1 > 0.0)
    {
      this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, amount, fromRPC);
      amount -= currentStatus1;
      if ((double) amount <= 0.0)
        return false;
    }
    else if (statusType == CharacterAfflictions.STATUSTYPE.Hot && (double) currentStatus2 > 0.0)
    {
      this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, amount, fromRPC);
      amount -= currentStatus2;
      if ((double) amount <= 0.0)
        return false;
    }
    int index = (int) statusType;
    this.currentIncrementalStatuses[index] += amount;
    this.lastAddedIncrementalStatus[index] = Time.time;
    Action<CharacterAfflictions.STATUSTYPE, float> incrementalStatus = this.OnAddedIncrementalStatus;
    if (incrementalStatus != null)
      incrementalStatus(statusType, amount);
    if ((double) this.currentIncrementalStatuses[index] >= 0.02500000037252903)
    {
      float num = Mathf.Min((float) Mathf.FloorToInt(this.currentIncrementalStatuses[index] / 0.025f) * 0.025f, b);
      this.currentStatuses[index] += num;
      this.currentStatuses[index] = Mathf.Clamp(this.currentStatuses[index], 0.0f, this.GetStatusCap(statusType));
      Action<CharacterAfflictions.STATUSTYPE, float> onAddedStatus = this.OnAddedStatus;
      if (onAddedStatus != null)
        onAddedStatus(statusType, num);
      this.currentIncrementalStatuses[index] = 0.0f;
      this.character.ClampStamina();
      GUIManager.instance.bar.ChangeBar();
      this.StatusSFX(statusType, amount);
      if (this.character.IsLocal && (UnityEngine.Object) this.character == (UnityEngine.Object) Character.observedCharacter)
        GUIManager.instance.AddStatusFX(statusType, amount);
      this.PlayParticle(statusType);
      this.lastAddedStatus[index] = Time.time;
      this.PushStatuses();
    }
    if (statusType == CharacterAfflictions.STATUSTYPE.Hot && this.character.IsLocal && (bool) (UnityEngine.Object) Singleton<MapHandler>.Instance && Singleton<MapHandler>.Instance.GetCurrentBiome() == Biome.BiomeType.Mesa)
    {
      float currentStatus3 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot);
      if ((double) currentStatus3 > (double) Singleton<AchievementManager>.Instance.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa))
        Singleton<AchievementManager>.Instance.SetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa, currentStatus3);
    }
    if (statusType == CharacterAfflictions.STATUSTYPE.Cold && this.character.IsLocal && (bool) (UnityEngine.Object) Singleton<MapHandler>.Instance && Singleton<MapHandler>.Instance.GetCurrentBiome() == Biome.BiomeType.Alpine)
    {
      float currentStatus4 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
      if ((double) currentStatus4 > (double) Singleton<AchievementManager>.Instance.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine))
        Singleton<AchievementManager>.Instance.SetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine, currentStatus4);
    }
    return true;
  }

  public void SubtractStatus(
    CharacterAfflictions.STATUSTYPE statusType,
    float amount,
    bool fromRPC = false)
  {
    if (this.character.isBot || this.character.statusesLocked || !this.character.IsLocal && !fromRPC)
      return;
    int index = (int) statusType;
    if ((double) this.currentStatuses[index] == 0.0)
    {
      this.currentDecrementalStatuses[index] = 0.0f;
    }
    else
    {
      this.currentDecrementalStatuses[index] += amount;
      if ((double) this.currentDecrementalStatuses[index] < 0.02500000037252903)
        return;
      float num = (float) Mathf.FloorToInt(this.currentDecrementalStatuses[index] / 0.025f) * 0.025f;
      Debug.Log((object) $"Removing status chunk: {statusType}");
      this.currentStatuses[index] -= num;
      this.currentStatuses[index] = Mathf.Clamp(this.currentStatuses[index], 0.0f, this.GetStatusCap(statusType));
      if (statusType == CharacterAfflictions.STATUSTYPE.Hunger)
        this.currentIncrementalStatuses[index] = 0.0f;
      this.currentDecrementalStatuses[index] = 0.0f;
      this.character.ClampStamina();
      GUIManager.instance.bar.ChangeBar();
      this.PushStatuses();
    }
  }

  private void StatusSFX(CharacterAfflictions.STATUSTYPE sT, float ammount)
  {
    switch (sT)
    {
      case CharacterAfflictions.STATUSTYPE.Injury:
        if ((double) ammount > 0.0 && (bool) (UnityEngine.Object) this.injurySmall)
          this.injurySmall.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        if ((double) ammount > 0.40000000596046448 && (bool) (UnityEngine.Object) this.injuryMid)
          this.injuryMid.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        if ((double) ammount <= 0.75 || !(bool) (UnityEngine.Object) this.injuryHeavy)
          break;
        this.injuryHeavy.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
      case CharacterAfflictions.STATUSTYPE.Hunger:
        if (!(bool) (UnityEngine.Object) this.injuryHunger)
          break;
        this.injuryHunger.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
      case CharacterAfflictions.STATUSTYPE.Cold:
        if (!(bool) (UnityEngine.Object) this.injuryIce)
          break;
        this.injuryIce.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
      case CharacterAfflictions.STATUSTYPE.Poison:
        if (!(bool) (UnityEngine.Object) this.injuryPoison)
          break;
        this.injuryPoison.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
      case CharacterAfflictions.STATUSTYPE.Hot:
        if (!(bool) (UnityEngine.Object) this.injuryFire)
          break;
        this.injuryFire.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
      case CharacterAfflictions.STATUSTYPE.Thorns:
        if (!(bool) (UnityEngine.Object) this.injuryThorns)
          break;
        this.injuryThorns.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
        break;
    }
  }

  public void PlayDebugParticle() => this.PlayParticle(this.debugStatusType);

  public void PlayParticle(CharacterAfflictions.STATUSTYPE statusType)
  {
    switch (statusType)
    {
      case CharacterAfflictions.STATUSTYPE.Injury:
        this.character.refs.customization.PulseStatus(this.colorInjury);
        break;
      case CharacterAfflictions.STATUSTYPE.Cold:
        this.character.refs.customization.PulseStatus(this.colorCold);
        break;
      case CharacterAfflictions.STATUSTYPE.Poison:
        this.character.refs.customization.PulseStatus(this.colorPoison);
        break;
      case CharacterAfflictions.STATUSTYPE.Crab:
        this.character.refs.customization.PulseStatus(this.colorCrab);
        break;
      case CharacterAfflictions.STATUSTYPE.Curse:
        this.character.refs.customization.PulseStatus(this.colorCurse);
        break;
      case CharacterAfflictions.STATUSTYPE.Drowsy:
        this.character.refs.customization.PulseStatus(this.colorDrowsy);
        break;
      case CharacterAfflictions.STATUSTYPE.Hot:
        this.character.refs.customization.PulseStatus(this.colorHot);
        break;
      case CharacterAfflictions.STATUSTYPE.Thorns:
        this.character.refs.customization.PulseStatus(this.colorThorns);
        break;
    }
  }

  public void PushStatuses(Photon.Realtime.Player specificPlayer = null)
  {
    if (!this.character.IsLocal)
      return;
    byte[] managedArray = IBinarySerializable.ToManagedArray<StatusSyncData>(new StatusSyncData()
    {
      statusList = new List<float>((IEnumerable<float>) this.currentStatuses)
    });
    if (specificPlayer == null)
      this.character.photonView.RPC("SyncStatusesRPC", RpcTarget.Others, (object) managedArray);
    else
      this.character.photonView.RPC("SyncStatusesRPC", specificPlayer, (object) managedArray);
  }

  [PunRPC]
  private void SyncStatusesRPC(byte[] data)
  {
    if (this.character.IsLocal)
      return;
    this.ApplyStatusesFromFloatArrayRPC(IBinarySerializable.GetFromManagedArray<StatusSyncData>(data).statusList.ToArray());
  }

  public void PushThorns(Photon.Realtime.Player specificPlayer = null)
  {
    if (!this.character.IsLocal)
      return;
    byte[] managedArray = IBinarySerializable.ToManagedArray<ThornSyncData>(this.thornData);
    if (specificPlayer == null)
      this.character.photonView.RPC("SyncThornsRPC_Remote", RpcTarget.Others, (object) managedArray);
    else
      this.character.photonView.RPC("SyncThornsRPC_Remote", specificPlayer, (object) managedArray);
  }

  [PunRPC]
  private void SyncThornsRPC_Remote(byte[] data)
  {
    if (this.photonView.IsMine)
      return;
    List<ushort> stuckThornIndices = IBinarySerializable.GetFromManagedArray<ThornSyncData>(data).stuckThornIndices;
    for (ushort index = 0; (int) index < this.physicalThorns.Count; ++index)
    {
      if (stuckThornIndices.Contains(index))
        this.physicalThorns[(int) index].EnableThorn();
      else
        this.physicalThorns[(int) index].DisableThorn();
    }
  }

  [PunRPC]
  public void ApplyStatusesFromFloatArrayRPC(float[] deserializedData)
  {
    if (deserializedData.Length != this.currentStatuses.Length)
    {
      string str1 = $"Deserialized data length for {this.character.gameObject.name} does not match current status length!!!\ndeserialized data:";
      for (int index = 0; index < deserializedData.Length; ++index)
        str1 = $"{str1}{deserializedData[index].ToString()}, ";
      string str2 = str1 + "\nlocal data:";
      for (int index = 0; index < this.currentStatuses.Length; ++index)
        str2 = $"{str2}{this.currentStatuses[index].ToString()}, ";
    }
    else
    {
      for (int index = 0; index < deserializedData.Length; ++index)
      {
        float amount = deserializedData[index] - this.currentStatuses[index];
        if ((double) amount > 0.0)
          this.AddStatus((CharacterAfflictions.STATUSTYPE) index, amount, true);
        if ((double) amount < 0.0)
          this.SubtractStatus((CharacterAfflictions.STATUSTYPE) index, -amount, true);
      }
    }
  }

  public void PushAfflictions(Photon.Realtime.Player specificPlayer = null)
  {
    if (!this.character.IsLocal)
      return;
    byte[] managedArray = IBinarySerializable.ToManagedArray<AfflictionSyncData>(new AfflictionSyncData()
    {
      afflictions = new List<Affliction>((IEnumerable<Affliction>) this.afflictionList)
    });
    if (specificPlayer == null)
      this.character.photonView.RPC("SyncAfflictionsRPC", RpcTarget.Others, (object) managedArray);
    else
      this.character.photonView.RPC("SyncAfflictionsRPC", specificPlayer, (object) managedArray);
  }

  [PunRPC]
  private void SyncAfflictionsRPC(byte[] data)
  {
    if (this.character.IsLocal)
      return;
    Affliction[] array = IBinarySerializable.GetFromManagedArray<AfflictionSyncData>(data).afflictions.ToArray();
    for (int index = this.afflictionList.Count - 1; index >= 0; --index)
    {
      Affliction affliction = this.afflictionList[index];
      if (!this.HasAfflictionType((IEnumerable<Affliction>) array, affliction.GetAfflictionType(), out Affliction _))
      {
        Debug.Log((object) $"{this.gameObject.name} removed old affliction: {affliction.GetAfflictionType()}");
        this.RemoveAffliction(affliction, true);
      }
    }
    for (int index = 0; index < array.Length; ++index)
    {
      Affliction affliction1 = array[index];
      Affliction affliction2;
      if (this.HasAfflictionType((IEnumerable<Affliction>) this.afflictionList, affliction1.GetAfflictionType(), out affliction2))
      {
        Debug.Log((object) $"{this.gameObject.name} stacked affliction: {affliction1.GetAfflictionType()}");
        affliction2.Stack(affliction1);
      }
      else
      {
        Debug.Log((object) $"{this.gameObject.name} added new affliction: {affliction1.GetAfflictionType()}");
        this.AddAffliction(affliction1, true);
      }
    }
  }

  public bool HasAfflictionType(Affliction.AfflictionType type, out Affliction affliction)
  {
    foreach (Affliction affliction1 in this.afflictionList)
    {
      if (affliction1.GetAfflictionType() == type)
      {
        affliction = affliction1;
        return true;
      }
    }
    affliction = (Affliction) null;
    return false;
  }

  private bool HasAfflictionType(
    IEnumerable<Affliction> afflictionList,
    Affliction.AfflictionType type,
    out Affliction affliction)
  {
    foreach (Affliction affliction1 in afflictionList)
    {
      if (affliction1.GetAfflictionType() == type)
      {
        affliction = affliction1;
        return true;
      }
    }
    affliction = (Affliction) null;
    return false;
  }

  public float GetStatusCap(CharacterAfflictions.STATUSTYPE type)
  {
    return this.statusCaps.ContainsKey(type) ? this.statusCaps[type] : 2f;
  }

  [ConsoleCommand]
  public static void TestExactStatus()
  {
    float amount = 1f - Character.localCharacter.refs.afflictions.statusSum;
    Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, amount);
  }

  [ConsoleCommand]
  public static void Starve()
  {
    Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f);
  }

  [ContextMenu("Test Poison over Time")]
  public void AddPoisonOverTime()
  {
    this.AddAffliction((Affliction) new Affliction_PoisonOverTime(10f, 0.0f, 0.05f));
  }

  [ConsoleCommand]
  public static void ClearAllAilments()
  {
    Character.localCharacter.refs.afflictions.ClearAllStatus(false);
  }

  [ConsoleCommand]
  public static void Hungry()
  {
    Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.75f);
  }

  public void ClearAllStatus(bool excludeCurse = true)
  {
    int length = Enum.GetNames(typeof (CharacterAfflictions.STATUSTYPE)).Length;
    for (int index = 0; index < length; ++index)
    {
      CharacterAfflictions.STATUSTYPE statusType = (CharacterAfflictions.STATUSTYPE) index;
      Debug.Log((object) ("Clearing status: " + statusType.ToString()));
      if (statusType != CharacterAfflictions.STATUSTYPE.Weight && statusType != CharacterAfflictions.STATUSTYPE.Crab && statusType != CharacterAfflictions.STATUSTYPE.Thorns && (!excludeCurse || statusType != CharacterAfflictions.STATUSTYPE.Curse))
      {
        Debug.Log((object) $"Current: {statusType}, amount {this.character.refs.afflictions.GetCurrentStatus(statusType)}");
        Debug.Log((object) $"SetStatus status: {statusType}");
        this.character.refs.afflictions.SetStatus(statusType, 0.0f);
      }
    }
  }

  [ConsoleCommand]
  public static void ClearHunger()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearDrowsy()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearInjury()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearCurse()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearCold()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearPoison()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearHot()
  {
    Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.0f);
  }

  [ConsoleCommand]
  public static void ClearAll() => Character.localCharacter.refs.afflictions.ClearAllStatus(false);

  public void ClearPoisonAfflictions()
  {
    List<Affliction> afflictionList = new List<Affliction>();
    foreach (Affliction affliction in this.afflictionList)
    {
      if (affliction is Affliction_PoisonOverTime)
        afflictionList.Add(affliction);
    }
    foreach (Affliction affliction in afflictionList)
      this.RemoveAffliction(affliction);
  }

  [ContextMenu("Test Full Drowsy")]
  [ConsoleCommand]
  public static void AddDrowsy()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0.2f);
  }

  [ContextMenu("Test Curse")]
  [ConsoleCommand]
  public static void AddCurse()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.2f);
  }

  [ContextMenu("Test Death")]
  [ConsoleCommand]
  public static void Die()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f);
  }

  [ContextMenu("Add Poison")]
  [ConsoleCommand]
  public static void AddPoison()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.2f);
  }

  [ContextMenu("Test Cold")]
  [ConsoleCommand]
  public static void AddCold()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.2f);
  }

  [ContextMenu("Test Hot")]
  [ConsoleCommand]
  public static void AddHot()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.2f);
  }

  [ContextMenu("Test Injury")]
  [ConsoleCommand]
  public static void AddInjury()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.2f);
  }

  [ContextMenu("Test Hunger")]
  [ConsoleCommand]
  public static void AddHunger()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.2f);
  }

  [ContextMenu("Test Crab")]
  public static void TestCrab()
  {
    PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Crab, 0.2f);
  }

  [ConsoleCommand]
  public static void GetThorned() => Character.localCharacter.refs.afflictions.AddThorn();

  [ConsoleCommand]
  public static void GetUnThorned()
  {
    foreach (ThornOnMe physicalThorn in Character.localCharacter.refs.afflictions.physicalThorns)
    {
      if (physicalThorn.gameObject.activeInHierarchy)
      {
        Character.localCharacter.refs.afflictions.RemoveThorn(physicalThorn);
        break;
      }
    }
  }

  public void AddThorn(Vector3 position)
  {
    if (!this.photonView.IsMine)
      return;
    float num1 = float.MaxValue;
    ThornOnMe thornOnMe = (ThornOnMe) null;
    foreach (ushort availableThornIndex in this.availableThornIndices)
    {
      float num2 = Vector3.Distance(this.physicalThorns[(int) availableThornIndex].transform.position, position);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        thornOnMe = this.physicalThorns[(int) availableThornIndex];
      }
    }
    if (!((UnityEngine.Object) thornOnMe != (UnityEngine.Object) null))
      return;
    this.AddThorn((ushort) this.physicalThorns.IndexOf(thornOnMe));
  }

  public void AddThorn(ushort thornIndex = 999)
  {
    if (!this.photonView.IsMine || this.availableThornIndices.Count == 0 || this.character.data.isInvincible)
      return;
    if (thornIndex == (ushort) 999 || !this.availableThornIndices.Contains(thornIndex))
      thornIndex = this.availableThornIndices.RandomSelection<ushort>((Func<ushort, int>) (u => 1));
    this.photonView.RPC("RPC_EnableThorn", RpcTarget.All, (object) (int) thornIndex);
    this.availableThornIndices.Remove(thornIndex);
    this.thornData.stuckThornIndices.Add(thornIndex);
    this.UpdateWeight();
  }

  [PunRPC]
  public void RPC_EnableThorn(int thornIndex) => this.physicalThorns[thornIndex].EnableThorn();

  [PunRPC]
  public void RPC_DisableThorn(int thornIndex) => this.physicalThorns[thornIndex].DisableThorn();

  public void RemoveAllThorns()
  {
    foreach (ThornOnMe physicalThorn in this.physicalThorns)
      this.RemoveThorn(physicalThorn);
  }

  [PunRPC]
  public void RemoveThornRPC(int index)
  {
    if (this.photonView.IsMine)
    {
      ThornOnMe physicalThorn = this.physicalThorns[index];
      this.photonView.RPC("RPC_DisableThorn", RpcTarget.All, (object) index);
      this.availableThornIndices.Add((ushort) index);
      this.thornData.stuckThornIndices.Remove((ushort) index);
      this.UpdateWeight();
    }
    else
      this.photonView.RPC(nameof (RemoveThornRPC), this.photonView.Owner, (object) index);
  }

  public void RemoveRandomThornLinq()
  {
    this.RemoveThorn(this.physicalThorns.Where<ThornOnMe>((Func<ThornOnMe, bool>) (t => t.stuckIn)).RandomSelection<ThornOnMe>((Func<ThornOnMe, int>) (t1 => 1)));
  }

  public void RemoveThorn(ThornOnMe thornToRemove)
  {
    for (ushort index = 0; (int) index < this.physicalThorns.Count; ++index)
    {
      if ((UnityEngine.Object) this.physicalThorns[(int) index] == (UnityEngine.Object) thornToRemove)
      {
        this.RemoveThornRPC((int) index);
        break;
      }
    }
  }

  public int GetTotalThornStatusIncrements()
  {
    int statusIncrements = 0;
    foreach (ThornOnMe physicalThorn in this.physicalThorns)
    {
      if (physicalThorn.stuckIn)
        statusIncrements += physicalThorn.thornDamage;
    }
    return statusIncrements;
  }

  public enum STATUSTYPE
  {
    Injury,
    Hunger,
    Cold,
    Poison,
    Crab,
    Curse,
    Drowsy,
    Weight,
    Hot,
    Thorns,
  }
}
