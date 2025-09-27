// Decompiled with JetBrains decompiler
// Type: BotMoverRagdoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BotMoverRagdoll : MonoBehaviour
{
  private Bot bot;
  public float movementSpeed;
  private Rigidbody rig_g;
  private Vector3 angularVel;
  public float rotSpring = 15f;
  public float rotDamp = 35f;

  private void Awake()
  {
    this.bot = this.GetComponent<Bot>();
    this.rig_g = this.GetComponent<Rigidbody>();
  }

  private void Start()
  {
  }

  private void FixedUpdate()
  {
    float fixedDeltaTime = Time.fixedDeltaTime;
    this.rig_g.AddForce(this.transform.forward * (this.bot.MovementInput.y * (this.movementSpeed * fixedDeltaTime)), ForceMode.Acceleration);
    Vector3 up = Vector3.up;
    Vector3 lookDirection = this.bot.LookDirection;
    Vector3 vector3 = Vector3.Cross(this.transform.up, up).normalized * Vector3.Angle(this.transform.up, up);
    this.rig_g.angularVelocity = FRILerp.PLerp(this.rig_g.angularVelocity, (Vector3.Cross(this.transform.forward, lookDirection).normalized * Vector3.Angle(this.transform.forward, lookDirection) + vector3) * this.rotSpring, this.rotDamp, fixedDeltaTime);
  }
}
