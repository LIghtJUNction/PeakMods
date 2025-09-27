// Decompiled with JetBrains decompiler
// Type: ScoutCannonFuse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ScoutCannonFuse : MonoBehaviour, IInteractibleConstant, IInteractible
{
  public ScoutCannon scoutCannon;
  private MaterialPropertyBlock mpb;
  private MeshRenderer[] _mr;

  public bool holdOnFinish => false;

  private MeshRenderer[] meshRenderers
  {
    get
    {
      if (this._mr == null)
        this._mr = this.GetComponentsInChildren<MeshRenderer>();
      return this._mr;
    }
    set => this._mr = value;
  }

  public void CancelCast(Character interactor)
  {
  }

  public Vector3 Center() => this.transform.position;

  public string GetInteractionText() => LocalizedText.GetText("LIGHT");

  public float GetInteractTime(Character interactor) => 1f;

  public string GetName() => LocalizedText.GetText("SCOUTCANNONFUSE");

  public Transform GetTransform() => this.transform;

  public void HoverEnter()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((Object) this.meshRenderers[index] != (Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void HoverExit()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((Object) this.meshRenderers[index] != (Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void Interact(Character interactor)
  {
  }

  public void Interact_CastFinished(Character interactor) => this.scoutCannon.Light();

  public bool IsConstantlyInteractable(Character interactor) => !this.scoutCannon.lit;

  public bool IsInteractible(Character interactor) => true;

  public void ReleaseInteract(Character interactor)
  {
  }
}
