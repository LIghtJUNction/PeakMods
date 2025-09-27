// Decompiled with JetBrains decompiler
// Type: MapHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.PhotonUtility;

#nullable disable
public class MapHandler : Singleton<MapHandler>
{
  public Transform globalParent;
  public MapHandler.MapSegment[] segments;
  public MapHandler.MapSegment[] variantSegments;
  public Transform respawnTheKiln;
  public Transform respawnThePeak;
  public LavaRising lavaRising;
  [SerializeField]
  private int currentSegment;
  private bool hasSpawnedInitialSpawners;
  private ListenerHandle debugCommandHandle;
  private bool hasEnded;
  private bool hasCutsceneEnded;
  private List<PhotonView> viewsToDestoryIfNotAlreadyWhenSwitchingSegments = new List<PhotonView>();
  public List<Biome.BiomeType> biomes = new List<Biome.BiomeType>();

  protected override void Awake()
  {
    base.Awake();
    this.debugCommandHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncMapHandlerDebugCommandPackage>(new Action<SyncMapHandlerDebugCommandPackage>(this.OnPackageHandle));
    if (!Application.isEditor)
      return;
    this.DetectBiomes();
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
  }

  private IEnumerator Start()
  {
    yield return (object) null;
    for (int index = 1; index < this.segments.Length; ++index)
    {
      this.segments[index].segmentParent.SetActive(false);
      if ((UnityEngine.Object) this.segments[index].segmentCampfire != (UnityEngine.Object) null)
        this.segments[index].segmentCampfire.SetActive(false);
      Debug.Log((object) $"Disabling segment: {index} with parent: {this.segments[index].segmentParent.name}");
    }
    this.segments[0].wallNext.SetActive(true);
  }

  internal void DetectBiomes()
  {
    this.biomes.Clear();
    for (int index1 = 0; index1 < this.transform.childCount; ++index1)
    {
      Transform child = this.transform.GetChild(index1);
      for (int index2 = 0; index2 < child.childCount; ++index2)
      {
        Biome component;
        if (child.GetChild(index2).gameObject.activeInHierarchy && child.GetChild(index2).TryGetComponent<Biome>(out component))
          this.biomes.Add(component.biomeType);
      }
    }
  }

