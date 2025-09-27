// Decompiled with JetBrains decompiler
// Type: Guidebook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using Photon.Pun;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
public class Guidebook : Item
{
  public static int BASETEX = Shader.PropertyToID("_BaseTexture");
  public bool isSinglePage;
  public Animator anim;
  public int currentPageSet;
  [FormerlySerializedAs("pages")]
  [PreviouslySerializedAs("pages")]
  public List<GuidebookSpread> pageSpreads;
  public List<RectTransform> pagePrefabs;
  public Camera renderCamera;
  public CanvasScaler canvasScaler;
  public Texture currentRenderTexture;
  public Texture lastRenderTexture;
  public Renderer[] pageRenderers;
  public Transform bookTransform;
  public float readingDistance = 0.4f;
  public Collider coll;
  [HideInInspector]
  public bool isOpen;
  public RenderTexture guidebookRenderTexture;
  private int currentlyVisibleLeftPageIndex;
  private int currentlyVisibleRightPageIndex;
  private int nextVisibleLeftPageIndex;
  private int nextVisibleRightPageIndex;

  public override void OnEnable()
  {
    base.OnEnable();
    if (SettingsHandler.Instance.GetSetting<RenderScaleSetting>().Value == RenderScaleSetting.RenderScaleQuality.Low)
    {
      this.canvasScaler.scaleFactor = 2f;
      this.currentRenderTexture.width = 3600;
      this.currentRenderTexture.height = 1800;
      this.lastRenderTexture.width = 3600;
      this.lastRenderTexture.height = 1800;
    }
    else
    {
      this.canvasScaler.scaleFactor = 1f;
      this.currentRenderTexture.width = 1800;
      this.currentRenderTexture.height = 900;
      this.lastRenderTexture.width = 1800;
      this.lastRenderTexture.height = 900;
    }
    if (!this.isSinglePage)
      return;
    this.Invoke("OpenSinglePage", 0.01f);
  }

  private void OpenSinglePage()
  {
    RenderTexture renderTexture = new RenderTexture(this.guidebookRenderTexture);
    renderTexture.Create();
    this.guidebookRenderTexture = renderTexture;
    this.currentRenderTexture = (Texture) renderTexture;
    this.renderCamera.targetTexture = this.guidebookRenderTexture;
    this.currentlyVisibleLeftPageIndex = 2;
    this.currentlyVisibleRightPageIndex = 3;
    this.nextVisibleLeftPageIndex = 0;
    this.nextVisibleRightPageIndex = 1;
    this.UpdatePageDisplay();
    for (int index = 0; index < this.pageRenderers.Length; ++index)
      this.pageRenderers[index].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
  }

  public override void OnDisable()
  {
    base.OnDisable();
    if (!this.isSinglePage)
      return;
    Object.Destroy((Object) this.renderCamera.targetTexture);
  }

  internal void ToggleGuidebook()
  {
    if (!this.photonView.IsMine)
      return;
    this.photonView.RPC("ToggleGuidebook_RPC", RpcTarget.All, (object) !this.isOpen);
  }

  [PunRPC]
  public void ToggleGuidebook_RPC(bool open)
  {
    this.isOpen = open;
    if (this.isOpen)
    {
      if (!this.isSinglePage)
        this.anim.Play("Open", 0, 0.0f);
      this.coll.enabled = false;
      this.renderCamera.targetTexture = this.guidebookRenderTexture;
      this.currentlyVisibleLeftPageIndex = 2;
      this.currentlyVisibleRightPageIndex = 3;
      this.nextVisibleLeftPageIndex = 0;
      this.nextVisibleRightPageIndex = 1;
      this.UpdatePageDisplay();
      for (int index = 0; index < this.pageRenderers.Length; ++index)
        this.pageRenderers[index].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
    }
    else
    {
      if (!this.isSinglePage)
        this.anim.Play("Close", 0, 0.0f);
      this.coll.enabled = true;
      this.bookTransform.DOLocalMove(Vector3.zero, 0.25f);
      this.bookTransform.DOLocalRotate(Vector3.zero, 0.25f);
      for (int index = 0; index < this.pageRenderers.Length; ++index)
        this.pageRenderers[index].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
    }
  }

  private void LateUpdate()
  {
    if (!this.isOpen || !this.holderCharacter.IsLocal)
      return;
    this.bookTransform.position = Vector3.Lerp(this.bookTransform.position, MainCamera.instance.cam.transform.position + MainCamera.instance.cam.transform.forward * this.readingDistance, Time.deltaTime * 10f);
    this.bookTransform.forward = MainCamera.instance.cam.transform.forward;
  }

  private void PopulatePages()
  {
    this.pageSpreads = ((IEnumerable<GuidebookSpread>) this.GetComponentsInChildren<GuidebookSpread>(true)).ToList<GuidebookSpread>();
  }

  private void PopulatePageNumbers()
  {
    int num = 0;
    while (num < this.pageSpreads.Count)
      ++num;
  }

  internal void FlipPageRight()
  {
    if (!this.photonView.IsMine || this.currentPageSet >= this.pageSpreads.Count - 1)
      return;
    ++this.currentPageSet;
    this.photonView.RPC("FlipPageRight_RPC", RpcTarget.All, (object) this.currentPageSet);
  }

  internal void FlipPageLeft()
  {
    if (!this.photonView.IsMine || this.currentPageSet < 1)
      return;
    --this.currentPageSet;
    this.photonView.RPC("FlipPageLeft_RPC", RpcTarget.All, (object) this.currentPageSet);
  }

  [PunRPC]
  public void FlipPageRight_RPC(int currentPage)
  {
    this.currentlyVisibleLeftPageIndex = 2;
    this.currentlyVisibleRightPageIndex = 3;
    this.nextVisibleLeftPageIndex = 4;
    this.nextVisibleRightPageIndex = 5;
    this.anim.Play("Guidebook_FlipRight", 0, 0.0f);
    this.currentPageSet = currentPage;
    this.UpdatePageDisplay();
  }

  [PunRPC]
  public void FlipPageLeft_RPC(int currentPage)
  {
    this.currentlyVisibleLeftPageIndex = 2;
    this.currentlyVisibleRightPageIndex = 3;
    this.nextVisibleLeftPageIndex = 0;
    this.nextVisibleRightPageIndex = 1;
    this.anim.Play("Guidebook_FlipLeft", 0, 0.0f);
    this.currentPageSet = currentPage;
    this.UpdatePageDisplay();
  }

  private void UpdatePageDisplay()
  {
    Graphics.CopyTexture(this.currentRenderTexture, this.lastRenderTexture);
    for (int index = 0; index < this.pageSpreads.Count; ++index)
      this.pageSpreads[index].gameObject.SetActive(index == this.currentPageSet);
    this.renderCamera.Render();
    this.pageRenderers[this.currentlyVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
    this.pageRenderers[this.currentlyVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
    this.pageRenderers[this.nextVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
    this.pageRenderers[this.nextVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
  }
}
