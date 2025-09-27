// Decompiled with JetBrains decompiler
// Type: BingBongPhysics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(1000001)]
public class BingBongPhysics : MonoBehaviour
{
  public BingBongPhysics.PhysicsType physicsType;
  private PhotonView view;
  private BingBongPowers bingBongPowers;
  private string descr = "Blow: [R]\n\nSuck: [T]\n\nForce Grab: [F]\n\nForce Push: [C]\n\nForce Push Gentle: [V]";
  private float counter;
  public GameObject effect_Blow;
  public GameObject effect_Suck;
  public GameObject effect_Push;
  public GameObject effect_Push_Gentle;
  public GameObject effect_Grab;

  private void OnEnable()
  {
    this.bingBongPowers = this.GetComponent<BingBongPowers>();
    this.bingBongPowers.SetTexts("PHYSICS", this.descr);
  }

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    this.CheckInuput();
    float cd = this.GetCD();
    bool auto = this.GetAuto();
    this.counter += Time.unscaledDeltaTime;
    if ((double) this.counter < (double) cd || auto && !Input.GetKey(KeyCode.Mouse0) || !auto && !Input.GetKeyDown(KeyCode.Mouse0))
      return;
    this.DoEffect();
    this.counter = 0.0f;
  }

  private void DoEffect()
  {
    PhotonNetwork.Instantiate(this.GetEffect().name, this.transform.position, this.transform.rotation).GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.All, (object) this.view.ViewID);
  }

  private GameObject GetEffect()
  {
    if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
      return this.effect_Blow;
    if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
      return this.effect_Suck;
    if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
      return this.effect_Push;
    if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
      return this.effect_Push_Gentle;
    return this.physicsType == BingBongPhysics.PhysicsType.ForceGrab ? this.effect_Grab : (GameObject) null;
  }

  private bool GetAuto()
  {
    if (this.physicsType == BingBongPhysics.PhysicsType.Blow || this.physicsType == BingBongPhysics.PhysicsType.Suck)
      return true;
    if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush || this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
      return false;
    int physicsType = (int) this.physicsType;
    return true;
  }

  private float GetCD()
  {
    if (this.physicsType == BingBongPhysics.PhysicsType.Blow || this.physicsType == BingBongPhysics.PhysicsType.Suck)
      return 0.25f;
    if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush || this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
      return 0.0f;
    int physicsType = (int) this.physicsType;
    return 0.25f;
  }

  private void CheckInuput()
  {
    if (Input.GetKeyDown(KeyCode.R))
      this.SetState(BingBongPhysics.PhysicsType.Blow);
    if (Input.GetKeyDown(KeyCode.T))
      this.SetState(BingBongPhysics.PhysicsType.Suck);
    if (Input.GetKeyDown(KeyCode.F))
      this.SetState(BingBongPhysics.PhysicsType.ForceGrab);
    if (Input.GetKeyDown(KeyCode.C))
      this.SetState(BingBongPhysics.PhysicsType.ForcePush);
    if (!Input.GetKeyDown(KeyCode.V))
      return;
    this.SetState(BingBongPhysics.PhysicsType.ForcePush_Gentle);
  }

  private void SetState(BingBongPhysics.PhysicsType setType)
  {
    this.physicsType = setType;
    this.bingBongPowers.SetTip(setType.ToString(), 0);
  }

  public enum PhysicsType
  {
    Blow,
    Suck,
    ForcePush,
    ForcePush_Gentle,
    ForceGrab,
  }
}
