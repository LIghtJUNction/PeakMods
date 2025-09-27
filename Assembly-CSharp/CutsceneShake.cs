// Decompiled with JetBrains decompiler
// Type: CutsceneShake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CutsceneShake : MonoBehaviour
{
  public Transform follow;
  public float smooth = 0.5f;
  public float shake;

  private void Update()
  {
    this.transform.position = Vector3.Lerp(this.transform.position, this.follow.position, this.smooth * Time.deltaTime);
    this.transform.Translate(Vector3.right * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
    this.transform.Translate(Vector3.up * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
    this.transform.Translate(Vector3.forward * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
  }
}
