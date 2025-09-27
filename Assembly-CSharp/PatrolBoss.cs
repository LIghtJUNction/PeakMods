// Decompiled with JetBrains decompiler
// Type: PatrolBoss
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class PatrolBoss : MonoBehaviour
{
  public static PatrolBoss me;
  public GameObject point;

  public void Awake() => PatrolBoss.me = this;

  public Vector3 GetPoint()
  {
    RaycastHit hitInfo;
    if (Physics.Raycast(this.point.transform.position + ExtMath.RandInsideUnitCircle().xoy() * 10f, Vector3.down, out hitInfo, 1000f, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
      return hitInfo.point;
    Debug.LogError((object) "This wrong");
    return Vector3.positiveInfinity;
  }
}
