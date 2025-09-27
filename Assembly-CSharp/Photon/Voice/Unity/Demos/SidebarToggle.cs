// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.SidebarToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos;

public class SidebarToggle : MonoBehaviour
{
  [SerializeField]
  private Button sidebarButton;
  [SerializeField]
  private RectTransform panelsHolder;
  private float sidebarWidth = 300f;
  private bool sidebarOpen = true;

  private void Awake()
  {
    this.sidebarButton.onClick.RemoveAllListeners();
    this.sidebarButton.onClick.AddListener(new UnityAction(this.ToggleSidebar));
    this.ToggleSidebar(this.sidebarOpen);
  }

  [ContextMenu("ToggleSidebar")]
  private void ToggleSidebar()
  {
    this.sidebarOpen = !this.sidebarOpen;
    this.ToggleSidebar(this.sidebarOpen);
  }

  private void ToggleSidebar(bool open)
  {
    if (!open)
      this.panelsHolder.SetPosX(0.0f);
    else
      this.panelsHolder.SetPosX(this.sidebarWidth);
  }
}
