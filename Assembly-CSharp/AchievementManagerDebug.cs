// Decompiled with JetBrains decompiler
// Type: AchievementManagerDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class AchievementManagerDebug : SerializedMonoBehaviour
{
  private AchievementManager achievementManager;
  [SerializeField]
  public Dictionary<RUNBASEDVALUETYPE, int> runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();

  private void Awake() => this.achievementManager = this.GetComponent<AchievementManager>();

  private void Update()
  {
    this.runBasedInts = this.achievementManager.runBasedValueData.runBasedInts;
  }
}
