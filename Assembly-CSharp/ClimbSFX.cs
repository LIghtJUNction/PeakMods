// Decompiled with JetBrains decompiler
// Type: ClimbSFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ClimbSFX : MonoBehaviour
{
  private Character character;
  public GameObject ropeOn;
  public GameObject ropeOff;
  private bool rToggle;
  public GameObject surfaceOn;
  public GameObject surfaceOff;
  public GameObject surfaceOnHeavy;
  private bool sToggle;

  private void Start() => this.character = this.transform.root.GetComponent<Character>();

  private void Update()
  {
    if (!(bool) (Object) this.character)
      return;
    if (!this.character.data.isClimbing && this.sToggle)
    {
      this.sToggle = false;
      this.surfaceOff.SetActive(true);
      this.surfaceOnHeavy.SetActive(false);
      this.surfaceOn.SetActive(false);
    }
    if (this.character.data.isClimbing && !this.sToggle)
    {
      this.sToggle = true;
      this.surfaceOn.SetActive(true);
      if ((double) this.character.data.avarageVelocity.y <= -6.0)
        this.surfaceOnHeavy.SetActive(true);
      this.surfaceOff.SetActive(false);
    }
    if (!this.character.data.isRopeClimbing && this.rToggle)
    {
      this.rToggle = false;
      this.ropeOff.SetActive(true);
      this.ropeOn.SetActive(false);
    }
    if (!this.character.data.isRopeClimbing || this.rToggle)
      return;
    this.rToggle = true;
    this.ropeOn.SetActive(true);
    this.ropeOff.SetActive(false);
  }
}
