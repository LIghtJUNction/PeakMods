// Decompiled with JetBrains decompiler
// Type: PauseMainMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
public class PauseMainMenu : MenuWindow
{
  public MenuWindow mainMenu;
  public Button backButton;

  public override bool openOnStart => false;

  public override bool selectOnOpen => true;

  public override bool closeOnPause => true;

  public override bool closeOnUICancel => true;

  protected override void Initialize()
  {
    this.backButton.onClick.AddListener(new UnityAction(((MenuWindow) this).Close));
  }

  protected override void OnClose() => this.mainMenu.Open();
}
