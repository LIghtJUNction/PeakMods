// Decompiled with JetBrains decompiler
// Type: RopeTier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RopeTier : ItemComponent
{
  public GameObject anchorPreview;
  public GameObject anchorPrefab;
  public float maxAnchorGhostDistance = 10f;
  public float maxAnchorDistance = 5f;
  public float castTime;
  private RaycastHit? goodAnchorPlace;
  public float timeWithGoodAnchor;
  private new Item item;
  private RopeSpool spool;
  public RopeAnchor ropeAnchor;
  private PhotonView view;
  private bool releaseCheck;

  private new void Awake()
  {
    this.view = this.GetComponent<PhotonView>();
    this.item = this.GetComponent<Item>();
    this.spool = this.GetComponent<RopeSpool>();
  }

  public override void OnInstanceDataSet()
  {
  }

  public bool LookingToPlaceAnchor => (Object) this.ropeAnchor != (Object) null;

  private void OnDestroy()
  {
    if (!(bool) (Object) this.ropeAnchor)
      return;
    Object.DestroyImmediate((Object) this.ropeAnchor.gameObject);
  }

  public void Update()
  {
    if (!this.view.IsMine || this.item.itemState != ItemState.Held)
      return;
    if (this.releaseCheck)
    {
      if (!Character.localCharacter.input.usePrimaryWasReleased)
        return;
      this.releaseCheck = false;
    }
    else if (!Character.localCharacter.input.usePrimaryIsPressed)
    {
      this.item.overrideProgress = 0.0f;
      this.item.overrideForceProgress = false;
      if (!((Object) this.ropeAnchor != (Object) null))
        return;
      Object.DestroyImmediate((Object) this.ropeAnchor.gameObject);
    }
    else if ((Object) this.ropeAnchor != (Object) null && this.goodAnchorPlace.HasValue && (double) Vector3.Distance(this.goodAnchorPlace.Value.point, this.transform.position) > (double) this.maxAnchorGhostDistance)
    {
      this.item.overrideProgress = 0.0f;
      this.item.overrideForceProgress = false;
      Object.DestroyImmediate((Object) this.ropeAnchor.gameObject);
    }
    else
    {
      if ((Object) this.ropeAnchor == (Object) null)
      {
        this.ropeAnchor = Object.Instantiate<GameObject>(this.anchorPreview).GetComponent<RopeAnchor>();
        this.ropeAnchor.anchorPoint.gameObject.SetActive(false);
        this.goodAnchorPlace = new RaycastHit?();
        this.timeWithGoodAnchor = 0.0f;
      }
      if (!this.goodAnchorPlace.HasValue)
      {
        RaycastHit raycastHit = HelperFunctions.LineCheck(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * this.maxAnchorGhostDistance, HelperFunctions.LayerType.TerrainMap);
        Debug.DrawLine(Camera.main.transform.position, raycastHit.point, Color.red);
        if ((Object) raycastHit.collider == (Object) null)
          return;
        if ((Object) this.item == (Object) null)
          Debug.Log((object) "Item is null");
        if ((Object) this.item.holderCharacter == (Object) null)
          Debug.Log((object) "Item holder is null");
        double num = (double) Vector3.Distance(raycastHit.point, this.item.holderCharacter.Center);
        this.ropeAnchor.Ghost = true;
        this.ropeAnchor.transform.position = raycastHit.point;
        this.ropeAnchor.transform.forward = Vector3.Cross(Camera.main.transform.right, raycastHit.normal);
        this.ropeAnchor.transform.up = raycastHit.normal;
        double maxAnchorDistance = (double) this.maxAnchorDistance;
        if (num >= maxAnchorDistance)
          return;
        this.goodAnchorPlace = new RaycastHit?(raycastHit);
        this.ropeAnchor.Ghost = false;
      }
      else
      {
        this.item.overrideForceProgress = false;
        if (!this.goodAnchorPlace.HasValue)
          return;
        this.timeWithGoodAnchor += Time.deltaTime;
        this.item.overrideForceProgress = true;
        this.item.overrideProgress = this.timeWithGoodAnchor / this.castTime;
        if ((double) this.timeWithGoodAnchor < (double) this.castTime)
          return;
        Debug.Log((object) "Cast anchor");
        this.item.overrideForceProgress = false;
        this.item.overrideProgress = 0.0f;
        GameObject gameObject = PhotonNetwork.Instantiate(this.anchorPrefab.name, this.ropeAnchor.transform.position, this.ropeAnchor.transform.rotation);
        if (this.item.photonView.IsMine)
        {
          Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, this.spool.rope.GetLengthInMeters());
          GameUtils.instance.IncrementPermanentItemsPlaced();
        }
        this.spool.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, (object) gameObject.GetComponent<PhotonView>());
        Object.DestroyImmediate((Object) this.ropeAnchor.gameObject);
        this.releaseCheck = true;
        this.ropeAnchor = (RopeAnchor) null;
      }
    }
  }

  public override void OnDisable()
  {
    this.item.overrideForceProgress = false;
    this.item.overrideProgress = 0.0f;
    base.OnDisable();
  }
}
