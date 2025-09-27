// Decompiled with JetBrains decompiler
// Type: Action_Spawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class Action_Spawn : ItemAction
{
  public GameObject objectToSpawn;
  public Transform spawnPoint;

  public override void RunAction()
  {
    this.item.photonView.RPC("RPCSpawn", RpcTarget.All, (object) this.spawnPoint.transform.position, (object) this.spawnPoint.transform.rotation);
  }

  [PunRPC]
  public void RPCSpawn(Vector3 position, Quaternion rotation)
  {
    Object.Instantiate<GameObject>(this.objectToSpawn, position, rotation);
  }
}
