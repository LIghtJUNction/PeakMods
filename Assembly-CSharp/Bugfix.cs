// Decompiled with JetBrains decompiler
// Type: Bugfix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Bugfix : MonoBehaviour, IInteractible
{
  public Item bugItem;
  private Transform leg;
  private Vector3 localPos;
  private Vector3 forward;
  private Vector3 up;
  public float maxStatus = 0.5f;
  private float totalStatusApplied;
  private float lifeTime;
  private PhotonView photonView;
  private Character targetCharacter;
  public static Dictionary<Bugfix, Character> AllAttachedBugs = new Dictionary<Bugfix, Character>();
  private float counter;

  private void Start()
  {
    this.transform.localScale = Vector3.zero;
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void LateUpdate()
  {
    this.counter += Time.deltaTime;
    this.lifeTime += Time.deltaTime;
    if ((bool) (Object) this.targetCharacter && !this.targetCharacter.data.dead)
    {
      if (this.targetCharacter.IsLocal && (double) this.counter > 29.0)
      {
        this.targetCharacter.refs.afflictions.AddAffliction((Affliction) new Affliction_PreventPoisonHealing(30f));
        if ((double) this.totalStatusApplied < (double) this.maxStatus || (double) this.targetCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) < 0.5)
        {
          this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f);
          this.totalStatusApplied += 0.05f;
        }
        this.counter = 0.0f;
      }
      this.transform.position = this.leg.TransformPoint(this.localPos);
      this.transform.rotation = Quaternion.LookRotation(this.leg.TransformDirection(this.forward), this.leg.TransformDirection(this.up));
      this.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, this.lifeTime / 300f);
    }
    else
    {
      if (!this.photonView.IsMine)
        return;
      PhotonNetwork.Destroy(this.gameObject);
    }
  }

  [PunRPC]
  public void AttachBug(int targetID)
  {
    this.targetCharacter = PhotonView.Find(targetID).GetComponent<Character>();
    Rigidbody bodypartRig = this.targetCharacter.GetBodypartRig(BodypartType.Knee_R);
    this.leg = bodypartRig.transform;
    this.localPos = new Vector3(-0.27054f, 0.0f, -0.17134f);
    Vector3 vector3 = bodypartRig.transform.TransformPoint(this.localPos);
    Vector3 euler = new Vector3(0.0f, 55f, 0.0f);
    Quaternion quaternion = bodypartRig.transform.rotation * Quaternion.Euler(euler);
    this.transform.position = vector3;
    this.transform.rotation = quaternion;
    this.forward = this.leg.InverseTransformDirection(this.transform.forward);
    this.up = this.leg.InverseTransformDirection(this.transform.up);
    Bugfix.AllAttachedBugs.Add(this, this.targetCharacter);
  }

  private void OnDestroy() => Bugfix.AllAttachedBugs.Remove(this);

  public bool IsInteractible(Character interactor)
  {
    return (double) Vector3.Angle(this.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward) <= 2.0 + (Character.AllCharacters.Count == 1 ? 15.0 : 0.0) + (double) this.lifeTime / 60.0;
  }

  public void Interact(Character interactor)
  {
    GameUtils.instance.InstantiateAndGrab(this.bugItem, interactor);
    this.photonView.RPC("RPCA_Remove", RpcTarget.All);
  }

  [PunRPC]
  public void RPCA_Remove()
  {
    if (!this.photonView.IsMine)
      return;
    PhotonNetwork.Destroy(this.gameObject);
  }

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("TICKINTERACT");

  public string GetName() => LocalizedText.GetText("TICK");
}
