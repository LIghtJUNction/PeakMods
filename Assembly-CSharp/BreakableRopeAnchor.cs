// Decompiled with JetBrains decompiler
// Type: BreakableRopeAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BreakableRopeAnchor : MonoBehaviour
{
  public float breakAnimTime = 3f;
  public Vector2 breakableTimeMinMax = new Vector2(3f, 8f);
  public float dropSegments = 1f;
  private float willBreakInTime;
  private RopeAnchorWithRope anchor;
  private PhotonView photonView;
  private bool isBreaking;

  private void Awake()
  {
    this.anchor = this.GetComponent<RopeAnchorWithRope>();
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void Start() => this.willBreakInTime = this.breakableTimeMinMax.PRndRange();

  private void Update()
  {
    if (!this.photonView.IsMine)
      return;
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    int num = 0;
    foreach (Character character in playerCharacters)
    {
      if (character.data.isRopeClimbing && (Object) character.data.heldRope == (Object) this.anchor.rope)
        ++num;
    }
    if (num > 0)
      this.willBreakInTime -= Time.deltaTime;
    if ((double) this.willBreakInTime > 0.0 || this.isBreaking)
      return;
    this.StartCoroutine(Break());

    IEnumerator Break()
    {
      this.isBreaking = true;
      Debug.Log((object) $"Break: segments {this.anchor.rope.Segments}");
      this.anchor.rope.Segments += this.dropSegments;
      float elapsed = 0.0f;
      float startSegments = this.anchor.rope.Segments;
      while ((double) elapsed < (double) this.breakAnimTime)
      {
        elapsed += Time.deltaTime;
        this.anchor.rope.Segments = Mathf.Lerp(startSegments, startSegments + 1f, elapsed / 0.5f);
        yield return (object) null;
      }
      Debug.Log((object) "Detach_Rpc");
      this.anchor.rope.photonView.RPC("Detach_Rpc", RpcTarget.AllBuffered);
    }
  }
}
