// Decompiled with JetBrains decompiler
// Type: BackPackAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BackPackAudio : MonoBehaviour
{
  private Backpack item;
  public SFX_Instance[] holdSFX;
  private bool hT;
  public SFX_Instance[] dropSFX;
  private bool dT;

  private void Start() => this.item = this.GetComponent<Backpack>();

  private void Update()
  {
    if (!(bool) (Object) this.item)
      return;
    if ((bool) (Object) this.item.holderCharacter)
    {
      if (!this.hT)
      {
        for (int index = 0; index < this.holdSFX.Length; ++index)
          this.holdSFX[index].Play(this.transform.position);
        this.hT = true;
      }
    }
    else
      this.hT = false;
    if (this.item.rig.useGravity)
    {
      if (!this.dT)
      {
        for (int index = 0; index < this.dropSFX.Length; ++index)
          this.dropSFX[index].Play(this.transform.position);
      }
      this.dT = true;
    }
    else
      this.dT = false;
  }
}
