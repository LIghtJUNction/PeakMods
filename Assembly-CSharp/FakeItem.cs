// Decompiled with JetBrains decompiler
// Type: FakeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI.Extensions;

#nullable disable
[ExecuteAlways]
public class FakeItem : MonoBehaviour, IInteractible
{
  public string itemName;
  public Item realItemPrefab;
  [ReadOnly]
  public bool pickedUp;
  [ReadOnly]
  public int index;
  private MaterialPropertyBlock mpb;
  public Renderer mainRenderer;
  private double lastCulledItems;

  private void Awake()
  {
    if (!Application.isPlaying)
      return;
    this.AddPropertyBlock();
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
    if (this.mpb != null && (Object) this.mainRenderer != (Object) null)
    {
      this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
      this.mainRenderer.SetPropertyBlock(this.mpb);
    }
    else if (this.mpb == null)
      Debug.LogError((object) "Fake item is missing it's material property block");
    else
      Debug.LogError((object) "Fake item is missing it's renderer");
  }

  public void HoverExit()
  {
    if (this.mpb != null && (Object) this.mainRenderer != (Object) null)
    {
      this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
      this.mainRenderer.SetPropertyBlock(this.mpb);
    }
    else if (this.mpb == null)
      Debug.LogError((object) "Fake item is missing it's material property block");
    else
      Debug.LogError((object) "Fake item is missing it's renderer");
  }

  public bool IsInteractible(Character interactor) => true;

  public void Interact(Character interactor)
  {
    if (!interactor.player.HasEmptySlot(this.realItemPrefab.itemID))
      return;
    this.gameObject.SetActive(false);
    FakeItemManager.Instance.photonView.RPC("RPC_RequestFakeItemPickup", RpcTarget.MasterClient, (object) interactor.GetComponent<PhotonView>(), (object) this.index);
    Debug.Log((object) ("Picking up " + this.gameObject.name));
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("PICKUP");

  public string GetName() => LocalizedText.GetText(LocalizedText.GetNameIndex(this.itemName));

  public virtual void PickUpVisibly()
  {
    this.gameObject.SetActive(false);
    FakeItemManager.Instance.fakeItemData.hiddenItems.Add(this.index);
    this.pickedUp = true;
  }

  public virtual void UnPickUpVisibly()
  {
    this.gameObject.SetActive(true);
    FakeItemManager.Instance.fakeItemData.hiddenItems.Remove(this.index);
    this.pickedUp = false;
  }
}
