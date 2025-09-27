// Decompiled with JetBrains decompiler
// Type: GodCam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class GodCam
{
  public float lookSens = 5f;
  public float lookDrag = 3f;
  public float force = 5f;
  public float drag = 3f;
  private Vector3 vel = Vector3.zero;
  private Vector2 lookData = Vector2.zero;
  private Vector2 lookVel = Vector2.zero;
  private bool isOrbiting;
  private Vector3 orbitingPoint;
  private Character orbitingCharacter;
  private float currentKeyMult = 1f;
  private float currentKeyMultTarget = 1f;
  private float sprintMult = 1f;
  private float targetFov = 70f;
  private float orbitinAmount;
  internal bool canOrbit = true;

  public void Update(Transform transform, MainCamera cam)
  {
    this.DoOrbiting(transform, cam);
    this.DoRotation(transform, cam);
    this.DoMovement(transform, cam);
    this.DoFOV(transform, cam);
    this.DoGamefeel(transform, cam);
  }

  private void DoOrbiting(Transform transform, MainCamera cam)
  {
    if (!this.isOrbiting)
    {
      if (Input.GetKey(KeyCode.Mouse0) && this.canOrbit)
      {
        Character orbitCharacter = this.GetOrbitCharacter(transform, cam);
        if ((bool) (UnityEngine.Object) orbitCharacter)
        {
          this.isOrbiting = true;
          this.orbitingCharacter = orbitCharacter;
          this.orbitingPoint = orbitCharacter.Center;
        }
        else
        {
          RaycastHit raycastHit = HelperFunctions.LineCheck(transform.position, transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical);
          if ((bool) (UnityEngine.Object) raycastHit.transform)
          {
            this.isOrbiting = true;
            this.orbitingCharacter = (Character) null;
            this.orbitingPoint = raycastHit.point;
          }
        }
      }
    }
    else if (!Input.GetKey(KeyCode.Mouse0))
      this.isOrbiting = false;
    this.orbitinAmount = !this.isOrbiting ? Mathf.Lerp(this.orbitinAmount, 0.0f, Time.unscaledDeltaTime * 2f) : Mathf.MoveTowards(this.orbitinAmount, 1f, Time.unscaledDeltaTime * Mathf.Lerp(this.orbitinAmount, 1f, 0.3f));
    if ((double) this.orbitinAmount <= 1.0 / 1000.0)
      return;
    if ((bool) (UnityEngine.Object) this.orbitingCharacter)
      this.orbitingPoint = this.orbitingCharacter.Center;
    Vector3 normalized = (this.orbitingPoint - transform.position).normalized;
    Vector3 dir = FRILerp.Lerp(transform.forward, normalized, 2f * this.orbitinAmount, false);
    this.lookVel = (Vector2) FRILerp.Lerp((Vector3) this.lookVel, (Vector3) Vector2.zero, 2f * this.orbitinAmount, false);
    this.lookData = (Vector2) HelperFunctions.DirectionToLook(dir);
  }

  private Character GetOrbitCharacter(Transform transform, MainCamera cam)
  {
    float num1 = 15f;
    Character orbitCharacter = (Character) null;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num2 = Vector3.Angle(allCharacter.Center - transform.position, transform.forward);
      if ((double) num2 < (double) num1 && (UnityEngine.Object) HelperFunctions.LineCheck(transform.position, allCharacter.Center, HelperFunctions.LayerType.TerrainMap).transform == (UnityEngine.Object) null)
      {
        num1 = num2;
        orbitCharacter = allCharacter;
      }
    }
    return orbitCharacter;
  }

  private void DoGamefeel(Transform transform, MainCamera cam)
  {
    transform.Rotate(GamefeelHandler.instance.GetRotation(), Space.World);
  }

  private void DoFOV(Transform transform, MainCamera cam)
  {
    this.targetFov += (float) (-(double) Input.mouseScrollDelta.y * 2.0) * Mathf.Lerp(cam.cam.fieldOfView / 70f, 1f, 0.25f);
    this.targetFov = Mathf.Clamp(this.targetFov, 1f, 120f);
    cam.cam.fieldOfView = Mathf.Lerp(cam.cam.fieldOfView, this.targetFov, Time.unscaledDeltaTime * 5f);
  }

  private void DoMovement(Transform transform, MainCamera cam)
  {
    this.currentKeyMult = Mathf.Lerp(this.currentKeyMult, this.currentKeyMultTarget, Time.unscaledDeltaTime * 2f);
    this.sprintMult = !Input.GetKey(KeyCode.LeftShift) ? Mathf.Lerp(this.sprintMult, 1f, Time.unscaledDeltaTime * 2f) : Mathf.Lerp(this.sprintMult, 10f, Time.unscaledDeltaTime * 2f);
    Vector3 zero = Vector3.zero;
    if (Input.GetKey(KeyCode.W))
      ++zero.z;
    if (Input.GetKey(KeyCode.S))
      --zero.z;
    if (Input.GetKey(KeyCode.A))
      --zero.x;
    if (Input.GetKey(KeyCode.D))
      ++zero.x;
    if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
      ++zero.y;
    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Q))
      --zero.y;
    this.vel += transform.TransformDirection(new Vector3(zero.x, zero.y, zero.z)) * this.force * this.sprintMult * this.currentKeyMult * Time.unscaledDeltaTime;
    this.vel = FRILerp.Lerp(this.vel, Vector3.zero, this.drag, false);
    transform.position += this.vel * Time.unscaledDeltaTime;
  }

  private void DoRotation(Transform transform, MainCamera cam)
  {
    float num = cam.cam.fieldOfView / 70f;
    this.lookVel += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 0.1f * this.lookSens * num;
    this.lookVel = (Vector2) FRILerp.Lerp((Vector3) this.lookVel, (Vector3) Vector2.zero, this.lookDrag, false);
    this.lookData += this.lookVel * Time.unscaledDeltaTime;
    transform.rotation = Quaternion.LookRotation(HelperFunctions.LookToDirection(new Vector3(this.lookData.x, this.lookData.y, 0.0f), Vector3.forward));
  }
}
