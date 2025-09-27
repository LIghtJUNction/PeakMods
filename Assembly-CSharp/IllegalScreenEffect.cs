// Decompiled with JetBrains decompiler
// Type: IllegalScreenEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class IllegalScreenEffect : MonoBehaviour
{
  public string statusName = "BLIND";
  public string shaderVarName = "_Alpha";
  private float activeForSeconds;
  private Character character;
  private MeshRenderer rend;
  private Material mat;

  private void Start()
  {
    this.rend = this.GetComponent<MeshRenderer>();
    this.rend.enabled = false;
    this.mat = this.rend.material;
  }

  private void Update()
  {
    if (!(bool) (UnityEngine.Object) this.character)
    {
      if (!(bool) (UnityEngine.Object) Character.localCharacter)
        return;
      this.character = Character.localCharacter;
      this.character.illegalStatusAction += new Action<string, float>(this.AddStatus);
    }
    else
    {
      if (this.character.data.fullyPassedOut || this.character.data.dead)
        this.activeForSeconds = 0.0f;
      this.activeForSeconds -= Time.deltaTime;
      if ((double) this.activeForSeconds > 0.0)
      {
        this.rend.enabled = true;
        float b = Mathf.Clamp01(this.activeForSeconds / 3f);
        float a = this.mat.GetFloat(this.shaderVarName);
        if ((double) b > (double) a)
          this.mat.SetFloat(this.shaderVarName, Mathf.Lerp(a, b, Time.deltaTime));
        else
          this.mat.SetFloat(this.shaderVarName, b);
        this.character.data.isBlind = true;
      }
      else
      {
        this.rend.enabled = false;
        this.character.data.isBlind = false;
      }
    }
  }

  private void AddStatus(string status, float duration)
  {
    if (status.ToUpper() != this.statusName.ToUpper())
      return;
    this.activeForSeconds = duration;
  }
}
