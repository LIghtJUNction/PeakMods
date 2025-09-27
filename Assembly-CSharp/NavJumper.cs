// Decompiled with JetBrains decompiler
// Type: NavJumper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class NavJumper : MonoBehaviour
{
  public int castsPerJump = 100;
  public float maxDistance = 3f;
  public float castRadius = 1f;
  public float castHeight = 100f;
  private int fails;

  private void Start()
  {
  }

  private void Jump()
  {
    List<RaycastHit> source = new List<RaycastHit>();
    for (int index = 0; index < this.castsPerJump; ++index)
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.position + (ExtMath.RandInsideUnitCircle() * this.castRadius).xny(this.castHeight), Vector3.down * this.castHeight, out hitInfo))
        source.Add(hitInfo);
    }
    Debug.Log((object) $"Total: {source.Count}");
    List<RaycastHit> list1 = source.Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (double) Vector3.Angle(hit.normal, Vector3.up) < 50.0)).ToList<RaycastHit>();
    Debug.Log((object) $"After angle: {list1.Count}");
    List<RaycastHit> list2 = list1.Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (double) Vector3.Distance(hit.point, this.transform.position) < (double) this.maxDistance)).ToList<RaycastHit>();
    Debug.Log((object) $"After distance: {list2.Count}");
    List<RaycastHit> list3 = list2.Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (double) hit.point.z > (double) this.transform.position.z && (double) hit.point.y > (double) this.transform.position.y)).ToList<RaycastHit>().Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (double) hit.point.y > (double) this.transform.position.y)).ToList<RaycastHit>();
    Debug.Log((object) $"After Z: {list3.Count}");
    if (list3.Count == 0)
      return;
    RaycastHit raycastHit = list3.OrderByDescending<RaycastHit, float>((Func<RaycastHit, float>) (hit => hit.point.z)).First<RaycastHit>();
    Debug.DrawLine(this.transform.position + Vector3.up, raycastHit.point + Vector3.up, Color.green, 10f);
    this.transform.position = raycastHit.point;
  }

  private void Update()
  {
  }
}
