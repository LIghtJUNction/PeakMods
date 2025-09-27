// Decompiled with JetBrains decompiler
// Type: GameBooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class GameBooter
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  public static void Initialize() => GameBooter.AutoBoot();

  public static void AutoBoot()
  {
    GameObject gameObject = new GameObject("Game");
    gameObject.AddComponent<GameHandler>().Initialize();
    gameObject.AddComponent<UIInputHandler>().Initialize();
  }
}
