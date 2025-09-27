// Decompiled with JetBrains decompiler
// Type: RopeAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class RopeAnchor : MonoBehaviour
{
  public GameObject ghostPart;
  public GameObject normalPart;
  public Transform anchorPoint;
  private bool isGhost;
  public PhotonView photonView;

  private void Awake() => this.photonView = this.GetComponent<PhotonView>();

  public bool Ghost
  {
    get => this.isGhost;
    set
    {
      this.isGhost = value;
      this.HideAll();
      if (this.isGhost)
        this.ghostPart.SetActive(true);
      else
        this.normalPart.SetActive(true);
    }
  }

  private void HideAll()
  {
    this.ghostPart.SetActive(false);
    this.normalPart.SetActive(false);
  }
}
