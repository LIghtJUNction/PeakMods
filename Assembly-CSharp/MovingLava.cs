// Decompiled with JetBrains decompiler
// Type: MovingLava
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class MovingLava : MonoBehaviour
{
  public float speed = 0.25f;
  public Animator rockAnim;
  private PhotonView view;
  private bool timeToMove;
  private float sinceSync;

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    if ((double) this.transform.position.y > 1150.0)
      return;
    if (!this.timeToMove)
    {
      if (!this.PlayersHaveMovedOn())
        return;
      this.view.RPC("RPCA_StartLavaRise", RpcTarget.All);
    }
    else
    {
      this.transform.position += Vector3.up * this.speed * Time.deltaTime;
      this.sinceSync += Time.deltaTime;
      if ((double) this.sinceSync <= 1.0)
        return;
      this.sinceSync = 0.0f;
      this.view.RPC("RPCA_SyncLavaHeight", RpcTarget.All, (object) this.transform.position.y);
    }
  }

  [PunRPC]
  public void RPCA_SyncLavaHeight(float height)
  {
    this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
  }

  [PunRPC]
  public void RPCA_StartLavaRise()
  {
    this.rockAnim.Play("RockDoor", 0, 0.0f);
    this.timeToMove = true;
    GamefeelHandler.instance.AddPerlinShake(3f, 2f, 10f);
    GamefeelHandler.instance.AddPerlinShake(15f, 0.3f);
  }

  private bool PlayersHaveMovedOn()
  {
    if (Character.AllCharacters.Count == 0)
      return false;
    float num = 879f;
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      if ((double) Character.AllCharacters[index].Center.y > (double) num)
        return true;
    }
    return false;
  }
}
