// Decompiled with JetBrains decompiler
// Type: ScoutCannon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ScoutCannon : MonoBehaviour
{
  public float launchForce = 500f;
  public float itemLaunchForce = 500f;
  public float fallFor = 1f;
  public float pullForce = 10f;
  public float pushForce = 10f;
  public bool lit;
  public float fireTime = 3f;
  public ParticleSystem litParticle;
  public ParticleSystem fireParticle;
  public GameObject fireSFX;
  public Animator anim;
  private MaterialPropertyBlock mpb;
  private PhotonView view;
  private Transform tube;
  private Transform entry;
  private Character target;
  private int targetID = -1;
  private MeshRenderer[] _mr;

  public bool holdOnFinish => false;

  private void FixedUpdate()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    float num1 = 1f;
    float num2 = num1;
    Character character = (Character) null;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num3 = Vector3.Distance(allCharacter.Center, this.entry.position);
      if ((double) num3 < (double) num2)
      {
        num2 = num3;
        character = allCharacter;
      }
      if ((double) num3 < (double) num1)
      {
        if ((Object) allCharacter == (Object) this.target)
        {
          if ((double) allCharacter.data.sinceJump >= 0.30000001192092896)
          {
            foreach (Bodypart part in allCharacter.refs.ragdoll.partList)
            {
              switch (part.partType)
              {
                case BodypartType.Arm_L:
                case BodypartType.Elbow_L:
                case BodypartType.Hand_L:
                case BodypartType.Arm_R:
                case BodypartType.Elbow_R:
                case BodypartType.Hand_R:
                  continue;
                default:
                  Vector3 vector3 = Vector3.ClampMagnitude(this.tube.position + Vector3.Project(part.transform.position - this.tube.position, this.tube.forward) - part.transform.position, 1f);
                  part.Rig.AddForce(vector3 * this.pullForce, ForceMode.Acceleration);
                  if ((double) this.entry.InverseTransformPoint(part.transform.position).z < 0.0 && (bool) (Object) HelperFunctions.LineCheck(this.entry.position, part.transform.position, HelperFunctions.LayerType.Map).transform)
                  {
                    Vector3 forward = this.entry.forward;
                    part.Rig.AddForce(forward * this.pullForce, ForceMode.Acceleration);
                    continue;
                  }
                  continue;
              }
            }
          }
        }
        else if ((Object) HelperFunctions.LineCheck(this.tube.position, allCharacter.Center, HelperFunctions.LayerType.Map).transform == (Object) null)
        {
          Vector3 vector3 = this.tube.position - allCharacter.Center;
          vector3.Normalize();
          allCharacter.AddForce(-vector3 * this.pushForce);
        }
      }
    }
    if (!((Object) this.target != (Object) character))
      return;
    if ((Object) character == (Object) null)
      this.view.RPC("RPCA_SetTarget", RpcTarget.All, (object) -1);
    else
      this.view.RPC("RPCA_SetTarget", RpcTarget.All, (object) character.refs.view.ViewID);
  }

  [PunRPC]
  private void RPCA_SetTarget(int setTargetID)
  {
    this.targetID = setTargetID;
    if (this.targetID == -1)
      this.target = (Character) null;
    else
      this.target = PhotonNetwork.GetPhotonView(this.targetID).GetComponent<Character>();
  }

  private void Awake()
  {
    this.tube = this.transform.Find("Cannon");
    this.entry = this.tube.Find("Entry");
    this.view = this.GetComponent<PhotonView>();
  }

  private MeshRenderer[] meshRenderers
  {
    get
    {
      if (this._mr == null)
        this._mr = this.tube.GetComponentsInChildren<MeshRenderer>();
      return this._mr;
    }
    set => this._mr = value;
  }

  public void CancelCast(Character interactor)
  {
  }

  public void Light() => this.view.RPC("RPCA_Light", RpcTarget.All);

  [PunRPC]
  public void RPCA_Light()
  {
    this.StartCoroutine(LightRoutine());

    IEnumerator LightRoutine()
    {
      this.lit = true;
      this.litParticle.Play();
      this.anim.Play("Light");
      yield return (object) new WaitForSeconds(this.fireTime);
      this.anim.Play("Fire");
      this.fireParticle.Play();
      this.fireSFX.SetActive(true);
      if (this.view.IsMine)
        this.FireTargets();
      yield return (object) new WaitForSeconds(this.fallFor);
      this.lit = false;
    }
  }
  private void FireTargets()
  {
    this.LaunchPlayers();
    this.LaunchItems();
  }
  private void LaunchPlayers()
  {
    List<Character> characterList = new List<Character>();
    if ((bool) (Object) this.target)
      characterList.Add(this.target);
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((double) Vector3.Distance(allCharacter.Center, this.entry.position) <= 0.75 && !((Object) allCharacter == (Object) this.target))
        characterList.Add(allCharacter);
    }
    foreach (Character character in characterList)
      this.view.RPC("RPCA_LaunchTarget", RpcTarget.All, (object) character.refs.view.ViewID);
  }

  private void LaunchItems()
  {
    Collider[] colliderArray = Physics.OverlapSphere(this.tube.position, 1f, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical));
    List<Item> objList = new List<Item>();
    foreach (Component component in colliderArray)
    {
      Item componentInParent = component.GetComponentInParent<Item>();
      if ((bool) (Object) componentInParent && componentInParent.itemState == ItemState.Ground && !(bool) (Object) HelperFunctions.LineCheck(this.tube.position, componentInParent.Center(), HelperFunctions.LayerType.Map).transform && !objList.Contains(componentInParent))
        objList.Add(componentInParent);
    }
    foreach (Item obj in objList)
      this.view.RPC("RPCA_LaunchItem", RpcTarget.All, (object) obj.photonView.ViewID);
  }

  [PunRPC]
  public void RPCA_LaunchItem(int targetID)
  {
    PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
    if ((Object) photonView == (Object) null)
      return;
    Item component = photonView.GetComponent<Item>();
    if ((Object) component == (Object) null)
      return;
    component.rig.AddForce(this.tube.forward * this.itemLaunchForce, ForceMode.VelocityChange);
  }

  [PunRPC]
  public void RPCA_LaunchTarget(int targetID)
  {
    PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
    if ((Object) photonView == (Object) null)
      return;
    Character t = photonView.GetComponent<Character>();
    if ((Object) t == (Object) null)
      return;
    t.data.launchedByCannon = true;
    t.RPCA_Fall(this.fallFor);
    t.AddForce(this.tube.forward * this.launchForce);
    this.StartCoroutine(ILaunch());

    IEnumerator ILaunch()
    {
      float c = 0.0f;
      while ((double) c < (double) this.fallFor)
      {
        c += Time.deltaTime;
        if ((bool) (Object) t)
          t.ClampSinceGrounded(1f);
        yield return (object) null;
      }
      if ((bool) (Object) t)
        t.data.fallSeconds = 0.0f;
      c = 0.0f;
      while ((double) c < 4.0)
      {
        c += Time.deltaTime;
        if ((bool) (Object) t)
          t.ClampSinceGrounded(1f);
        yield return (object) null;
      }
    }
  }
}
