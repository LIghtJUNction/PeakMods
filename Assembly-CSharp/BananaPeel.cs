// Decompiled with JetBrains decompiler
// Type: BananaPeel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BananaPeel : MonoBehaviour
{
  private float counter = 2.5f;
  private Item item;
  public SFX_Instance[] slipSFX;

  private void Start() => this.item = this.GetComponent<Item>();

  private void Update()
  {
    if (this.item.itemState != ItemState.Ground)
      return;
    this.counter += Time.deltaTime;
    if ((double) this.counter < 3.0 || (double) Vector3.Distance(Character.localCharacter.Center, this.transform.position) > 1.0 || !Character.localCharacter.data.isGrounded || (double) Character.localCharacter.data.avarageVelocity.magnitude < 1.5)
      return;
    this.counter = 0.0f;
    this.GetComponent<PhotonView>().RPC("RPCA_TriggerBanana", RpcTarget.All, (object) Character.localCharacter.refs.view.ViewID);
  }

  [PunRPC]
  public void RPCA_TriggerBanana(int viewID)
  {
    Character component = PhotonView.Find(viewID).GetComponent<Character>();
    if ((Object) component == (Object) null)
      return;
    this.GetComponent<Rigidbody>().AddForce((component.data.lookDirection_Flat * 0.5f + Vector3.up) * 40f, ForceMode.Impulse);
    Rigidbody bodypartRig1 = component.GetBodypartRig(BodypartType.Foot_R);
    Rigidbody bodypartRig2 = component.GetBodypartRig(BodypartType.Foot_L);
    Rigidbody bodypartRig3 = component.GetBodypartRig(BodypartType.Hip);
    Rigidbody bodypartRig4 = component.GetBodypartRig(BodypartType.Head);
    component.RPCA_Fall(2f);
    Vector3 force = (component.data.lookDirection_Flat + Vector3.up) * 200f;
    bodypartRig1.AddForce(force, ForceMode.Impulse);
    bodypartRig2.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, ForceMode.Impulse);
    bodypartRig3.AddForce(Vector3.up * 1500f, ForceMode.Impulse);
    bodypartRig4.AddForce(component.data.lookDirection_Flat * -300f, ForceMode.Impulse);
    for (int index = 0; index < this.slipSFX.Length; ++index)
      this.slipSFX[index].Play(this.transform.position);
  }
}