  private void Update()
  {
    if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && !this.hasSpawnedInitialSpawners)
    {
      ISpawner[] componentsInChildren1 = this.segments[0].segmentParent.GetComponentsInChildren<ISpawner>();
      ISpawner[] componentsInChildren2 = this.segments[0].segmentCampfire.GetComponentsInChildren<ISpawner>();
      ISpawner[] componentsInChildren3 = this.globalParent.GetComponentsInChildren<ISpawner>();
      this.hasSpawnedInitialSpawners = true;
      foreach (ISpawner spawner in componentsInChildren1)
        this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange((IEnumerable<PhotonView>) spawner.TrySpawnItems());
      foreach (ISpawner spawner in componentsInChildren2)
        spawner.TrySpawnItems();
      foreach (ISpawner spawner in componentsInChildren3)
        spawner.TrySpawnItems();
    }
    else if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
      this.hasSpawnedInitialSpawners = true;
    bool flag1 = false;
    List<Player> allPlayers = PlayerHandler.GetAllPlayers();
    int num1 = allPlayers.Count;
    if (allPlayers.Count > 4)
      num1 = allPlayers.Count / 2;
    else if (allPlayers.Count == 4)
      num1 = 3;
    else if (allPlayers.Count == 3)
      num1 = 2;
    if (allPlayers.Count<Player>((Func<Player, bool>) (player => player.hasClosedEndScreen)) >= num1)
      flag1 = true;
    if (flag1 && allPlayers.Count > 0 && !GameHandler.TryGetStatus<EndScreenStatus>(out EndScreenStatus _) && !this.hasEnded)
    {
      int num2 = Character.localCharacter.refs.stats.won ? 1 : (Character.localCharacter.refs.stats.somebodyElseWon ? 1 : 0);
      this.hasEnded = true;
      if (num2 != 0)
      {
        GameHandler.AddStatus<EndScreenStatus>((GameStatus) new EndScreenStatus());
        Singleton<PeakHandler>.Instance.EndScreenComplete();
      }
      else
      {
        Debug.LogError((object) "Everyone has closed end screen.. Loading airport");
        Singleton<GameOverHandler>.Instance.LoadAirport();
      }
    }
    bool flag2 = false;
    foreach (Player player in allPlayers)
    {
      if (player.doneWithCutscene)
      {
        flag2 = true;
        break;
      }
    }
    if (!flag2 || allPlayers.Count <= 0 || this.hasCutsceneEnded)
      return;
    this.hasCutsceneEnded = true;
    Debug.Log((object) "Everyone is done with cutscene, loading airport");
    GameHandler.AddStatus<SceneSwitchingStatus>((GameStatus) new SceneSwitchingStatus());
    RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 1f));
  }

  public void GoToSegment(Segment s)
  {
    if (s <= (Segment) this.currentSegment)
    {
      Debug.LogError((object) $"Trying to transition to segment already passed: {s}");
    }
    else
    {
      Debug.Log((object) $"Going to segment: {s}");
      this.StartCoroutine(ShowNextSegmentCoroutine());
    }

    IEnumerator ShowNextSegmentCoroutine()
    {
      int startSegment = this.currentSegment;
      ++this.currentSegment;
      OrbFogHandler orbFogHandler = Singleton<OrbFogHandler>.Instance;
      yield return (object) orbFogHandler.WaitForFogCatchUp();
      yield return (object) new WaitForSecondsRealtime(1f);
      this.segments[startSegment].segmentParent.SetActive(false);
      yield return (object) null;
      this.segments[this.currentSegment].segmentParent.SetActive(true);
      EnablingSubstep[] array = ((IEnumerable<EnablingSubstep>) this.segments[this.currentSegment].segmentParent.GetComponentsInChildren<EnablingSubstep>()).Where<EnablingSubstep>((Func<EnablingSubstep, bool>) (substep => substep.gameObject.activeSelf)).ToArray<EnablingSubstep>();
      foreach (Component component in array)
        component.gameObject.SetActive(false);
      if ((bool) (UnityEngine.Object) this.segments[startSegment].wallNext)
        this.segments[startSegment].wallNext.SetActive(false);
      if ((bool) (UnityEngine.Object) this.segments[startSegment].wallPrevious)
        this.segments[startSegment].wallPrevious.SetActive(false);
      if ((bool) (UnityEngine.Object) this.segments[this.currentSegment].wallNext)
        this.segments[this.currentSegment].wallNext.SetActive(true);
      if ((bool) (UnityEngine.Object) this.segments[this.currentSegment].wallPrevious)
        this.segments[this.currentSegment].wallPrevious.SetActive(true);
      this.segments[this.currentSegment].segmentParent.SetActive(true);
      EnablingSubstep[] enablingSubstepArray = array;
      for (int index = 0; index < enablingSubstepArray.Length; ++index)
      {
        EnablingSubstep substep = enablingSubstepArray[index];
        yield return (object) new WaitForSeconds(0.15f);
        substep.gameObject.SetActive(true);
        Debug.Log((object) $"Enabling substep: {substep}");
        substep = (EnablingSubstep) null;
      }
      enablingSubstepArray = (EnablingSubstep[]) null;
      if (PhotonNetwork.IsMasterClient)
      {
        foreach (PhotonView switchingSegment in this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments)
        {
          if (!((UnityEngine.Object) switchingSegment == (UnityEngine.Object) null))
          {
            bool flag = true;
            try
            {
              foreach (Character allPlayerCharacter in PlayerHandler.GetAllPlayerCharacters())
              {
                if (!((UnityEngine.Object) allPlayerCharacter == (UnityEngine.Object) null) && (double) Vector3.Distance(switchingSegment.transform.position, allPlayerCharacter.Center) < 60.0)
                  flag = false;
              }
              if ((UnityEngine.Object) switchingSegment != (UnityEngine.Object) null & flag)
              {
                Debug.Log((object) ("Removing: " + switchingSegment.gameObject.name));
                PhotonNetwork.Destroy(switchingSegment);
              }
            }
            catch (Exception ex)
            {
              Debug.LogError((object) ("Exception while trying to remove old item: " + ex.ToString()));
            }
          }
        }
        this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.Clear();
      }
      if ((UnityEngine.Object) this.segments[this.currentSegment].segmentCampfire != (UnityEngine.Object) null)
        this.segments[this.currentSegment].segmentCampfire.SetActive(true);
      if (this.segments.WithinRange<MapHandler.MapSegment>(startSegment - 1) && (UnityEngine.Object) this.segments[startSegment - 1].segmentCampfire != (UnityEngine.Object) null)
        this.segments[startSegment - 1].segmentCampfire.SetActive(false);
      if (PhotonNetwork.IsMasterClient)
      {
        Debug.Log((object) ("Spawning from parent: " + this.segments[this.currentSegment].segmentParent.gameObject.name));
        List<ISpawner> list = ((IEnumerable<ISpawner>) this.segments[this.currentSegment].segmentParent.GetComponentsInChildren<ISpawner>()).ToList<ISpawner>();
        if (s == Segment.TheKiln)
          list.AddRange((IEnumerable<ISpawner>) Singleton<PeakHandler>.Instance.gameObject.GetComponentsInChildren<ISpawner>());
        foreach (ISpawner spawner in list)
          this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange((IEnumerable<PhotonView>) spawner.TrySpawnItems());
        if ((bool) (UnityEngine.Object) this.segments[this.currentSegment].segmentCampfire)
        {
          Debug.Log((object) ("Spawning items in " + this.segments[this.currentSegment].segmentCampfire.gameObject.name));
          foreach (ISpawner componentsInChild in this.segments[this.currentSegment].segmentCampfire.GetComponentsInChildren<ISpawner>())
            componentsInChild.TrySpawnItems();
        }
        else
          Debug.Log((object) "NO CAMPFIRE SEGMENT");
      }
      if ((UnityEngine.Object) this.segments[this.currentSegment].dayNightProfile != (UnityEngine.Object) null)
        DayNightManager.instance.BlendProfiles(this.segments[this.currentSegment].dayNightProfile);
      yield return (object) new WaitForSeconds(0.5f);
      this.StartCoroutine(ShowTitleText());
      yield return (object) orbFogHandler.WaitForReveal();
      if (s != Segment.TheKiln)
        orbFogHandler.SetFogOrigin(this.currentSegment);

      IEnumerator ShowTitleText()
      {
        yield return (object) new WaitForSeconds(0.5f);
        Singleton<MountainProgressHandler>.Instance.SetSegmentComplete(startSegment + 1);
      }
    }
  }

  [ConsoleCommand]
  public static void JumpToSegment(Segment segment)
  {
    MapHandler.JumpToSegmentLogic(segment, PlayerHandler.GetAllPlayers().Select<Player, int>((Func<Player, int>) (player => player.photonView.Owner.ActorNumber)).ToHashSet<int>(), true);
  }

  private static void JumpToSegmentLogic(
    Segment segment,
    HashSet<int> playersToTeleport,
    bool sendToEveryone)
  {
    Singleton<MapHandler>.Instance.currentSegment = (int) segment;
    Debug.Log((object) $"Jumping to segment: {segment}");
    foreach (MapHandler.MapSegment segment1 in Singleton<MapHandler>.Instance.segments)
    {
      segment1.segmentParent.SetActive(false);
      if ((bool) (UnityEngine.Object) segment1.segmentCampfire)
        segment1.segmentCampfire.SetActive(false);
      if ((bool) (UnityEngine.Object) segment1.wallNext)
        segment1.wallNext.gameObject.SetActive(false);
      if ((bool) (UnityEngine.Object) segment1.wallPrevious)
        segment1.wallPrevious.gameObject.SetActive(false);
    }
    int id = (int) segment;
    if (segment == Segment.Peak)
      --id;
    MapHandler.MapSegment segment2 = Singleton<MapHandler>.Instance.segments[id];
    segment2.segmentParent.SetActive(true);
    if ((bool) (UnityEngine.Object) segment2.segmentCampfire)
      segment2.segmentCampfire.SetActive(true);
    if ((bool) (UnityEngine.Object) segment2.wallNext)
      segment2.wallNext.gameObject.SetActive(true);
    if ((bool) (UnityEngine.Object) segment2.wallPrevious)
      segment2.wallPrevious.gameObject.SetActive(true);
    Vector3 position = segment2.reconnectSpawnPos.position;
    if (segment == Segment.Peak)
      position = Singleton<MapHandler>.Instance.respawnThePeak.position;
    if (id > 0)
    {
      MapHandler.MapSegment segment3 = Singleton<MapHandler>.Instance.segments[id - 1];
      if ((UnityEngine.Object) segment3.segmentCampfire != (UnityEngine.Object) null)
        segment3.segmentCampfire.SetActive(true);
    }
    if (PhotonNetwork.IsMasterClient)
    {
      Debug.Log((object) $"Spawning items in {segment}. Parent: {segment2.segmentParent.gameObject.name}");
      ISpawner[] componentsInChildren = segment2.segmentParent.GetComponentsInChildren<ISpawner>();
      int num = 0;
      foreach (ISpawner spawner in componentsInChildren)
      {
        Debug.Log((object) $"Spawning...{num.ToString()} {spawner.GetType()?.ToString()}");
        spawner.TrySpawnItems();
        ++num;
      }
      if ((bool) (UnityEngine.Object) segment2.segmentCampfire)
      {
        Debug.Log((object) ("Spawning items in " + segment2.segmentCampfire.gameObject.name));
        foreach (ISpawner componentsInChild in segment2.segmentCampfire.GetComponentsInChildren<ISpawner>())
          componentsInChild.TrySpawnItems();
      }
      else
        Debug.Log((object) "NO CAMPFIRE SEGMENT");
    }
    Singleton<OrbFogHandler>.Instance.SetFogOrigin(id);
    if ((UnityEngine.Object) segment2.dayNightProfile != (UnityEngine.Object) null)
      DayNightManager.instance.BlendProfiles(segment2.dayNightProfile);
    if (PhotonNetwork.IsMasterClient)
    {
      Debug.Log((object) $"Teleporting all players to {segment} campfire..");
      foreach (Character allPlayerCharacter in PlayerHandler.GetAllPlayerCharacters())
      {
        if (playersToTeleport.Contains(allPlayerCharacter.photonView.Owner.ActorNumber))
          allPlayerCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, (object) position, (object) false);
      }
    }
    if (!sendToEveryone)
      return;
    CustomCommands<CustomCommandType>.SendPackage((CustomCommandPackage<CustomCommandType>) new SyncMapHandlerDebugCommandPackage(segment, Array.Empty<int>()), ReceiverGroup.Others);
  }

  private void OnPackageHandle(SyncMapHandlerDebugCommandPackage p)
  {
    MapHandler.JumpToSegmentLogic(p.Segment, ((IEnumerable<int>) p.PlayerToTeleport).ToHashSet<int>(), false);
  }

  public Segment GetCurrentSegment() => (Segment) this.currentSegment;

  public Biome.BiomeType GetCurrentBiome() => this.segments[this.currentSegment].biome;

  public bool BiomeIsPresent(Biome.BiomeType biomeType) => this.biomes.Contains(biomeType);

  public MapHandler.MapSegment GetVariantSegmentFromBiome(Biome.BiomeType biome)
  {
    for (int index = 0; index < this.variantSegments.Length; ++index)
    {
      if (this.variantSegments[index].biome == biome)
        return this.variantSegments[index];
    }
    Debug.LogError((object) "COULDNT FIND SEGMENT FROM BIOME. RETURNING SHORE SEGMENT");
    return this.segments[0];
  }

  [Serializable]
  public class MapSegment
  {
    [SerializeField]
    private Biome.BiomeType _biome;
    [SerializeField]
    private GameObject _segmentParent;
    [SerializeField]
    private GameObject _segmentCampfire;
    public GameObject wallNext;
    public GameObject wallPrevious;
    public Transform reconnectSpawnPos;
    [SerializeField]
    private DayNightProfile _dayNightProfile;
    public bool hasVariant;
    public Biome.BiomeType variantBiome;
    public bool isVariant;

    public Biome.BiomeType biome
    {
      get
      {
        return this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome) ? Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).biome : this._biome;
      }
    }

    public GameObject segmentParent
    {
      get
      {
        return this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome) ? Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentParent : this._segmentParent;
      }
    }

    public GameObject segmentCampfire
    {
      get
      {
        return this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome) ? Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentCampfire : this._segmentCampfire;
      }
    }

    public DayNightProfile dayNightProfile
    {
      get
      {
        return this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome) ? Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).dayNightProfile : this._dayNightProfile;
      }
    }
  }
}
