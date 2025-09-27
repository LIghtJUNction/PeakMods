// Decompiled with JetBrains decompiler
// Type: ChangeColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (Renderer))]
[RequireComponent(typeof (PhotonView))]
public class ChangeColor : MonoBehaviour
{
  private PhotonView photonView;

  private void Start()
  {
    this.photonView = this.GetComponent<PhotonView>();
    if (!this.photonView.IsMine)
      return;
    Color color = Random.ColorHSV();
    this.photonView.RPC("ChangeColour", RpcTarget.AllBuffered, (object) new Vector3(color.r, color.g, color.b));
  }

  [PunRPC]
  private void ChangeColour(Vector3 randomColor)
  {
    this.GetComponent<Renderer>().material.SetColor("_Color", new Color(randomColor.x, randomColor.y, randomColor.z));
  }
}
