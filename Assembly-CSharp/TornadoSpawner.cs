// Decompiled with JetBrains decompiler
// Type: TornadoSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class TornadoSpawner : MonoBehaviour
{
  public float minSpawnTimeFirst = 30f;
  public float maxSpawnTimeFirst = 300f;
  public float minSpawnTime = 30f;
  public float maxSpawnTime = 300f;
  private float untilNext;
  private bool firstTime = true;
  private PhotonView view;

  private void Start()
  {
    this.untilNext = Random.Range(this.minSpawnTimeFirst, this.maxSpawnTimeFirst);
    this.view = this.GetComponent<PhotonView>();
  }

  private void Update()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.untilNext -= Time.deltaTime;
    if ((double) this.untilNext > 0.0)
      return;
    this.SpawnTornado();
    this.untilNext = Random.Range(this.minSpawnTime, this.maxSpawnTime);
  }

  private void SpawnTornado()
  {
    PhotonNetwork.Instantiate("Tornado", this.GetSpawnPos(), Quaternion.identity).GetComponent<PhotonView>().RPC("RPCA_InitTornado", RpcTarget.All, (object) this.view.ViewID);
  }

  private Vector3 GetSpawnPos()
  {
    Transform transform = this.transform.Find("TornadoPoints");
    return transform.GetChild(Random.Range(0, transform.childCount)).position;
  }
}
