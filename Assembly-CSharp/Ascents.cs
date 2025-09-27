// Decompiled with JetBrains decompiler
// Type: Ascents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public static class Ascents
{
  internal static int _currentAscent;

  public static int currentAscent
  {
    get => Ascents._currentAscent;
    set
    {
      Ascents._currentAscent = value;
      Debug.Log((object) ("Ascent set to " + value.ToString()));
    }
  }

  public static bool fogEnabled => Ascents.currentAscent >= 0;

  public static float fallDamageMultiplier => Ascents.currentAscent < 1 ? 1f : 2f;

  public static float hungerRateMultiplier
  {
    get
    {
      if (Ascents.currentAscent == -1)
        return 0.7f;
      return Ascents.currentAscent >= 2 ? 1.6f : 1f;
    }
  }

  public static int itemWeightModifier => Ascents.currentAscent < 3 ? 0 : 1;

  public static bool shouldSpawnFlare => Ascents.currentAscent < 4;

  public static bool isNightCold => Ascents.currentAscent >= 5;

  public static float nightColdRate => 0.005f;

  public static bool canReviveDead => Ascents.currentAscent < 7;

  public static float climbStaminaMultiplier
  {
    get
    {
      if (Ascents.currentAscent >= 6)
        return 1.4f;
      return Ascents.currentAscent == -1 ? 0.7f : 1f;
    }
  }

  [ConsoleCommand]
  public static void UnlockAll()
  {
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 7);
  }

  [ConsoleCommand]
  public static void UnlockOne()
  {
    int num;
    if (!Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
      return;
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, num + 1);
  }

  [ConsoleCommand]
  public static void LockAll()
  {
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 0);
  }
}
