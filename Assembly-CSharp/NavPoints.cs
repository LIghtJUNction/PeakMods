// Decompiled with JetBrains decompiler
// Type: NavPoints
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class NavPoints : MonoBehaviour
{
  public static NavPoints instance;
  public bool drawGizmos;
  private List<NavPoint> points = new List<NavPoint>();

  private void Awake()
  {
    NavPoints.instance = this;
    this.points = new List<NavPoint>();
    this.points.AddRange((IEnumerable<NavPoint>) this.GetComponentsInChildren<NavPoint>());
  }

  private void OnDrawGizmos()
  {
    if (!this.drawGizmos)
      return;
    Gizmos.color = Color.blue;
    foreach (NavPoint point in this.points)
    {
      foreach (NavPoint connection in point.connections)
        Gizmos.DrawLine(point.transform.position, connection.transform.position);
    }
  }

  public void ConnectPoints()
  {
    this.points = new List<NavPoint>();
    this.points.AddRange((IEnumerable<NavPoint>) this.GetComponentsInChildren<NavPoint>());
    foreach (NavPoint point in this.points)
      this.CheckPoint(point);
    foreach (NavPoint point in this.points)
      point.MirrorConnections();
  }

  private void CheckPoint(NavPoint point)
  {
    point.connections = new List<NavPoint>();
    float num1 = float.PositiveInfinity;
    List<NavPoint> navPointList = new List<NavPoint>();
    foreach (NavPoint point1 in this.points)
    {
      if (!((Object) point1 == (Object) point) && !(bool) (Object) HelperFunctions.LineCheck(point.transform.position + Vector3.up, point1.transform.position + Vector3.up, HelperFunctions.LayerType.TerrainMap).transform)
      {
        navPointList.Add(point1);
        float num2 = Vector3.Distance(point.transform.position, point1.transform.position);
        if ((double) num2 < (double) num1)
          num1 = num2;
      }
    }
    float num3 = num1 * 1.5f;
    foreach (NavPoint navPoint in navPointList)
    {
      if ((double) Vector3.Distance(point.transform.position, navPoint.transform.position) < (double) num3)
        point.connections.Add(navPoint);
    }
  }

  internal NavPoint GetNavPoint(Vector3 destination, Vector3 currentPos)
  {
    NavPoint navPoint = (NavPoint) null;
    float num1 = float.PositiveInfinity;
    foreach (NavPoint point in this.points)
    {
      float num2 = Vector3.Distance(currentPos, point.transform.position);
      if ((double) num2 <= (double) num1 && (double) Vector3.Angle(destination - currentPos, point.transform.position - currentPos) <= 90.0)
      {
        num1 = num2;
        navPoint = point;
      }
    }
    return navPoint;
  }
}
