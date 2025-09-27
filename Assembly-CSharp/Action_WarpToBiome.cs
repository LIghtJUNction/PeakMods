// Decompiled with JetBrains decompiler
// Type: Action_WarpToBiome
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Action_WarpToBiome : ItemAction
{
  public Segment segmentToWarpTo;

  public override void RunAction()
  {
    Debug.Log((object) ("WARP TO " + this.segmentToWarpTo.ToString()));
    MapHandler.JumpToSegment(this.segmentToWarpTo);
  }
}
