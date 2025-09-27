// Decompiled with JetBrains decompiler
// Type: BackpackOnBackVisuals
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BackpackOnBackVisuals : BackpackVisuals, IInteractibleConstant, IInteractible
{
  private static readonly int Interactable = Shader.PropertyToID("_Interactable");
  public Character character;
  public float openRadialMenuTime = 0.25f;
  private MeshRenderer[] renderers;
  private Color[] defaultTints;

  private void Awake()
  {
    this.character = this.GetComponentInParent<Character>();
    this.InitRenderers();
  }

  private void OnEnable() => this.RefreshCooking();

  private void InitRenderers()
  {
    this.renderers = this.GetComponentsInChildren<MeshRenderer>();
    this.defaultTints = new Color[this.renderers.Length];
    for (int index = 0; index < this.renderers.Length; ++index)
      this.defaultTints[index] = this.renderers[index].material.GetColor("_Tint");
  }

  private void RefreshCooking()
  {
    IntItemData intItemData;
    if (!this.character.player.backpackSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
      return;
    this.CookVisually(intItemData.Value);
  }

  protected virtual void CookVisually(int cookedAmount)
  {
    Debug.Log((object) "Cooking backpack visually");
    if (this.renderers == null)
      this.InitRenderers();
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      if (cookedAmount > 0)
      {
        Debug.Log((object) $"Cooked amount is {cookedAmount}");
        this.renderers[index].material.SetColor("_Tint", this.defaultTints[index] * ItemCooking.GetCookColor(cookedAmount));
      }
    }
  }

  public override BackpackData GetBackpackData()
  {
    BackpackData backpackData;
    if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
      this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
    return backpackData;
  }

  protected override void PutItemInBackpack(GameObject visual, byte slotID)
  {
    visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, (object) slotID, (object) BackpackReference.GetFromEquippedBackpack(this.character));
  }

  public bool IsInteractible(Character interactor)
  {
    return (double) Vector3.Angle(HelperFunctions.ZeroY(interactor.data.lookDirection), HelperFunctions.ZeroY(this.transform.forward)) < 110.0;
  }

  public void Interact(Character interactor)
  {
  }

  public void HoverEnter()
  {
    MeshRenderer componentInChildren = this.GetComponentInChildren<MeshRenderer>();
    if (!(bool) (Object) componentInChildren)
      return;
    componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 1f);
  }

  public void HoverExit()
  {
    MeshRenderer componentInChildren = this.GetComponentInChildren<MeshRenderer>();
    if (!(bool) (Object) componentInChildren)
      return;
    componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 0.0f);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("open");

  public string GetName()
  {
    return LocalizedText.GetText("SOMEONESBACKPACK").Replace("#", this.character.characterName);
  }

  public bool IsConstantlyInteractable(Character interactor) => this.IsInteractible(interactor);

  public float GetInteractTime(Character interactor) => this.openRadialMenuTime;

  public void Interact_CastFinished(Character interactor)
  {
    GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromEquippedBackpack(this.character));
  }

  public void CancelCast(Character interactor)
  {
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public bool holdOnFinish => false;
}
