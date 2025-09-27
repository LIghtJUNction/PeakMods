// Decompiled with JetBrains decompiler
// Type: HideTheBody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class HideTheBody : MonoBehaviour
{
  public bool isDummy;
  public SkinnedMeshRenderer body;
  public Renderer headRend;
  public CustomizationRefs refs;
  public Transform face;
  public GameObject shadowCaster;
  public GameObject shadowCasterHat;
  public SkinnedMeshRenderer[] costumes;
  [FormerlySerializedAs("Sash")]
  public SkinnedMeshRenderer sash;
  private bool isShowing = true;
  private Character character;
  private int VERTEXGHOST = Shader.PropertyToID("_VertexGhost");

  private void Start()
  {
    this.character = this.GetComponentInParent<Character>();
    this.Toggle(true);
  }

  private void Update()
  {
    bool show = !this.character.IsLocal || this.character.data.fullyPassedOut || this.character.data.dead || this.isDummy;
    if (!this.character.IsLocal && (Object) this.character.data.carrier != (Object) null && this.character.data.carrier.IsLocal)
      show = false;
    if (show == this.isShowing)
      return;
    this.Toggle(show);
  }

  public void Refresh() => this.Toggle(this.isShowing);

  private void Toggle(bool show)
  {
    this.isShowing = show;
    this.shadowCaster.SetActive(!show);
    this.shadowCasterHat.SetActive(!show);
    if (show)
    {
      this.SetShowing((Renderer) this.body, 0.0f);
      this.SetShowing(this.headRend, 0.0f);
      this.SetShowing((Renderer) this.sash, 0.0f);
      for (int index = 0; index < this.costumes.Length; ++index)
        this.SetShowing((Renderer) this.costumes[index], 0.0f);
      foreach (Renderer componentsInChild in this.face.GetComponentsInChildren<Renderer>())
        this.SetShowing(componentsInChild, 0.0f);
      for (int index = 0; index < this.refs.playerHats.Length; ++index)
        this.SetShowing(this.refs.playerHats[index], 0.0f);
    }
    else
    {
      this.SetShowing((Renderer) this.body, 1f);
      this.SetShowing(this.headRend, 1f);
      this.SetShowing((Renderer) this.sash, 1f);
      for (int index = 0; index < this.costumes.Length; ++index)
        this.SetShowing((Renderer) this.costumes[index], 1f);
      foreach (Renderer componentsInChild in this.face.GetComponentsInChildren<Renderer>())
        this.SetShowing(componentsInChild, 1f);
      for (int index = 0; index < this.refs.playerHats.Length; ++index)
        this.SetShowing(this.refs.playerHats[index], 1f);
    }
  }

  public void SetShowing(Renderer r, float x)
  {
    Material[] materials = r.materials;
    foreach (Material material in materials)
      material.SetFloat(this.VERTEXGHOST, x);
    r.materials = materials;
  }
}
