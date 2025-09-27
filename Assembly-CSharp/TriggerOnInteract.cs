// Decompiled with JetBrains decompiler
// Type: TriggerOnInteract
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TriggerOnInteract : MonoBehaviour, IInteractible
{
  private MaterialPropertyBlock mpb;
  public string interactText;
  public TriggerEvent triggerEvent;
  public string interactableName;

  private void Awake() => this.mpb = new MaterialPropertyBlock();

  public bool IsInteractible(Character interactor) => true;

  public void Interact(Character interactor) => this.triggerEvent.TriggerEntered();

  public void HoverEnter()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    this.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
  }

  public void HoverExit()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    this.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("PICKUP");

  public string GetName() => this.interactableName;
}
