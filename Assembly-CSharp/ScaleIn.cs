// Decompiled with JetBrains decompiler
// Type: ScaleIn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ScaleIn : MonoBehaviour
{
  private float targetScale = 1f;

  private void Start()
  {
    this.targetScale = this.transform.localScale.x;
    this.transform.localScale = Vector3.zero;
  }

  private void Update()
  {
    this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(this.targetScale, this.targetScale, this.targetScale), Time.deltaTime * 5f);
    if ((double) Mathf.Abs(this.targetScale - this.transform.localScale.x) >= 0.05000000074505806)
      return;
    Object.Destroy((Object) this);
  }
}
