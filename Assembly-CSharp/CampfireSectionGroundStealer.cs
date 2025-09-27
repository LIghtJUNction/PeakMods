// Decompiled with JetBrains decompiler
// Type: CampfireSectionGroundStealer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CampfireSectionGroundStealer : MonoBehaviour
{
  public float offset;
  public GameObject groundParent;

  private void Awake()
  {
    foreach (Transform transform in this.groundParent.transform)
    {
      if ((double) transform.GetComponent<MeshRenderer>().bounds.center.y > (double) this.transform.position.y + (double) this.offset)
        transform.SetParent(this.transform, true);
    }
  }
}
