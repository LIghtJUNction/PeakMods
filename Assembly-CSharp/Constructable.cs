// Decompiled with JetBrains decompiler
// Type: Constructable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Constructable : ItemComponent
{
  public ConstructablePreview previewPrefab;
  public GameObject constructedPrefab;
  public float maxPreviewDistance;
  public float maxConstructDistance;
  public float maxConstructVerticalAngle;
  public bool angleToNormal;
  [SerializeField]
  public ConstructablePreview currentPreview;
  public float angleOffset;
  public bool isAngleable;
  protected RaycastHit currentConstructHit;
  protected bool constructing;
  private bool valid;

  public override void OnInstanceDataSet()
  {
  }

  protected virtual void Update()
  {
    if ((bool) (Object) this.item.holderCharacter && this.item.holderCharacter.IsLocal)
    {
      if (!this.constructing)
        this.TryUpdatePreview();
      else if (this.constructing && (double) Vector3.Distance(MainCamera.instance.transform.position, this.currentConstructHit.point) > (double) this.maxConstructDistance)
      {
        this.DestroyPreview();
        this.item.CancelUsePrimary();
      }
    }
    else
      this.DestroyPreview();
    if (!this.valid)
      this.item.overrideUsability = Optionable<bool>.Some(false);
    else
      this.item.overrideUsability = Optionable<bool>.None;
  }

  private void OnDestroy() => this.DestroyPreview();

  public virtual void TryUpdatePreview()
  {
    RaycastHit raycastHit = HelperFunctions.LineCheckIgnoreItem(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward.normalized * this.maxConstructDistance, HelperFunctions.LayerType.TerrainMap, this.item);
    this.currentConstructHit = raycastHit;
    this.valid = this.CurrentHitIsValid();
    if ((Object) raycastHit.collider == (Object) null)
      this.DestroyPreview();
    else
      this.CreateOrMovePreview();
  }

  private void OnDrawGizmosSelected()
  {
    if (!((Object) this.currentConstructHit.collider != (Object) null))
      return;
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(this.currentConstructHit.point, 0.5f);
  }

  private void CreateOrMovePreview()
  {
    if ((Object) this.currentPreview == (Object) null)
    {
      this.currentPreview = Object.Instantiate<ConstructablePreview>(this.previewPrefab);
      if (this.isAngleable)
        this.UpdateAngle();
    }
    this.currentPreview.transform.position = this.currentConstructHit.point;
    if (this.angleToNormal)
      this.currentPreview.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, this.currentConstructHit.normal).normalized, this.currentConstructHit.normal);
    else
      this.currentPreview.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, Vector3.up).normalized, Vector3.up);
    if (!this.currentPreview.CollisionValid())
      this.valid = false;
    this.currentPreview.SetValid(this.valid);
  }

  internal void DestroyPreview()
  {
    this.constructing = false;
    if (!((Object) this.currentPreview != (Object) null))
      return;
    Object.Destroy((Object) this.currentPreview.gameObject);
  }

  private bool CurrentHitIsValid()
  {
    return (double) this.currentConstructHit.distance <= (double) this.maxConstructDistance && ((double) this.maxConstructVerticalAngle <= 0.0 || (double) Vector3.Angle(Vector3.up, this.currentConstructHit.normal) <= (double) this.maxConstructVerticalAngle);
  }

  public virtual void StartConstruction()
  {
    if (!this.valid)
      return;
    this.constructing = true;
  }

  public virtual void FinishConstruction()
  {
    if (!this.constructing || (Object) this.currentPreview == (Object) null)
      return;
    if ((Object) this.constructedPrefab.GetComponent<PhotonView>() == (Object) null)
    {
      this.photonView.RPC("CreatePrefabRPC", RpcTarget.AllBuffered, (object) this.currentPreview.transform.position, (object) this.currentPreview.transform.rotation);
    }
    else
    {
      GameObject gameObject = PhotonNetwork.Instantiate(this.constructedPrefab.name, this.currentPreview.transform.position, this.currentPreview.transform.rotation);
      if (this.isAngleable)
        this.photonView.RPC("AngleIt", RpcTarget.AllBuffered, (object) gameObject.GetComponent<PhotonView>(), (object) this.angleOffset);
    }
    this.item.StartCoroutine(this.item.ConsumeDelayed());
    if (!this.item.holderCharacter.IsLocal)
      return;
    GameUtils.instance.IncrementPermanentItemsPlaced();
  }

  public void UpdateAngle()
  {
    if (!((Object) this.currentPreview != (Object) null))
      return;
    this.currentPreview.transform.GetChild(0).GetChild(0).localEulerAngles = new Vector3(this.angleOffset - 45f, 0.0f, 0.0f);
  }

  [PunRPC]
  protected void CreatePrefabRPC(Vector3 position, Quaternion rotation)
  {
    Object.Instantiate<GameObject>(this.constructedPrefab, position, rotation);
  }

  [PunRPC]
  protected void AngleIt(PhotonView view, float angle)
  {
    view.transform.GetChild(0).transform.localEulerAngles = new Vector3(angle - 45f, 0.0f, 0.0f);
  }
}
