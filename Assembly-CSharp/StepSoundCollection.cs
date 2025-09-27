// Decompiled with JetBrains decompiler
// Type: StepSoundCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StepSoundCollection : MonoBehaviour
{
  public SFX_Instance[] stepDefault;
  public SFX_Instance[] beachSand;
  public SFX_Instance[] beachRock;
  public SFX_Instance[] jungleGrass;
  public SFX_Instance[] jungleRock;
  public SFX_Instance[] iceSnow;
  public SFX_Instance[] iceRock;
  public SFX_Instance[] metal;
  public SFX_Instance[] wood;
  public SFX_Instance[] volcanoRock;

  public void PlayStep(Vector3 pos, int index)
  {
    if (index == 0)
    {
      for (int index1 = 0; index1 < this.stepDefault.Length; ++index1)
        this.stepDefault[index1].Play(pos);
    }
    if (index == 1)
    {
      for (int index2 = 0; index2 < this.beachSand.Length; ++index2)
        this.beachSand[index2].Play(pos);
    }
    if (index == 2)
    {
      for (int index3 = 0; index3 < this.beachRock.Length; ++index3)
        this.beachRock[index3].Play(pos);
    }
    if (index == 3)
    {
      for (int index4 = 0; index4 < this.jungleGrass.Length; ++index4)
        this.jungleGrass[index4].Play(pos);
    }
    if (index == 4)
    {
      for (int index5 = 0; index5 < this.jungleRock.Length; ++index5)
        this.jungleRock[index5].Play(pos);
    }
    if (index == 5)
    {
      for (int index6 = 0; index6 < this.iceSnow.Length; ++index6)
        this.iceSnow[index6].Play(pos);
    }
    if (index == 6)
    {
      for (int index7 = 0; index7 < this.iceSnow.Length; ++index7)
        this.iceSnow[index7].Play(pos);
    }
    if (index == 7)
    {
      for (int index8 = 0; index8 < this.metal.Length; ++index8)
        this.metal[index8].Play(pos);
    }
    if (index == 8)
    {
      for (int index9 = 0; index9 < this.wood.Length; ++index9)
        this.wood[index9].Play(pos);
    }
    if (index != 9)
      return;
    for (int index10 = 0; index10 < this.volcanoRock.Length; ++index10)
      this.volcanoRock[index10].Play(pos);
  }
}
