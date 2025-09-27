// Decompiled with JetBrains decompiler
// Type: SlideTrail
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SlideTrail : MonoBehaviour
{
  private ParticleSystem l;
  private ParticleSystem r;
  private Character character;

  private void Start()
  {
    ParticleSystem[] componentsInChildren = this.GetComponentsInChildren<ParticleSystem>();
    this.l = componentsInChildren[0];
    this.r = componentsInChildren[1];
    this.character = this.GetComponentInParent<Character>();
  }

  private void Update()
  {
    this.l.transform.position = this.character.GetBodypartRig(BodypartType.Hand_L).position;
    this.r.transform.position = this.character.GetBodypartRig(BodypartType.Hand_R).position;
    if (this.character.IsSliding() && (double) this.character.data.outOfStaminaFor > 2.0)
    {
      this.HandlePart(this.l, this.l.transform.position);
      this.HandlePart(this.r, this.r.transform.position);
    }
    else
    {
      this.SetPartOn(this.l, false);
      this.SetPartOn(this.r, false);
    }
  }

  private void HandlePart(ParticleSystem part, Vector3 position)
  {
    if ((bool) (Object) HelperFunctions.LineCheck(position, position - this.character.data.groundNormal * 0.3f, HelperFunctions.LayerType.Terrain).transform)
      this.SetPartOn(part, true);
    else
      this.SetPartOn(part, false);
  }

  private void SetPartOn(ParticleSystem part, bool on)
  {
    if (on && !part.isPlaying)
    {
      part.Play(true);
    }
    else
    {
      if (on || !part.isPlaying)
        return;
      part.Stop(true);
    }
  }
}
