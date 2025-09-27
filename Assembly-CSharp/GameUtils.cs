// Decompiled with JetBrains decompiler
// Type: GameUtils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class GameUtils : MonoBehaviourPunCallbacks
{
  public static GameUtils instance;
  [SerializeField]
  public List<FeedData> feedData = new List<FeedData>();
  public Action OnUpdatedFeedData;
  internal PhotonView photonView;
  private List<GameUtils.IgnoredCollidersEntry> ignoredCollidersCache = new List<GameUtils.IgnoredCollidersEntry>();

  private void Awake()
  {
    GameUtils.instance = this;
    this.photonView = this.GetComponent<PhotonView>();
  }

  public void StartFeed(int giverID, int receiverID, ushort itemID, float totalItemTime)
  {
    this.feedData.Add(new FeedData()
    {
      giverID = giverID,
      receiverID = receiverID,
      itemID = itemID,
      totalItemTime = totalItemTime
    });
    Action onUpdatedFeedData = this.OnUpdatedFeedData;
    if (onUpdatedFeedData == null)
      return;
    onUpdatedFeedData();
  }

  public List<FeedData> GetFeedDataForReceiver(int receiverID)
  {
    return this.feedData.FindAll((Predicate<FeedData>) (x => x.receiverID == receiverID));
  }

  public void EndFeed(int giverID)
  {
    for (int index = this.feedData.Count - 1; index >= 0; --index)
    {
      if (this.feedData[index].giverID == giverID)
        this.feedData.RemoveAt(index);
    }
    Action onUpdatedFeedData = this.OnUpdatedFeedData;
    if (onUpdatedFeedData == null)
      return;
    onUpdatedFeedData();
  }

  private void FixedUpdate() => this.UpdateCollisionIgnores();

  private void UpdateCollisionIgnores()
  {
    for (int index = this.ignoredCollidersCache.Count - 1; index >= 0; --index)
    {
      this.ignoredCollidersCache[index].time -= Time.fixedDeltaTime;
      if ((double) this.ignoredCollidersCache[index].time <= 0.0)
      {
        if ((UnityEngine.Object) this.ignoredCollidersCache[index].colliderA != (UnityEngine.Object) null && (UnityEngine.Object) this.ignoredCollidersCache[index].colliderB != (UnityEngine.Object) null)
          Physics.IgnoreCollision(this.ignoredCollidersCache[index].colliderA, this.ignoredCollidersCache[index].colliderB, false);
        this.ignoredCollidersCache.RemoveAt(index);
      }
    }
  }

  public void IgnoreCollisions(GameObject object1, GameObject object2, float time)
  {
    this.IgnoreCollisions(object1.GetComponentsInChildren<Collider>(), object2.GetComponentsInChildren<Collider>(), time);
  }

  public void IgnoreCollisions(Character c, Item item)
  {
  }

  public void IgnoreCollisions(Collider[] collidersA, Collider[] collidersB, float time)
  {
    foreach (Collider collider1 in collidersA)
    {
      foreach (Collider collider2 in collidersB)
      {
        Physics.IgnoreCollision(collider1, collider2);
        this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider1, collider2, time));
      }
    }
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (PhotonNetwork.IsMasterClient)
    {
      GameHandler.GetService<PersistentPlayerDataService>().SyncToPlayer(newPlayer);
      this.photonView.RPC("RPC_SyncAscent", newPlayer, (object) Ascents.currentAscent);
    }
    GlobalEvents.TriggerPlayerConnected(newPlayer);
  }

  public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerLeftRoom(newPlayer);
    GlobalEvents.TriggerPlayerDisconnected(newPlayer);
  }

  internal void SyncAscentAll(int ascent)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.photonView.RPC("RPC_SyncAscent", RpcTarget.All, (object) ascent);
  }

  [PunRPC]
  internal void RPC_SyncAscent(int ascent) => Ascents.currentAscent = ascent;

  internal void ThrowBingBongAchievement()
  {
    this.photonView.RPC("ThrowBingBongAchievementRpc", RpcTarget.All);
  }

  [PunRPC]
  private void ThrowBingBongAchievementRpc()
  {
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.BingBongBadge);
  }

  internal void ThrowSacrificeAchievement()
  {
    this.photonView.RPC("ThrowSacrificeAchievementRpc", RpcTarget.All);
  }

  [PunRPC]
  private void ThrowSacrificeAchievementRpc()
  {
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.TwentyFourKaratBadge);
  }

  internal void IncrementPermanentItemsPlaced()
  {
    this.photonView.RPC("IncrementPermanentItemsPlacedRpc", RpcTarget.All);
  }

  [PunRPC]
  private void IncrementPermanentItemsPlacedRpc()
  {
    Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced, 1);
  }

  internal void IncrementFriendHealing(int amt, Photon.Realtime.Player target)
  {
    this.photonView.RPC("IncrementFriendHealingRpc", target, (object) amt);
  }

  [PunRPC]
  private void IncrementFriendHealingRpc(int amt)
  {
    Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.FriendsHealedAmount, amt);
  }

  internal void IncrementFriendPoisonHealing(int amt, Photon.Realtime.Player target)
  {
    this.photonView.RPC("IncrementPoisonHealedStat", target, (object) amt);
  }

  [PunRPC]
  protected void IncrementPoisonHealedStat(int amt)
  {
    Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, amt);
  }

  internal void ThrowEmergencyPreparednessAchievement(Photon.Realtime.Player target)
  {
    this.photonView.RPC("ThrowEmergencyPreparednessAchievementRpc", target);
  }

  [PunRPC]
  private void ThrowEmergencyPreparednessAchievementRpc()
  {
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EmergencyPreparednessBadge);
  }

  [PunRPC]
  private void InstantiateAndGrabRPC(string itemPrefabName, PhotonView characterView)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    Character component = characterView.GetComponent<Character>();
    component.refs.items.lastEquippedSlotTime = 0.0f;
    Bodypart bodypart = component.GetBodypart(BodypartType.Hip);
    PhotonNetwork.InstantiateItemRoom(itemPrefabName, bodypart.transform.position + bodypart.transform.forward * 0.5f, Quaternion.identity).GetComponent<Item>().Interact(component);
  }

  public void InstantiateAndGrab(Item item, Character character)
  {
    this.photonView.RPC("InstantiateAndGrabRPC", RpcTarget.MasterClient, (object) item.gameObject.name, (object) character.photonView);
  }

  public void SyncLava(bool started, bool ended, float time, float timeWaited)
  {
    this.photonView.RPC("RPC_SyncLava", RpcTarget.Others, (object) started, (object) ended, (object) time, (object) timeWaited);
  }

  [PunRPC]
  public void RPC_SyncLava(bool started, bool ended, float time, float timeWaited)
  {
    Singleton<LavaRising>.Instance.RecieveLavaData(started, ended, time, timeWaited);
  }

  [ContextMenu("Debug All Items")]
  private void DebugAllItems()
  {
    string message1 = "";
    foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
      message1 = $"{message1}{keyValuePair.Value.UIData.itemName}\n";
    Debug.Log((object) message1);
    string message2 = "";
    foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
      message2 = $"{message2}{keyValuePair.Value.gameObject.name}\n";
    Debug.Log((object) message2);
  }

  private class IgnoredCollidersEntry
  {
    public Collider colliderA;
    public Collider colliderB;
    public float time;

    public IgnoredCollidersEntry(Collider A, Collider B, float time)
    {
      this.colliderA = A;
      this.colliderB = B;
      this.time = time;
    }
  }
}
