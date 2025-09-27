// Decompiled with JetBrains decompiler
// Type: SlipperyJellyfish
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class SlipperyJellyfish : MonoBehaviour
{
  private float counter = 2.5f;
  private TriggerRelay relay;
  public SFX_Instance[] slipSFX;

  private void Start() => this.relay = this.GetComponentInParent<TriggerRelay>();

  private void Update() => this.counter += Time.deltaTime;

  public void OnTriggerEnter(Collider other)
  {
    if ((double) this.counter < 3.0)
      return;
    Character componentInParent = other.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || !componentInParent.IsLocal)
      return;
    this.counter = 0.0f;
    this.relay.view.RPC("RPCA_TriggerWithTarget", RpcTarget.All, (object) this.transform.GetSiblingIndex(), (object) Character.localCharacter.refs.view.ViewID);
  }

  public void Trigger(int targetID)
  {
    Character component = PhotonView.Find(targetID).GetComponent<Character>();
    if ((Object) component == (Object) null)
      return;
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
    component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, true);
    for (int index = 0; index < this.slipSFX.Length; ++index)
      this.slipSFX[index].Play(this.transform.position);
  }
}
