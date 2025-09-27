// Decompiled with JetBrains decompiler
// Type: Action_Guidebook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core;

#nullable disable
public class Action_Guidebook : ItemAction
{
  private Guidebook guidebook;
  public bool isSinglePage;
  public int singlePageIndex;

  private void Awake() => this.guidebook = this.GetComponent<Guidebook>();

  public override void RunAction()
  {
    this.guidebook.ToggleGuidebook();
    if (!this.isSinglePage)
      return;
    Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(this.singlePageIndex);
  }
}
