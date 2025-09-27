// Decompiled with JetBrains decompiler
// Type: RopeSyncer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RopeSyncer : PhotonBinaryStreamSerializer<RopeSyncData>
{
  public Rope rope;
  public Optionable<float> startSyncTime = Optionable<float>.None;
  private int syncIndex;
  private bool updateVisualizerManually;

  protected override void Awake()
  {
    if ((bool) (UnityEngine.Object) this.rope)
      return;
    this.rope = this.GetComponent<Rope>();
  }

  public override RopeSyncData GetDataToWrite()
  {
    return this.rope.GetSyncData() with
    {
      updateVisualizerManually = this.updateVisualizerManually
    };
  }

  public override void OnDataReceived(RopeSyncData data)
  {
    base.OnDataReceived(data);
    this.rope.SetSyncData(data);
  }

  public override bool ShouldSendData()
  {
    List<Transform> ropeSegments = this.rope.GetRopeSegments();
    if (this.rope.isClimbable && this.startSyncTime.IsNone)
      this.startSyncTime = Optionable<float>.Some(Time.realtimeSinceStartup);
    if (ropeSegments.Count == 0)
      return false;
    Vector3 pos = ropeSegments.First<Transform>().position;
    if (PlayerHandler.GetAllPlayerCharacters().Count == 0 || (double) PlayerHandler.GetAllPlayerCharacters().Select<Character, float>((Func<Character, float>) (character => Vector3.Distance(character.Center, pos))).Min<float>((Func<float, float>) (f => f)) > 100.0)
      return false;
    if (this.startSyncTime.IsSome && (double) Time.realtimeSinceStartup - (double) this.startSyncTime.Value > 60.0)
    {
      this.updateVisualizerManually = true;
      ++this.syncIndex;
      if (this.syncIndex < 600)
        return false;
      this.syncIndex = 0;
    }
    return !this.rope.creatorLeft;
  }
}
