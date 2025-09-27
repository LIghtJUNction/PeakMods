// Decompiled with JetBrains decompiler
// Type: EnabledRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class EnabledRandom : MonoBehaviour
{
  public int odds = 1;

  private void Start()
  {
    this.odds = Random.Range(0, 4);
    if (this.odds >= 2)
      return;
    this.gameObject.SetActive(false);
  }
}
