// Decompiled with JetBrains decompiler
// Type: EruptionSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class EruptionSpawner : MonoBehaviour
{
  private float counter = 10f;
  public GameObject eruption;
  private PhotonView photonView;
  private Transform min;
  private Transform max;

  private void Start()
  {
    this.min = this.transform.GetChild(0);
    this.max = this.transform.GetChild(1);
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void Update()
  {
    if (!PhotonNetwork.IsMasterClient || !HelperFunctions.AnyPlayerInZRange(this.min.position.z, this.max.position.z))
      return;
    this.counter -= Time.deltaTime;
    if ((double) this.counter >= 0.0)
      return;
    this.counter = Random.Range(-5f, 15f);
    Vector3 position = this.transform.position;
    position.x += Random.Range(-155f, 155f);
    position.z += Random.Range(-140f, 140f);
    this.photonView.RPC("RPCA_SpawnEruption", RpcTarget.All, (object) position);
  }

  [PunRPC]
  public void RPCA_SpawnEruption(Vector3 position)
  {
    Object.Instantiate<GameObject>(this.eruption, position, Quaternion.LookRotation(Vector3.up));
  }
}
