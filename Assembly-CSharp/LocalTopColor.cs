// Decompiled with JetBrains decompiler
// Type: LocalTopColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LocalTopColor : MonoBehaviour
{
  public MeshRenderer renderer;

  private void Start() => this.setTopVector();

  private void setTopVector()
  {
    MaterialPropertyBlock properties = new MaterialPropertyBlock();
    Vector3 vector3 = this.transform.InverseTransformDirection(Vector3.up);
    properties.SetVector("_LocalTopDirection", (Vector4) vector3);
    this.renderer.SetPropertyBlock(properties);
  }
}
