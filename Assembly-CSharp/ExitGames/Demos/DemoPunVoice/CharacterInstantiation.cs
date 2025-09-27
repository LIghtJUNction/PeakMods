// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.CharacterInstantiation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

public class CharacterInstantiation : MonoBehaviourPunCallbacks, IOnEventCallback
{
  public Transform SpawnPosition;
  public float PositionOffset = 2f;
  public GameObject[] PrefabsToInstantiate;
  public List<Transform> SpawnPoints;
  public bool AutoSpawn = true;
  public bool UseRandomOffset = true;
  public CharacterInstantiation.SpawnSequence Sequence;
  [SerializeField]
  private byte manualInstantiationEventCode = 1;
  protected int lastUsedSpawnPointIndex = -1;
  [SerializeField]
  private bool manualInstantiation;
  [SerializeField]
  private bool differentPrefabs;
  [SerializeField]
  private string localPrefabSuffix;
  [SerializeField]
  private string remotePrefabSuffix;

  public static event CharacterInstantiation.OnCharacterInstantiated CharacterInstantiated;

  public override void OnJoinedRoom()
  {
    if (!this.AutoSpawn || this.PrefabsToInstantiate == null)
      return;
    int num = PhotonNetwork.LocalPlayer.ActorNumber;
    if (num < 1)
      num = 1;
    int index = (num - 1) % this.PrefabsToInstantiate.Length;
    Vector3 spawnPos;
    Quaternion spawnRot;
    this.GetSpawnPoint(out spawnPos, out spawnRot);
    Camera.main.transform.position += spawnPos;
    if (this.manualInstantiation)
    {
      this.ManualInstantiation(index, spawnPos, spawnRot);
    }
    else
    {
      GameObject character = PhotonNetwork.Instantiate(this.PrefabsToInstantiate[index].name, spawnPos, spawnRot);
      if (CharacterInstantiation.CharacterInstantiated == null)
        return;
      CharacterInstantiation.CharacterInstantiated(character);
    }
  }

  private void ManualInstantiation(int index, Vector3 position, Quaternion rotation)
  {
    GameObject original = this.PrefabsToInstantiate[index];
    GameObject character = !this.differentPrefabs ? Object.Instantiate<GameObject>(original, position, rotation) : Object.Instantiate<GameObject>(Resources.Load($"{original.name}{this.localPrefabSuffix}") as GameObject, position, rotation);
    PhotonView component = character.GetComponent<PhotonView>();
    if (PhotonNetwork.AllocateViewID(component))
    {
      PhotonNetwork.RaiseEvent(this.manualInstantiationEventCode, (object) new object[4]
      {
        (object) index,
        (object) character.transform.position,
        (object) character.transform.rotation,
        (object) component.ViewID
      }, new RaiseEventOptions()
      {
        Receivers = ReceiverGroup.Others,
        CachingOption = EventCaching.AddToRoomCache
      }, SendOptions.SendReliable);
      if (CharacterInstantiation.CharacterInstantiated == null)
        return;
      CharacterInstantiation.CharacterInstantiated(character);
    }
    else
    {
      Debug.LogError((object) "Failed to allocate a ViewId.");
      Object.Destroy((Object) character);
    }
  }

  public void OnEvent(EventData photonEvent)
  {
    if ((int) photonEvent.Code != (int) this.manualInstantiationEventCode)
      return;
    object[] customData = photonEvent.CustomData as object[];
    GameObject original = this.PrefabsToInstantiate[(int) customData[0]];
    Vector3 position = (Vector3) customData[1];
    Quaternion rotation = (Quaternion) customData[2];
    (!this.differentPrefabs ? Object.Instantiate<GameObject>(original, position, Quaternion.identity) : Object.Instantiate<GameObject>(Resources.Load($"{original.name}{this.remotePrefabSuffix}") as GameObject, position, rotation)).GetComponent<PhotonView>().ViewID = (int) customData[3];
  }

  protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
  {
    Transform spawnPoint = this.GetSpawnPoint();
    if ((Object) spawnPoint != (Object) null)
    {
      spawnPos = spawnPoint.position;
      spawnRot = spawnPoint.rotation;
    }
    else
    {
      spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
      spawnRot = new Quaternion(0.0f, 0.0f, 0.0f, 1f);
    }
    if (!this.UseRandomOffset)
      return;
    Debug.Log((object) "Set Seed");
    Random.InitState((int) ((double) Time.time * 10000.0));
    Vector3 vector3 = Random.insideUnitSphere with
    {
      y = 0.0f
    };
    vector3 = vector3.normalized;
    spawnPos += this.PositionOffset * vector3;
  }

  protected virtual Transform GetSpawnPoint()
  {
    if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
      return (Transform) null;
    switch (this.Sequence)
    {
      case CharacterInstantiation.SpawnSequence.Connection:
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        return this.SpawnPoints[actorNumber == -1 ? 0 : actorNumber % this.SpawnPoints.Count];
      case CharacterInstantiation.SpawnSequence.Random:
        return this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
      case CharacterInstantiation.SpawnSequence.RoundRobin:
        ++this.lastUsedSpawnPointIndex;
        if (this.lastUsedSpawnPointIndex >= this.SpawnPoints.Count)
          this.lastUsedSpawnPointIndex = 0;
        return this.SpawnPoints[this.lastUsedSpawnPointIndex];
      default:
        return (Transform) null;
    }
  }

  public enum SpawnSequence
  {
    Connection,
    Random,
    RoundRobin,
  }

  public delegate void OnCharacterInstantiated(GameObject character);
}
