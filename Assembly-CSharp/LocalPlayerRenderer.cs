// Decompiled with JetBrains decompiler
// Type: LocalPlayerRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public class LocalPlayerRenderer : MonoBehaviour
{
  public ShadowCastingMode renderMode;

  private void Start()
  {
    Character componentInParent = this.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || !componentInParent.IsLocal)
      return;
    this.GetComponent<MeshRenderer>().shadowCastingMode = this.renderMode;
  }
}
