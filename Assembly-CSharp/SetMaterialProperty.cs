// Decompiled with JetBrains decompiler
// Type: SetMaterialProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SetMaterialProperty : MonoBehaviour
{
  public string propertyName;
  public float propertyValue;

  public void Go() => this.SetVal(this.propertyValue);

  public void SetVal(float val)
  {
    Renderer component = this.GetComponent<Renderer>();
    MaterialPropertyBlock properties = new MaterialPropertyBlock();
    component.GetPropertyBlock(properties);
    properties.SetFloat(this.propertyName, val);
    component.SetPropertyBlock(properties);
  }
}
