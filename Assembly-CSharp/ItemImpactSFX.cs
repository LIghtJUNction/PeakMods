// Decompiled with JetBrains decompiler
// Type: ItemImpactSFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ItemImpactSFX : MonoBehaviour
{
  public float vel;
  private Rigidbody rig;
  private Item item;
  public float velMult = 1f;
  public SFX_Instance[] impact;
  public bool disallowInHands;

  private void Start()
  {
    this.rig = this.GetComponent<Rigidbody>();
    this.item = this.GetComponent<Item>();
  }

  private void Update()
  {
    if (!(bool) (Object) this.rig || this.rig.isKinematic)
      return;
    this.vel = Mathf.Lerp(this.vel, Vector3.SqrMagnitude(this.rig.linearVelocity) * this.velMult, 10f * Time.deltaTime);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!(bool) (Object) this.rig)
      return;
    if ((bool) (Object) this.item)
    {
      if (!(bool) (Object) this.item.holderCharacter)
      {
        if ((double) this.vel > 4.0)
        {
          for (int index = 0; index < this.impact.Length; ++index)
            this.impact[index].Play(this.transform.position);
        }
      }
      else if ((double) this.vel > 36.0 && !this.disallowInHands)
      {
        for (int index = 0; index < this.impact.Length; ++index)
          this.impact[index].Play(this.transform.position);
      }
    }
    if (!(bool) (Object) this.item && !(bool) (Object) collision.rigidbody && (double) this.vel > 64.0)
    {
      for (int index = 0; index < this.impact.Length; ++index)
        this.impact[index].Play(this.transform.position);
    }
    this.vel = 0.0f;
  }
}
