// Decompiled with JetBrains decompiler
// Type: MaterialLayerSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MaterialLayerSwapper : MonoBehaviour
{
  public int targetMaterial;
  public Vector2Int layer;
  [ColorUsage(true, true)]
  public Color color;
  public float smooth;
  public float height;
  public Texture texture;
  public float triplanar;
  public float uv;
  public float flip;
  public Vector2 remap;
  [ColorUsage(true, true)]
  public Color color2;
  public float smooth2;
  public float height2;
  public Texture texture2;
  public float triplanar2;
  public float uv2;
  public float flip2;
  public Vector2 remap2;

  private void Swap()
  {
    string name1 = "_Color" + this.layer.x.ToString("F0");
    string name2 = "_Smooth" + this.layer.x.ToString("F0");
    string name3 = "_Height" + this.layer.x.ToString("F0");
    string name4 = "_Texture" + this.layer.x.ToString("F0");
    string name5 = "_Triplanar" + this.layer.x.ToString("F0");
    string name6 = "_UV" + this.layer.x.ToString("F0");
    string name7 = "_Flip" + this.layer.x.ToString("F0");
    string name8 = "_Remap" + this.layer.x.ToString("F0");
    Material sharedMaterial = this.GetComponentInChildren<Renderer>().sharedMaterials[this.targetMaterial];
    this.color = sharedMaterial.GetColor(name1);
    this.smooth = sharedMaterial.GetFloat(name2);
    this.height = sharedMaterial.GetFloat(name3);
    this.texture = sharedMaterial.GetTexture(name4);
    this.triplanar = sharedMaterial.GetFloat(name5);
    this.uv = sharedMaterial.GetFloat(name6);
    this.flip = sharedMaterial.GetFloat(name7);
    this.remap = (Vector2) sharedMaterial.GetVector(name8);
    string name9 = "_Color" + this.layer.y.ToString("F0");
    string name10 = "_Smooth" + this.layer.y.ToString("F0");
    string name11 = "_Height" + this.layer.y.ToString("F0");
    string name12 = "_Texture" + this.layer.y.ToString("F0");
    string name13 = "_Triplanar" + this.layer.y.ToString("F0");
    string name14 = "_UV" + this.layer.y.ToString("F0");
    string name15 = "_Flip" + this.layer.y.ToString("F0");
    string name16 = "_Remap" + this.layer.y.ToString("F0");
    this.color2 = sharedMaterial.GetColor(name9);
    this.smooth2 = sharedMaterial.GetFloat(name10);
    this.height2 = sharedMaterial.GetFloat(name11);
    this.texture2 = sharedMaterial.GetTexture(name12);
    this.triplanar2 = sharedMaterial.GetFloat(name13);
    this.uv2 = sharedMaterial.GetFloat(name14);
    this.flip2 = sharedMaterial.GetFloat(name15);
    this.remap2 = (Vector2) sharedMaterial.GetVector(name16);
    sharedMaterial.SetColor(name9, this.color);
    sharedMaterial.SetFloat(name10, this.smooth);
    sharedMaterial.SetFloat(name11, this.height);
    sharedMaterial.SetTexture(name12, this.texture);
    sharedMaterial.SetFloat(name13, this.triplanar);
    sharedMaterial.SetFloat(name14, this.uv);
    sharedMaterial.SetFloat(name15, this.flip);
    sharedMaterial.SetVector(name16, (Vector4) this.remap);
    sharedMaterial.SetColor(name1, this.color2);
    sharedMaterial.SetFloat(name2, this.smooth2);
    sharedMaterial.SetFloat(name3, this.height2);
    sharedMaterial.SetTexture(name4, this.texture2);
    sharedMaterial.SetFloat(name5, this.triplanar2);
    sharedMaterial.SetFloat(name6, this.uv2);
    sharedMaterial.SetFloat(name7, this.flip2);
    sharedMaterial.SetVector(name8, (Vector4) this.remap2);
  }
}
