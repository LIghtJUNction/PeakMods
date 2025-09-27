// Decompiled with JetBrains decompiler
// Type: MenuWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
public class MenuWindow : MonoBehaviour, INavigationContainer
{
  public static List<MenuWindow> AllActiveWindows = new List<MenuWindow>();

  public virtual bool openOnStart => true;

  public virtual bool selectOnOpen => true;

  public virtual Selectable objectToSelectOnOpen => (Selectable) null;

  public virtual bool closeOnPause => false;

  public virtual bool closeOnUICancel => false;

  public virtual bool blocksPlayerInput => true;

  public virtual bool showCursorWhileOpen => true;

  public virtual bool autoHideOnClose => true;

  public bool isOpen { get; private set; }

  public bool inputActive { get; private set; }

  public bool initialized { get; private set; }

  public virtual GameObject panel => this.gameObject;

  protected virtual void Start()
  {
    if (this.isOpen)
      return;
    if (this.openOnStart)
      this.Open();
    else
      this.StartClosed();
  }

  protected virtual void Update()
  {
    if (this.isOpen)
      INavigationContainer.PushActive((INavigationContainer) this);
    this.TestCloseViaInput();
  }

  private void TestCloseViaInput()
  {
    if (!this.inputActive)
      return;
    if (this.closeOnPause && (bool) (Object) Character.localCharacter && Character.localCharacter.input.pauseWasPressed)
    {
      this.Close();
      Character.localCharacter.input.pauseWasPressed = false;
    }
    else
    {
      if (!this.closeOnUICancel || !Singleton<UIInputHandler>.Instance.cancelWasPressed)
        return;
      this.Close();
      Singleton<UIInputHandler>.Instance.cancelWasPressed = false;
    }
  }

  protected virtual void Initialize()
  {
  }

  internal virtual void Open()
  {
    Debug.Log((object) "opening window", (Object) this.gameObject);
    this.isOpen = true;
    if (!MenuWindow.AllActiveWindows.Contains(this))
      MenuWindow.AllActiveWindows.Add(this);
    this.Show();
    if (!this.initialized)
    {
      this.Initialize();
      this.initialized = true;
    }
    this.OnOpen();
    if (this.selectOnOpen)
      this.SelectStartingElement();
    this.SetInputActive(true);
  }

  protected virtual void OnOpen()
  {
  }

  private void OnDestroy()
  {
    if (!MenuWindow.AllActiveWindows.Contains(this))
      return;
    MenuWindow.AllActiveWindows.Remove(this);
  }

  public static void CloseAllWindows()
  {
    for (int index = MenuWindow.AllActiveWindows.Count - 1; index >= 0; --index)
    {
      if ((Object) MenuWindow.AllActiveWindows[index] != (Object) null)
        MenuWindow.AllActiveWindows[index].ForceClose();
    }
  }

  internal void StartClosed()
  {
    this.isOpen = false;
    this.SetInputActive(false);
    this.panel.SetActive(false);
  }

  internal void Close()
  {
    Debug.Log((object) (this.gameObject.name + " closing."));
    this.isOpen = false;
    if (MenuWindow.AllActiveWindows.Contains(this))
      MenuWindow.AllActiveWindows.Remove(this);
    this.OnClose();
    this.SetInputActive(false);
    if (!this.autoHideOnClose)
      return;
    this.Hide();
  }

  internal void ForceClose() => this.Close();

  protected virtual void OnClose()
  {
  }

  public void Show() => this.panel.SetActive(true);

  public void Hide() => this.panel.SetActive(false);

  public void SetInputActive(bool active) => this.inputActive = active;

  private void SelectStartingElement()
  {
    UIInputHandler.SetSelectedObject((Object) this.objectToSelectOnOpen == (Object) null ? (GameObject) null : this.objectToSelectOnOpen.gameObject);
  }

  public int GetContainerPriority() => 1;

  public GameObject GetDefaultSelection()
  {
    return (Object) this.objectToSelectOnOpen == (Object) null ? (GameObject) null : this.objectToSelectOnOpen.gameObject;
  }

  public bool IsValidSelection(GameObject selection) => selection.activeInHierarchy;
}
