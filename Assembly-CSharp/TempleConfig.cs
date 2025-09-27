// Decompiled with JetBrains decompiler
// Type: TempleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class TempleConfig : MonoBehaviourPunCallbacks
{
  private PhotonView view;
  [Range(0.0f, 1f)]
  public float arrowShooterChance;
  public List<GameObject> columns;
  private List<Vector3> positions = new List<Vector3>();
  public GameObject[] arrowShooters;

  private void Awake() => this.view = this.GetComponent<PhotonView>();

  private void Start()
  {
    for (int index = 0; index < this.columns.Count; ++index)
      this.positions.Add(this.columns[index].transform.position);
  }

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    if (!this.view.IsMine)
      return;
    this.view.RPC("CreateTemple_RPC", RpcTarget.AllBuffered, (object) (int) DateTime.Now.Ticks);
  }

  [PunRPC]
  public void CreateTemple_RPC(int seed)
  {
    Debug.Log((object) "Set Seed");
    UnityEngine.Random.InitState(seed);
    List<GameObject> list = this.columns.OrderBy<GameObject, float>((Func<GameObject, float>) (x => UnityEngine.Random.value)).ToList<GameObject>();
    for (int index = 0; index < list.Count; ++index)
    {
      list[index].transform.position = this.positions[index];
      this.columns[index].transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, (float) ((int) ((double) UnityEngine.Random.value * 4.0) * 90)));
    }
    for (int index = 0; index < this.arrowShooters.Length; ++index)
    {
      if ((double) UnityEngine.Random.value < (double) this.arrowShooterChance)
        this.arrowShooters[index].SetActive(true);
      else
        this.arrowShooters[index].SetActive(false);
    }
  }

  private void Update()
  {
  }
}
