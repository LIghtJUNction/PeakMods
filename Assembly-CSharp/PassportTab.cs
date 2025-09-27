// Decompiled with JetBrains decompiler
// Type: PassportTab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PassportTab : MonoBehaviour
{
  public PassportManager manager;
  public Customization.Type type;
  public Animator anim;
  private bool opened;

  public void SetTab()
  {
    if (this.opened)
      return;
    this.manager.OpenTab(this.type);
  }

  public void Open()
  {
    this.anim.SetBool(nameof (Open), true);
    this.opened = true;
  }

  public void Close()
  {
    this.anim.SetBool("Open", false);
    this.opened = false;
  }
}
