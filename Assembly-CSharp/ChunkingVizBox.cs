// Decompiled with JetBrains decompiler
// Type: ChunkingVizBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ChunkingVizBox : MonoBehaviour
{
  public GameObject[] objects;
  private bool m_lastState = true;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(this.transform.position, this.transform.localScale);
  }

  private void LateUpdate()
  {
    bool flag = new Bounds(this.transform.position, this.transform.localScale).Contains(MainCamera.instance.transform.position);
    if (this.m_lastState != flag)
    {
      foreach (GameObject gameObject in this.objects)
        gameObject.SetActive(flag);
    }
    this.m_lastState = flag;
  }
}
