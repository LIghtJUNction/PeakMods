// Decompiled with JetBrains decompiler
// Type: OrbThatMakesYouSleepy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class OrbThatMakesYouSleepy : MonoBehaviour
{
  public Transform orb;
  public float orbRadius;
  public float maxDistance;
  public float minDrowsyPerSecond;
  public float maxDrowsyPerSecond;
  public float minDrowsyPerTick;
  public float maxDrowsyPerTick;
  private Vector3[] castPositions = new Vector3[5];
  public ParticleSystem particle;
  public Animator anim;
  public float animSpeed = 1f;
  public ParticleSystem ambientParticles;
  public FakeItem fakeBerry;
  private Plane[] planes = new Plane[6];

  private void Start() => this.anim.speed = this.animSpeed;

  public void Tick() => this.UpdateHypnosis();

  private void LateUpdate()
  {
    if ((Object) this.fakeBerry != (Object) null && this.fakeBerry.gameObject.activeInHierarchy)
    {
      this.anim.speed = this.animSpeed;
      this.ambientParticles.gameObject.SetActive(true);
      this.fakeBerry.transform.localPosition = new Vector3(-0.013f, -0.22f, 0.008f);
      this.fakeBerry.transform.localEulerAngles = Vector3.zero;
    }
    else
    {
      this.anim.speed = 0.0f;
      this.ambientParticles.gameObject.SetActive(false);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.magenta;
    Gizmos.DrawWireSphere(this.orb.transform.position, this.orbRadius);
  }

  private void UpdateCastPositions()
  {
    this.castPositions[0] = this.orb.transform.position;
    this.castPositions[1] = this.orb.transform.position + MainCamera.instance.cam.transform.right * this.orbRadius;
    this.castPositions[2] = this.orb.transform.position - MainCamera.instance.cam.transform.right * this.orbRadius;
    this.castPositions[3] = this.orb.transform.position + MainCamera.instance.cam.transform.up * this.orbRadius;
    this.castPositions[4] = this.orb.transform.position - MainCamera.instance.cam.transform.up * this.orbRadius;
  }

  private void DebugHypnosis() => this.UpdateHypnosis(true);

  private void UpdateHypnosis(bool debug = false)
  {
    if (!this.enabled || !(bool) (Object) this.fakeBerry || !this.fakeBerry.gameObject.activeInHierarchy || !Character.localCharacter.UnityObjectExists<Character>() || !Character.localCharacter.data.fullyConscious)
      return;
    Vector3 to = Character.localCharacter.Center - this.orb.transform.position;
    if (debug)
      Debug.Log((object) ("distance to character: " + to.magnitude.ToString()));
    if ((double) to.magnitude > (double) this.maxDistance)
      return;
    if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(MainCamera.instance.cam), new Bounds(this.orb.transform.position, Vector3.one * 0.5f)))
    {
      if (!debug)
        return;
      Debug.Log((object) "Not inside view frustum");
    }
    else
    {
      int num1 = 0;
      this.UpdateCastPositions();
      for (int index = 0; index < this.castPositions.Length; ++index)
      {
        if (debug)
          Debug.Log((object) $"testing cast {index}");
        Collider collider = HelperFunctions.LineCheck(this.castPositions[index], MainCamera.instance.cam.transform.position, HelperFunctions.LayerType.AllPhysical).collider;
        if ((Object) collider == (Object) null)
        {
          if (debug)
            Debug.Log((object) "Hit nothing");
          ++num1;
        }
        else if ((Object) collider.gameObject.GetComponentInParent<Character>() == (Object) Character.localCharacter)
        {
          if (debug)
            Debug.Log((object) "Hit our own character");
          ++num1;
        }
      }
      if (num1 == 0)
      {
        if (!debug)
          return;
        Debug.Log((object) "Blocked");
      }
      else
      {
        float num2 = Vector3.Angle(-MainCamera.instance.cam.transform.forward, to);
        float num3 = Mathf.InverseLerp(this.maxDistance, 2f, to.magnitude);
        if (debug)
          Debug.Log((object) $"factor 1: {num3}");
        float a = Mathf.Lerp(10f, 110f, num3);
        if (debug)
          Debug.Log((object) $"max angle: {a}");
        float b = Mathf.InverseLerp(a, a / 2f, num2);
        if (debug)
          Debug.Log((object) $"factor 2 {b}");
        float amount = Mathf.Lerp(this.minDrowsyPerTick, this.maxDrowsyPerTick, Mathf.Min(num3, b));
        if (num1 <= 2)
          amount *= 0.5f;
        if (debug)
          Debug.Log((object) $"Adding Status: {amount}");
        Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, amount);
        this.particle.Play();
      }
    }
  }
}
