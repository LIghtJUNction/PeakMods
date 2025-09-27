// Decompiled with JetBrains decompiler
// Type: RopeSegment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RopeSegment : MonoBehaviour, IInteractible
{
  public Rope rope;
  public float angle;
  public string displayName;

  private void Awake() => this.rope = this.GetComponentInParent<Rope>();

  private void Update() => this.angle = this.GetAngle();

  public Vector3 Center() => this.transform.position;

  public string GetInteractionText() => LocalizedText.GetText("GRAB");

  public string GetName() => LocalizedText.GetText(this.displayName);

  public Transform GetTransform() => this.transform;

  public void Interact(Character interactor)
  {
    interactor.refs.items.EquipSlot(Optionable<byte>.None);
    int num = this.transform.GetSiblingIndex() - 2;
    Debug.Log((object) $"Grabbing Rope: {this.gameObject.name} with index {num}");
    interactor.GetComponent<PhotonView>().RPC("GrabRopeRpc", RpcTarget.All, (object) this.rope.GetComponentInParent<PhotonView>(), (object) num);
  }

  public bool IsInteractible(Character interactor)
  {
    float angle = this.GetAngle();
    bool flag = ((double) angle < (double) interactor.refs.ropeHandling.maxRopeAngle * 0.60000002384185791 || 180.0 - (double) angle < (double) interactor.refs.ropeHandling.maxRopeAngle * 0.60000002384185791) && this.rope.isClimbable;
    if (interactor.data.isRopeClimbing)
      flag = flag && (Object) interactor.data.heldRope != (Object) this.rope;
    return flag;
  }

  public float GetAngle() => Vector3.Angle(Vector3.up, this.transform.up);

  internal Vector3 GetClimbNormal(Vector3 charPos)
  {
    return this.transform.InverseTransformDirection(Vector3.ProjectOnPlane(charPos - this.transform.position, this.transform.up));
  }

  internal void Tie(Vector3 tiePos)
  {
    ConfigurableJoint joint = this.gameObject.AddComponent<ConfigurableJoint>();
    joint.xMotion = ConfigurableJointMotion.Locked;
    joint.yMotion = ConfigurableJointMotion.Locked;
    joint.zMotion = ConfigurableJointMotion.Locked;
    joint.anchor = Vector3.zero;
    this.StartCoroutine(ITighten(tiePos));

    IEnumerator ITighten(Vector3 tiePos)
    {
      float c = 0.0f;
      Vector3 start = this.transform.position;
      while ((double) c < 1.0)
      {
        c += Time.fixedDeltaTime * 3f;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.Lerp(start, tiePos, c);
        yield return (object) new WaitForFixedUpdate();
      }
    }
  }

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }
}
