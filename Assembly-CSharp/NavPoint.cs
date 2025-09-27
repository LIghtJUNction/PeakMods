// Decompiled with JetBrains decompiler
// Type: NavPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(1000)]
public class NavPoint : MonoBehaviour
{
  public List<NavPoint> connections = new List<NavPoint>();

  internal NavPoint GetNext(Vector3 targetDirection)
  {
    List<NavPoint> navPointList = new List<NavPoint>();
    foreach (NavPoint connection in this.connections)
    {
      if ((double) HelperFunctions.FlatAngle(targetDirection, connection.transform.position - this.transform.position) < 90.0)
        navPointList.Add(connection);
    }
    return navPointList.Count == 0 ? (NavPoint) null : navPointList[Random.Range(0, navPointList.Count)];
  }

  internal void MirrorConnections()
  {
    foreach (NavPoint connection in this.connections)
    {
      if (!connection.connections.Contains(this))
        connection.connections.Add(this);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    foreach (Component connection in this.connections)
      Gizmos.DrawLine(this.transform.position + Vector3.up * 0.1f, connection.transform.position + Vector3.up * 0.1f);
  }
}
