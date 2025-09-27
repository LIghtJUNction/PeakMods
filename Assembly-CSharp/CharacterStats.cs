// Decompiled with JetBrains decompiler
// Type: CharacterStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterStats : MonoBehaviour
{
  public static float peakHeightInUnits = 1200f;
  private Character character;
  public float heightInUnits;
  public float heightInMeters;
  public static float unitsToMeters = 1.6f;
  private float tick;
  public float tickRate = 1f;
  public List<EndScreen.TimelineInfo> timelineInfo = new List<EndScreen.TimelineInfo>();
  public bool won;
  public bool lost;
  public bool somebodyElseWon;
  public bool justDied;
  public bool justPassedOut;
  public bool justRevived;

  private void Awake() => this.character = this.GetComponentInParent<Character>();

  private void Start()
  {
    this.RecordHeight();
    this.Record();
  }

  public void GetCaughtUp()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    List<EndScreen.TimelineInfo> timelineInfo = Character.localCharacter.refs.stats.timelineInfo;
    for (int index = 0; index < timelineInfo.Count; ++index)
      this.timelineInfo.Add(new EndScreen.TimelineInfo()
      {
        time = timelineInfo[index].time,
        height = this.heightInUnits
      });
  }

  private void RecordHeight()
  {
    this.heightInUnits = this.character.HipPos().y;
    this.heightInMeters = (float) Mathf.RoundToInt(this.heightInUnits * CharacterStats.unitsToMeters);
    if (!this.character.IsLocal || this.character.data.dead)
      return;
    Singleton<AchievementManager>.Instance.RecordMaxHeight(Mathf.RoundToInt(this.heightInMeters));
  }

  private void Update()
  {
    this.RecordHeight();
    this.tick += Time.deltaTime;
    if ((double) this.tick <= (double) this.tickRate || this.won || this.lost)
      return;
    this.tick = 0.0f;
    if (!this.character.IsLocal && this.timelineInfo.Count == 1)
      this.GetCaughtUp();
    this.Record();
  }

  public EndScreen.TimelineInfo GetFirstTimelineInfo() => this.timelineInfo[0];

  public EndScreen.TimelineInfo GetFinalTimelineInfo()
  {
    return this.timelineInfo[this.timelineInfo.Count - 1];
  }

  public static int UnitsToMeters(float units)
  {
    return Mathf.RoundToInt(Mathf.Min(units, CharacterStats.peakHeightInUnits) * CharacterStats.unitsToMeters);
  }

  public void Record(bool useOverridePosition = false, float overrideHeight = 0.0f)
  {
    EndScreen.TimelineInfo timelineInfo = new EndScreen.TimelineInfo();
    timelineInfo.height = this.heightInUnits;
    if (useOverridePosition)
      timelineInfo.height = overrideHeight;
    if ((double) timelineInfo.height > 2000.0)
      return;
    timelineInfo.time = Time.time;
    if (this.justDied)
    {
      this.justDied = false;
      timelineInfo.died = true;
    }
    else if (this.character.data.dead)
      timelineInfo.dead = true;
    if (this.justRevived)
    {
      this.justRevived = false;
      timelineInfo.revived = true;
      Debug.LogError((object) "RECORD REVIVED!");
    }
    else
    {
      if (this.justPassedOut)
      {
        this.justPassedOut = false;
        timelineInfo.justPassedOut = true;
      }
      if (this.character.data.passedOut)
        timelineInfo.passedOut = true;
    }
    this.timelineInfo.Add(timelineInfo);
  }

  public void Win()
  {
    this.won = true;
    if (!this.character.IsLocal)
      return;
    EndScreen.TimelineInfo timelineInfo = this.timelineInfo[this.timelineInfo.Count - 1] with
    {
      won = true
    };
    GlobalEvents.TriggerLocalCharacterWonRun();
    this.timelineInfo[this.timelineInfo.Count - 1] = timelineInfo;
  }

  public void Lose(bool somebodyElseWon)
  {
    this.lost = true;
    this.somebodyElseWon = somebodyElseWon;
  }
}
