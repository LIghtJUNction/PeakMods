// Decompiled with JetBrains decompiler
// Type: Basketball
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class Basketball : MonoBehaviour
{
  public Item item;
  public float xzBounceLoss = 0.5f;
  public float yFall = 5f;
  public Transform basketballMesh;
  public SFX_Instance impact;
  public float dribbleWait = 0.25f;
  public float dribbleWaitSprint = 0.05f;
  public float dribbleFloorOffset = 0.3f;
  public AnimationCurve dribbleCurve;
  public AnimationCurve handsPositionCurve;
  private bool dribbling;

  public void OnCollisionEnter(Collision collision)
  {
    if ((bool) (Object) collision.rigidbody || (double) Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, Vector3.up)) >= 0.20000000298023224)
      return;
    this.item.rig.linearVelocity = new Vector3(this.item.rig.linearVelocity.x * this.xzBounceLoss, this.item.rig.linearVelocity.y - this.yFall, this.item.rig.linearVelocity.z * this.xzBounceLoss);
  }

  public void Update()
  {
    if (this.item.itemState == ItemState.Held && (double) this.item.holderCharacter.input.movementInput.magnitude > 0.5 && this.item.holderCharacter.data.isGrounded && !this.item.holderCharacter.refs.items.isChargingThrow)
      this.dribbling = true;
    else
      this.dribbling = false;
  }

  private void Start() => this.StartCoroutine(this.DribbleRoutine());

  private IEnumerator DribbleRoutine()
  {
    while (true)
    {
      if (this.dribbling)
      {
        Vector3 endPosWorldSpace = Vector3.zero;
        RaycastHit raycastHit = HelperFunctions.LineCheck(this.basketballMesh.position, this.basketballMesh.position - Vector3.up * 100f, HelperFunctions.LayerType.AllPhysicalExceptCharacter, 0.1f);
        if ((Object) raycastHit.collider != (Object) null)
        {
          endPosWorldSpace = raycastHit.point;
          bool playedSFX = false;
          float t = 0.0f;
          float dribSpeed = Mathf.Clamp((this.item.holderCharacter.data.avarageVelocity with
          {
            y = 0.0f
          }).magnitude, 1f, 3f);
          while ((double) t < 1.0)
          {
            t += Time.deltaTime * dribSpeed;
            if ((double) t > 0.5 && !playedSFX)
            {
              this.impact.Play(this.basketballMesh.position);
              playedSFX = true;
            }
            endPosWorldSpace = new Vector3(this.basketballMesh.parent.position.x, endPosWorldSpace.y, this.basketballMesh.parent.position.z);
            this.basketballMesh.position = Vector3.Lerp(this.basketballMesh.parent.TransformPoint(Vector3.zero), endPosWorldSpace + Vector3.up * this.dribbleFloorOffset, this.dribbleCurve.Evaluate(t));
            this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.handsPositionCurve.Evaluate(t), this.item.defaultPos.z);
            yield return (object) null;
          }
        }
        yield return (object) new WaitForSeconds((double) (this.item.holderCharacter.data.avarageVelocity with
        {
          y = 0.0f
        }).magnitude > 3.0 ? this.dribbleWaitSprint : this.dribbleWait);
        endPosWorldSpace = new Vector3();
      }
      yield return (object) null;
    }
  }
}
