// Decompiled with JetBrains decompiler
// Type: RescueHook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RescueHook : ItemComponent
{
  public Transform dragPoint;
  public GameObject disableOnFire;
  public float maxLength = 40f;
  public float dragForce = 100f;
  public float launchForce = 10f;
  private Action_ReduceUses actionReduceUses;
  private Camera camera;
  private float currentDistance;
  private bool fly;
  private bool hitNothing;
  private bool isPulling;
  private float sinceFire;
  private Character targetPlayer;
  private Vector3 targetPos;
  private Rigidbody targetRig;
  public RopeRender ropeRender;
  public LineRenderer line;
  public Transform firePoint;
  public float range = 30f;

  public Character playerHoldingItem => this.item.holderCharacter;

  public override void Awake()
  {
    this.actionReduceUses = this.GetComponent<Action_ReduceUses>();
    this.camera = Camera.main;
    base.Awake();
    this.item.OnPrimaryFinishedCast += new Action(this.OnPrimaryFinishedCast);
    this.line.positionCount = 40;
  }

  public void Update()
  {
    this.item.overrideUsability = Optionable<bool>.Some((bool) (UnityEngine.Object) this.GetHit(out Vector3 _).transform);
  }

  private void FixedUpdate()
  {
    this.sinceFire += Time.fixedDeltaTime;
    if (this.isPulling)
    {
      if ((bool) (UnityEngine.Object) this.targetPlayer)
        this.targetPos = this.targetRig.position;
      if ((double) this.sinceFire > 0.25)
      {
        if ((bool) (UnityEngine.Object) this.targetPlayer)
        {
          Vector3 vector3 = this.transform.position - this.targetPlayer.Center;
          Vector3 normalized = vector3.normalized;
          Debug.Log((object) $"IsPulling Player, force: {this.dragForce}");
          this.targetPlayer.refs.movement.ApplyExtraDrag(0.95f, true);
          this.targetPlayer.AddForceToBodyPart(this.targetRig, normalized * (this.dragForce * 0.2f), normalized * this.dragForce);
          this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0.0f, 1f);
          Rigidbody rig = this.item.rig;
          vector3 = this.targetRig.position - this.item.rig.position;
          Vector3 force = vector3.normalized * (this.dragForce * 0.3f);
          rig.AddForce(force, ForceMode.Acceleration);
          if ((double) Vector3.Distance(this.dragPoint.position, this.targetRig.position) < 2.0 || (double) this.sinceFire > 2.0)
          {
            if (this.photonView.IsMine)
              this.photonView.RPC("RPCA_LetGo", RpcTarget.All);
            else
              this.RPCA_LetGo();
          }
          else if (!this.photonView.AmOwner)
            ;
        }
        else
        {
          Debug.Log((object) $"IsPulling Shooter, hitNothing: {this.hitNothing}, fallSeconds: {this.playerHoldingItem.data.fallSeconds}");
          if ((double) this.playerHoldingItem.data.fallSeconds <= 0.0 && !this.hitNothing)
          {
            Vector3 normalized = (this.targetPos - this.playerHoldingItem.Center).normalized;
            Debug.Log((object) $"IsPulling Shooter, force: {this.launchForce}");
            this.fly = true;
            this.playerHoldingItem.Fall(2f, 15f);
            this.playerHoldingItem.AddForce(normalized * this.launchForce);
            this.playerHoldingItem.AddForceToBodyPart(this.playerHoldingItem.GetBodypart(BodypartType.Hand_R).Rig, normalized * this.launchForce * 1f, Vector3.zero);
            this.playerHoldingItem.AddForceToBodyPart(this.playerHoldingItem.GetBodypart(BodypartType.Torso).Rig, normalized * this.launchForce * 1f, Vector3.zero);
          }
          if ((double) this.sinceFire > 0.5)
            this.RPCA_LetGo();
        }
      }
      this.ropeRender.DisplayRope(this.dragPoint.position, this.targetPos, this.sinceFire, this.line);
      this.currentDistance = Vector3.Distance(this.dragPoint.position, this.targetPos);
    }
    else
    {
      this.fly = false;
      this.StopDisplayRope();
    }
  }

  private void OnDestroy()
  {
    this.item.OnPrimaryFinishedCast -= new Action(this.OnPrimaryFinishedCast);
  }

  private void StopDisplayRope() => this.line.enabled = false;

  private void Fire()
  {
    Debug.Log((object) nameof (Fire));
    this.sinceFire = 0.0f;
    Vector3 endFire;
    RaycastHit hit = this.GetHit(out endFire);
    if ((bool) (UnityEngine.Object) hit.transform)
    {
      Character componentInParent = hit.transform.GetComponentInParent<Character>();
      Debug.Log((object) $"Hit: {hit.collider.name} Rig: {hit.rigidbody}, !hit.rigidbody: {!(bool) (UnityEngine.Object) hit.rigidbody}", (UnityEngine.Object) hit.collider.gameObject);
      if ((bool) (UnityEngine.Object) componentInParent)
        this.photonView.RPC("RPCA_RescueCharacter", RpcTarget.All, (object) componentInParent.photonView);
      else
        this.photonView.RPC("RPCA_RescueWall", RpcTarget.All, (object) false, (object) hit.point);
    }
    else
      this.photonView.RPC("RPCA_RescueWall", RpcTarget.All, (object) true, (object) endFire);
  }

  private RaycastHit GetHit(out Vector3 endFire)
  {
    Ray middleScreenRay = PExt.GetMiddleScreenRay();
    List<RaycastHit> list = ((IEnumerable<RaycastHit>) Physics.RaycastAll(middleScreenRay, this.range, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysicalExceptDefault))).ToList<RaycastHit>();
    endFire = middleScreenRay.origin + middleScreenRay.direction * this.range;
    RaycastHit hit = new RaycastHit();
    list.Reverse();
    foreach (RaycastHit raycastHit in list)
    {
      if ((UnityEngine.Object) raycastHit.rigidbody != (UnityEngine.Object) null)
      {
        Item component = raycastHit.rigidbody.GetComponent<Item>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.holderCharacter == (UnityEngine.Object) this.item.holderCharacter)
          continue;
      }
      hit = raycastHit;
      break;
    }
    Debug.DrawLine(middleScreenRay.origin, middleScreenRay.origin + middleScreenRay.direction * this.range, (bool) (UnityEngine.Object) hit.transform ? Color.green : Color.red, 10f);
    return hit;
  }

  [PunRPC]
  public void RPCA_RescueItem(PhotonView objectView)
  {
  }

  [PunRPC]
  public void RPCA_RescueCharacter(PhotonView characterView)
  {
    Debug.Log((object) nameof (RPCA_RescueCharacter));
    Character component = characterView.GetComponent<Character>();
    this.targetPlayer = component;
    this.targetRig = component.GetBodypart(BodypartType.Torso).Rig;
    this.sinceFire = 0.0f;
    if (this.targetPlayer.photonView.IsMine || this.photonView.IsMine)
      GamefeelHandler.instance.perlin.AddShake(this.transform.position, 5f, range: 40f);
    this.isPulling = true;
    this.targetPlayer.data.fallSeconds = 2f;
  }

  [PunRPC]
  private void RPCA_LetGo()
  {
    Debug.Log((object) nameof (RPCA_LetGo));
    this.targetRig = (Rigidbody) null;
    this.targetPlayer = (Character) null;
    this.isPulling = false;
  }

  [PunRPC]
  public void RPCA_RescueWall(bool hitNothing, Vector3 targetPos)
  {
    Debug.Log((object) $"RPCA_RescueWall, hitnothing:{hitNothing}");
    this.targetPos = targetPos;
    this.hitNothing = hitNothing;
    this.sinceFire = 0.0f;
    GamefeelHandler.instance.perlin.AddShake(this.transform.position, 5f, range: 40f);
    this.isPulling = true;
  }

  private void OnPrimaryFinishedCast()
  {
    Debug.Log((object) "RescueHook shoot");
    this.Fire();
  }

  public bool WillAttach(out RaycastHit hit)
  {
    hit = new RaycastHit();
    return Character.localCharacter.data.isGrounded && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, (int) HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
  }

  public override void OnInstanceDataSet()
  {
  }
}
