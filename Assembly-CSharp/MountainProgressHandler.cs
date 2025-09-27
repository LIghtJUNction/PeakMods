// Decompiled with JetBrains decompiler
// Type: MountainProgressHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class MountainProgressHandler : Singleton<MountainProgressHandler>
{
  public MountainProgressHandler.ProgressPoint[] progressPoints;
  public MountainProgressHandler.ProgressPoint tombProgressPoint;
  public int debugProgress;

  public int maxProgressPointReached { get; private set; }

  private void Start()
  {
    this.InitProgressPoints();
    GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Shore);
  }

  private void InitProgressPoints()
  {
    if (!(bool) (UnityEngine.Object) Singleton<MapHandler>.Instance)
      return;
    List<MountainProgressHandler.ProgressPoint> progressPointList = new List<MountainProgressHandler.ProgressPoint>();
    foreach (Biome.BiomeType biome in Singleton<MapHandler>.Instance.biomes)
    {
      List<MountainProgressHandler.ProgressPoint> progressPoint1;
      if (this.TryGetProgressPoints(biome, out progressPoint1))
      {
        foreach (MountainProgressHandler.ProgressPoint progressPoint2 in progressPoint1)
          progressPointList.Add(progressPoint2);
      }
    }
    progressPointList.Add(((IEnumerable<MountainProgressHandler.ProgressPoint>) this.progressPoints).Last<MountainProgressHandler.ProgressPoint>());
    this.progressPoints = progressPointList.ToArray();
  }

  public bool TryGetProgressPoints(
    Biome.BiomeType biomeType,
    out List<MountainProgressHandler.ProgressPoint> progressPoint)
  {
    progressPoint = new List<MountainProgressHandler.ProgressPoint>();
    foreach (MountainProgressHandler.ProgressPoint progressPoint1 in this.progressPoints)
    {
      if (progressPoint1.biome == biomeType)
        progressPoint.Add(progressPoint1);
    }
    return progressPoint.Count > 0;
  }

  public void SetSegmentComplete(int segment)
  {
    Debug.Log((object) ("Segment complete: " + segment.ToString()));
    MountainProgressHandler.ProgressPoint progressPoint = this.progressPoints[segment];
    progressPoint.Reached = true;
    this.TriggerReached(progressPoint);
    if (segment <= this.maxProgressPointReached)
      return;
    this.maxProgressPointReached = segment;
  }

  private void Update() => this.CheckProgress();

  public void CheckProgress(bool playAnimation = true)
  {
    if (!(bool) (UnityEngine.Object) Singleton<MapHandler>.Instance)
    {
      this.enabled = false;
    }
    else
    {
      for (int index = 0; index < this.progressPoints.Length; ++index)
      {
        if (!this.progressPoints[index].Reached)
        {
          if ((UnityEngine.Object) this.progressPoints[index].transform != (UnityEngine.Object) null)
            this.progressPoints[index].Reached = this.CheckReached(this.progressPoints[index]);
          if (playAnimation && this.progressPoints[index].Reached)
            this.TriggerReached(this.progressPoints[index]);
        }
      }
    }
  }

  public void DebugTriggerReached() => this.TriggerReached(this.progressPoints[this.debugProgress]);

  public void TriggerReached(
    MountainProgressHandler.ProgressPoint progressPoint)
  {
    if ((double) Time.time <= 2.0)
      return;
    this.CheckAreaAchievement(progressPoint);
    GUIManager.instance.SetHeroTitle(progressPoint.localizedTitle, progressPoint.clip);
    GameHandler.GetService<RichPresenceService>().SetState(GetRichPresenceState(progressPoint));

    static RichPresenceState GetRichPresenceState(MountainProgressHandler.ProgressPoint p)
    {
      switch (p.title)
      {
        case "ALPINE":
          return RichPresenceState.Status_Alpine;
        case "CALDERA":
          return RichPresenceState.Status_Caldera;
        case "MESA":
          return RichPresenceState.Status_Mesa;
        case "PEAK":
          return RichPresenceState.Status_Peak;
        case "SHORE":
          return RichPresenceState.Status_Shore;
        case "THE KILN":
          return RichPresenceState.Status_Kiln;
        case "TROPICS":
          return RichPresenceState.Status_Tropics;
        default:
          Debug.LogError((object) ("Failed to find Rich Presence State from " + p.title));
          return RichPresenceState.Status_Shore;
      }
    }
  }

  public bool IsAtPeak(Transform tf) => this.IsAtPeak(tf.position);

  public bool IsAtPeak(Vector3 position)
  {
    return this.progressPoints != null && this.progressPoints.Length != 0 && (double) position.z > (double) ((IEnumerable<MountainProgressHandler.ProgressPoint>) this.progressPoints).Last<MountainProgressHandler.ProgressPoint>().transform.position.z;
  }

  private bool CheckReached(MountainProgressHandler.ProgressPoint point)
  {
    return (bool) (UnityEngine.Object) Character.localCharacter && (double) Character.localCharacter.Center.z > (double) point.transform.position.z && !Character.localCharacter.data.dead && (Singleton<MapHandler>.Instance.BiomeIsPresent(point.biome) || point.biome == Biome.BiomeType.Peak);
  }

  private void CheckAreaAchievement(MountainProgressHandler.ProgressPoint pointReached)
  {
    if (Character.localCharacter.data.dead)
      return;
    Debug.Log((object) ("Checking achievement. We just reached: " + pointReached.title));
    for (int index = 0; index < this.progressPoints.Length && this.progressPoints[index].achievement != pointReached.achievement; ++index)
    {
      if (this.progressPoints[index].achievement != ACHIEVEMENTTYPE.NONE)
        Singleton<AchievementManager>.Instance.ThrowAchievement(this.progressPoints[index].achievement);
      if (this.progressPoints[index].biome == Biome.BiomeType.Mesa)
        Singleton<AchievementManager>.Instance.TestCoolCucumberAchievement();
      else if (this.progressPoints[index].biome == Biome.BiomeType.Alpine)
        Singleton<AchievementManager>.Instance.TestBundledUpAchievement();
    }
  }

  [Serializable]
  public class ProgressPoint
  {
    public Transform transform;
    public string title;
    public AudioClip clip;
    public ACHIEVEMENTTYPE achievement;
    public Biome.BiomeType biome;

    public bool Reached { get; set; }

    public string localizedTitle => LocalizedText.GetText(this.title);
  }
}
