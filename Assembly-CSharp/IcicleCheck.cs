// Decompiled with JetBrains decompiler
// Type: IcicleCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class IcicleCheck : CustomSpawnCondition
{
  public BoxCollider boxCollider;
  public Vector2 minMax;
  public Vector2 minMaxScale = new Vector2(1f, 1f);
  public Vector3 localStart = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 localEnd = new Vector3(0.0f, 5f, 0.0f);

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    PropSpawner comp = this.GetComponentInParent<PropSpawner>();
    this.transform.localScale = this.minMaxScale.PRndRange().xxx();
    Vector3 vector3 = this.boxCollider.transform.TransformPoint(this.boxCollider.center);
    Vector3 halfExtents = Vector3.Scale(this.boxCollider.transform.lossyScale, this.boxCollider.size) / 2f;
    if (!this.LineCheck())
      return false;
    Collider[] array = ((IEnumerable<Collider>) Physics.OverlapBox(vector3, halfExtents, this.boxCollider.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c.GetComponentInParent<PropSpawner>() != (UnityEngine.Object) comp)).ToArray<Collider>();
    foreach (Collider collider in array)
      Debug.DrawLine(vector3, collider.transform.position, Color.red);
    this.transform.position += Vector2.Scale((Vector2) this.transform.lossyScale, this.minMax).PRndRange().oxo();
    return array.Length == 0;
  }

  public bool LineCheck()
  {
    Vector3 vector3_1 = this.transform.TransformPoint(this.localStart);
    Vector3 vector3_2 = this.transform.TransformPoint(this.localEnd);
    bool flag = !(bool) (UnityEngine.Object) HelperFunctions.LineCheck(vector3_1, vector3_2, HelperFunctions.LayerType.TerrainMap).transform;
    Debug.DrawLine(vector3_1, vector3_2, flag ? Color.green : Color.red, 10f);
    return flag;
  }
}
