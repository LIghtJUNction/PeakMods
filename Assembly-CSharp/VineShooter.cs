// Decompiled with JetBrains decompiler
// Type: VineShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class VineShooter : ItemComponent
{
  public GameObject vinePrefab;
  public GameObject disableOnFire;
  public float maxLength = 40f;
  private Camera camera;
  private Action_ReduceUses actionReduceUses;

  public override void Awake()
  {
    this.actionReduceUses = this.GetComponent<Action_ReduceUses>();
    this.camera = Camera.main;
    base.Awake();
    this.item.OnPrimaryFinishedCast += new Action(this.OnPrimaryFinishedCast);
  }

  private void OnDestroy()
  {
    this.item.OnPrimaryFinishedCast -= new Action(this.OnPrimaryFinishedCast);
  }

  public void Update()
  {
    this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out RaycastHit _));
  }

  private void OnPrimaryFinishedCast()
  {
    Debug.Log((object) "VineShooter shoot");
    RaycastHit hit;
    if (!this.WillAttach(out hit))
      return;
    if ((UnityEngine.Object) this.disableOnFire != (UnityEngine.Object) null)
      this.disableOnFire.SetActive(false);
    JungleVine component1 = this.vinePrefab.GetComponent<JungleVine>();
    Vector2 a = new Vector2(component1.minDown, component1.maxDown);
    int num = 10;
    for (int index = 0; index < num; ++index)
    {
      float hang = -Vector2.Lerp(a, Vector2.zero, (float) index / ((float) num - 1f)).PRndRange();
      Vector3 origin = this.camera.transform.position + this.camera.transform.forward * 1f;
      Vector3 vector3 = this.camera.transform.position - Vector3.up * 0.2f;
      Vector3 down = Vector3.down;
      RaycastHit raycastHit;
      ref RaycastHit local = ref raycastHit;
      int layerMask = (int) HelperFunctions.LayerType.TerrainMap.ToLayerMask();
      if (Physics.Raycast(origin, down, out local, 4f, layerMask, QueryTriggerInteraction.UseGlobal))
        vector3 = raycastHit.point + Vector3.up * 1.5f;
      Debug.Log((object) $"from: {vector3}, to: {hit.point}, hang: {hang}");
      Vector3 mid;
      if (JungleVine.CheckVinePath(vector3, hit.point, hang, out mid))
      {
        JungleVine component2 = PhotonNetwork.Instantiate(this.vinePrefab.name, vector3, Quaternion.identity).GetComponent<JungleVine>();
        component2.photonView.RPC("ForceBuildVine_RPC", RpcTarget.AllBuffered, (object) vector3, (object) hit.point, (object) hang, (object) mid);
        component2.TryGetComponent<SpawnedVine>(out SpawnedVine _);
        this.actionReduceUses.RunAction();
        component1.SetRendererBounds();
        Debug.DrawLine(vector3, hit.point, Color.green, 5f);
        GameUtils.instance.IncrementPermanentItemsPlaced();
        break;
      }
      Debug.DrawLine(vector3, hit.point, Color.red, 5f);
    }
  }

  public bool WillAttach(out RaycastHit hit)
  {
    hit = new RaycastHit();
    if (!Character.localCharacter.data.isGrounded || !Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, (int) HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal))
      return false;
    Vector3 up1 = Vector3.up;
    Vector3 vector3 = hit.point - MainCamera.instance.transform.position;
    Vector3 normalized1 = vector3.normalized;
    float num = Vector3.Dot(up1, normalized1);
    if ((double) num > 0.60000002384185791 || (double) num < -0.699999988079071)
      return false;
    Vector3 up2 = Vector3.up;
    vector3 = hit.point - MainCamera.instance.transform.position;
    Vector3 normalized2 = vector3.normalized;
    return (double) Vector3.Dot(up2, normalized2) <= 0.5;
  }

  public override void OnInstanceDataSet()
  {
  }
}
