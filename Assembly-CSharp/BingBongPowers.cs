// Decompiled with JetBrains decompiler
// Type: BingBongPowers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using TMPro;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(1000000)]
public class BingBongPowers : MonoBehaviour
{
  public TextMeshProUGUI titleText;
  public TextMeshProUGUI descriptionText;
  public Transform tooltipBar;

  private void Start()
  {
    this.SetGodCamStyle();
    this.GetComponentInChildren<Canvas>().enabled = this.GetComponent<PhotonView>().IsMine;
  }

  private void SetGodCamStyle()
  {
    MainCameraMovement component = MainCamera.instance.GetComponent<MainCameraMovement>();
    component.godcam.lookSens = 20f;
    component.godcam.lookDrag = 5f;
    component.godcam.force = 15f;
    component.godcam.drag = 3f;
    component.godcam.canOrbit = false;
  }

  private void LateUpdate()
  {
    this.TogglePowers();
    this.transform.position = MainCamera.instance.transform.position;
    this.transform.rotation = MainCamera.instance.transform.rotation;
  }

  private void TogglePowers()
  {
    if (Input.GetKeyDown(KeyCode.Alpha1))
      this.ToggleID(0);
    if (Input.GetKeyDown(KeyCode.Alpha2))
      this.ToggleID(1);
    if (!Input.GetKeyDown(KeyCode.Alpha3))
      return;
    this.ToggleID(2);
  }

  private void ToggleID(int id)
  {
    this.GetComponent<BingBongPhysics>().enabled = false;
    this.GetComponent<BingBongTimeControl>().enabled = false;
    this.GetComponent<BingBongStatus>().enabled = false;
    if (id == 0)
      this.GetComponent<BingBongPhysics>().enabled = true;
    if (id == 1)
      this.GetComponent<BingBongTimeControl>().enabled = true;
    if (id == 2)
      this.GetComponent<BingBongStatus>().enabled = true;
    for (int index = 0; index < this.tooltipBar.childCount; ++index)
    {
      if (index == id)
        this.tooltipBar.GetChild(index).GetComponent<CanvasGroup>().alpha = 1f;
      else
        this.tooltipBar.GetChild(index).GetComponent<CanvasGroup>().alpha = 0.5f;
    }
  }

  public void SetTexts(string titleDescr, string description)
  {
    this.titleText.text = titleDescr;
    this.descriptionText.text = description;
  }

  public void SetTip(string tip, int toolID)
  {
    this.tooltipBar.GetChild(toolID).Find("Tip").GetComponent<TextMeshProUGUI>().text = tip;
  }
}
