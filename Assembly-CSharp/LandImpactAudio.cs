// Decompiled with JetBrains decompiler
// Type: LandImpactAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LandImpactAudio : MonoBehaviour
{
  private Character character;
  public float impactVelocity;
  private float yVel;
  private float storeYVel;
  private float prevY;
  private bool t;
  public GameObject impactSmall;
  public GameObject impactMedium;
  public GameObject impactHeavy;
  public GameObject impactGiant;

  private void Start() => this.character = this.transform.root.GetComponent<Character>();

  private void Update()
  {
    this.yVel = this.transform.position.y - this.prevY;
    this.prevY = this.transform.position.y;
    if ((double) this.yVel < -0.02500000037252903)
      this.storeYVel = this.yVel;
    if ((double) this.yVel > 0.02500000037252903)
      this.storeYVel = 0.0f;
    this.impactVelocity = this.storeYVel;
    if (!this.t && this.character.data.isGrounded)
    {
      if ((double) this.impactVelocity < -0.30000001192092896 && !this.t)
      {
        if ((bool) (Object) this.impactGiant)
          Object.Instantiate<GameObject>(this.impactGiant, this.transform.position, this.transform.rotation);
        this.t = true;
      }
      if ((double) this.impactVelocity < -0.20000000298023224 && !this.t)
      {
        this.impactHeavy.SetActive(true);
        this.t = true;
      }
      if ((double) this.impactVelocity < -0.10000000149011612 && !this.t)
      {
        this.impactMedium.SetActive(true);
        this.t = true;
      }
      if ((double) this.impactVelocity < -0.05000000074505806 && !this.t)
      {
        this.impactSmall.SetActive(true);
        this.t = true;
      }
      this.storeYVel = 0.0f;
    }
    if (this.character.data.isGrounded)
      this.storeYVel = 0.0f;
    if (this.character.data.isGrounded)
      return;
    this.t = false;
    this.impactHeavy.SetActive(false);
    this.impactMedium.SetActive(false);
    this.impactSmall.SetActive(false);
  }
}
