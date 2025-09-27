// Decompiled with JetBrains decompiler
// Type: StupidRockPlacerHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StupidRockPlacerHandler : MonoBehaviour
{
  public float amount = 1f;

  private void Start()
  {
  }

  private void ReDo()
  {
    StupidRockPlacer[] componentsInChildren = this.GetComponentsInChildren<StupidRockPlacer>();
    foreach (StupidRockPlacer stupidRockPlacer in componentsInChildren)
      stupidRockPlacer.Clear();
    foreach (StupidRockPlacer stupidRockPlacer in componentsInChildren)
    {
      int amount = stupidRockPlacer.amount;
      stupidRockPlacer.amount = (int) ((double) this.amount * (double) stupidRockPlacer.amount);
      stupidRockPlacer.Go();
      stupidRockPlacer.amount = amount;
    }
  }

  private void Clear()
  {
    foreach (StupidRockPlacer componentsInChild in this.GetComponentsInChildren<StupidRockPlacer>())
      componentsInChild.Clear();
  }

  private void Update()
  {
  }
}
