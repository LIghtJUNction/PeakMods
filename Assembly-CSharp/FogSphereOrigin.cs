// Decompiled with JetBrains decompiler
// Type: FogSphereOrigin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FogSphereOrigin : MonoBehaviour
{
  public float size = 650f;
  public float moveOnHeight;
  public float moveOnForward;
  public bool disableFog;

  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawCube(new Vector3(0.0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 200f, 2f));
    Gizmos.color = Color.green;
    Gizmos.DrawCube(new Vector3(0.0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 2f, 1000f));
  }
}
