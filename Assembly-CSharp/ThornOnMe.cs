// Decompiled with JetBrains decompiler
// Type: ThornOnMe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ThornOnMe : MonoBehaviour, IInteractibleConstant, IInteractible
{
  [HideInInspector]
  public Character character;
  public int thornDamage;
  public bool stuckIn;
  public bool visibleLocally;
  private float popOutTime;
  private MaterialPropertyBlock mpb;
  public Renderer mainRenderer;

  private void OnEnable()
  {
    if (!((Object) this.mainRenderer == (Object) null))
      return;
    this.AddPropertyBlock();
  }

  private float GetPopOutTime(bool solo) => !solo ? 120f : 30f;

  public bool ShouldPopOut() => this.stuckIn && (double) Time.time > (double) this.popOutTime;

  public void EnableThorn()
  {
    if (!this.character.IsLocal || this.visibleLocally)
      this.gameObject.SetActive(true);
    this.stuckIn = true;
    this.popOutTime = Time.time + this.GetPopOutTime(Character.AllCharacters.Count == 1);
  }

  public void DisableThorn()
  {
    this.gameObject.SetActive(false);
    this.stuckIn = false;
  }

  public bool IsInteractible(Character interactor)
  {
    return !interactor.IsStuck() && (double) Vector3.Angle(this.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward) <= 5.0 + (Character.AllCharacters.Count == 1 || (Object) interactor != (Object) this.character ? 15.0 : 0.0);
  }

  public void Interact(Character interactor)
  {
  }

  private void AddPropertyBlock()
  {
    this.mpb = new MaterialPropertyBlock();
    this.mainRenderer = (Renderer) this.GetComponentInChildren<MeshRenderer>();
    if (!(bool) (Object) this.mainRenderer)
      this.mainRenderer = (Renderer) this.GetComponentInChildren<SkinnedMeshRenderer>();
    this.mainRenderer.GetPropertyBlock(this.mpb);
  }

  public void HoverEnter()
  {
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    this.mainRenderer.SetPropertyBlock(this.mpb);
  }

  public void HoverExit()
  {
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    this.mainRenderer.SetPropertyBlock(this.mpb);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("REMOVE");

  public string GetName() => LocalizedText.GetText("Name_Thorn");

  public bool IsConstantlyInteractable(Character interactor) => true;

  public float GetInteractTime(Character interactor)
  {
    return Character.AllCharacters.Count != 1 && !((Object) interactor != (Object) this.character) ? 3f : 1f;
  }

  public void Interact_CastFinished(Character interactor)
  {
    this.character.refs.afflictions.RemoveThorn(this);
  }

  public void CancelCast(Character interactor)
  {
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public bool holdOnFinish => false;
}
