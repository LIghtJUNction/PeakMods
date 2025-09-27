// Decompiled with JetBrains decompiler
// Type: PutMeInWall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class PutMeInWall : MonoBehaviour
{
  public Vector2 penetrationRnage;
  public Vector2 scaleRange = new Vector2(1f, 1f);
  public bool checkBelow;
  public float belowMargin = 1f;
  public float angle = -1f;

  private void Go() => this.PutInTheWall();

  public bool PutInTheWall()
  {
    Vector3 vector3_1 = this.transform.position - Vector3.forward * 50f;
    RaycastHit[] source1 = Physics.RaycastAll(vector3_1, Vector3.forward, 500f);
    Debug.DrawLine(vector3_1, vector3_1 + Vector3.forward * 100f, Color.red, 10f);
    Debug.Log((object) $"hits: {source1.Length}");
    Debug.Log((object) $"list{source1}");
    RaycastHit raycastHit = ((IEnumerable<RaycastHit>) ((IEnumerable<RaycastHit>) source1).OrderBy<RaycastHit, float>((Func<RaycastHit, float>) (h => h.distance)).ToArray<RaycastHit>()).First<RaycastHit>((Func<RaycastHit, bool>) (h => (UnityEngine.Object) h.collider.gameObject != (UnityEngine.Object) this.gameObject));
    Vector3 vector3_2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
    Collider component = this.GetComponent<Collider>();
    if ((double) this.angle > 0.0 && (double) Vector2.Angle((Vector2) raycastHit.normal, Vector2.up) <= (double) this.angle)
      return false;
    if (this.checkBelow)
    {
      Vector3 origin = vector3_2;
      Bounds bounds = component.bounds;
      Vector3 extents = bounds.extents;
      double magnitude1 = (double) extents.magnitude;
      Vector3 down1 = Vector3.down;
      bounds = component.bounds;
      extents = bounds.extents;
      double maxDistance = (double) extents.magnitude * (double) this.belowMargin;
      RaycastHit[] source2 = Physics.SphereCastAll(origin, (float) magnitude1, down1, (float) maxDistance);
      Debug.Log((object) $"belowHits: {source2.Length}");
      RaycastHit[] array = ((IEnumerable<RaycastHit>) source2).Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (UnityEngine.Object) hit.collider.gameObject != (UnityEngine.Object) this.gameObject && (UnityEngine.Object) hit.collider.gameObject != (UnityEngine.Object) raycastHit.collider.gameObject)).ToArray<RaycastHit>();
      Debug.Log((object) $"belowHits2: {array.Length}");
      if (array.Length != 0)
      {
        foreach (RaycastHit raycastHit1 in array)
          Debug.Log((object) $"hit: {raycastHit1.collider.gameObject}");
        Vector3 start = vector3_2;
        Vector3 vector3_3 = vector3_2;
        Vector3 down2 = Vector3.down;
        bounds = component.bounds;
        double num1 = (double) bounds.extents.magnitude * (double) this.belowMargin;
        bounds = component.bounds;
        double magnitude2 = (double) bounds.extents.magnitude;
        double num2 = num1 + magnitude2;
        Vector3 vector3_4 = down2 * (float) num2;
        Vector3 end = vector3_3 + vector3_4;
        Color red = Color.red;
        Debug.DrawLine(start, end, red, 10f);
        return false;
      }
      Vector3 start1 = vector3_2;
      Vector3 vector3_5 = vector3_2;
      Vector3 down3 = Vector3.down;
      bounds = component.bounds;
      double num3 = (double) bounds.extents.magnitude * (double) this.belowMargin;
      bounds = component.bounds;
      double magnitude3 = (double) bounds.extents.magnitude;
      double num4 = num3 + magnitude3;
      Vector3 vector3_6 = down3 * (float) num4;
      Vector3 end1 = vector3_5 + vector3_6;
      Color green = Color.green;
      Debug.DrawLine(start1, end1, green, 10f);
    }
    Debug.Log((object) raycastHit.collider.gameObject, (UnityEngine.Object) raycastHit.collider.gameObject);
    this.transform.position = vector3_2;
    return true;
  }

  public Vector3? GetWallPosition2(Vector3 startCast, float maxDistance = 100f)
  {
    Vector3 vector3_1 = startCast - Vector3.forward * 50f;
    maxDistance += 50f;
    RaycastHit[] source = Physics.RaycastAll(vector3_1, Vector3.forward, maxDistance);
    Debug.DrawLine(vector3_1, vector3_1 + Vector3.forward * maxDistance, Color.red, 10f);
    Debug.Log((object) $"hits: {source.Length}");
    Debug.Log((object) $"list{source}");
    RaycastHit raycastHit = ((IEnumerable<RaycastHit>) ((IEnumerable<RaycastHit>) source).OrderBy<RaycastHit, float>((Func<RaycastHit, float>) (h => h.distance)).ToArray<RaycastHit>()).First<RaycastHit>((Func<RaycastHit, bool>) (h => (UnityEngine.Object) h.collider.gameObject != (UnityEngine.Object) this.gameObject));
    Vector3 vector3_2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
    Collider component = this.GetComponent<Collider>();
    if ((double) this.angle > 0.0 && (double) Vector2.Angle((Vector2) raycastHit.normal, Vector2.up) <= (double) this.angle)
      return new Vector3?();
    if (this.checkBelow)
    {
      Vector3 origin = vector3_2;
      Bounds bounds = component.bounds;
      double magnitude1 = (double) bounds.extents.magnitude;
      Vector3 down1 = Vector3.down;
      bounds = component.bounds;
      double maxDistance1 = (double) bounds.extents.magnitude * (double) this.belowMargin;
      if (((IEnumerable<RaycastHit>) Physics.SphereCastAll(origin, (float) magnitude1, down1, (float) maxDistance1)).Where<RaycastHit>((Func<RaycastHit, bool>) (hit => (UnityEngine.Object) hit.collider.gameObject != (UnityEngine.Object) this.gameObject && (UnityEngine.Object) hit.collider.gameObject != (UnityEngine.Object) raycastHit.collider.gameObject)).ToArray<RaycastHit>().Length != 0)
      {
        Vector3 start = vector3_2;
        Vector3 vector3_3 = vector3_2;
        Vector3 down2 = Vector3.down;
        bounds = component.bounds;
        Vector3 extents = bounds.extents;
        double num1 = (double) extents.magnitude * (double) this.belowMargin;
        bounds = component.bounds;
        extents = bounds.extents;
        double magnitude2 = (double) extents.magnitude;
        double num2 = num1 + magnitude2;
        Vector3 vector3_4 = down2 * (float) num2;
        Vector3 end = vector3_3 + vector3_4;
        Color red = Color.red;
        Debug.DrawLine(start, end, red, 10f);
        return new Vector3?();
      }
      Vector3 start1 = vector3_2;
      Vector3 vector3_5 = vector3_2;
      Vector3 down3 = Vector3.down;
      bounds = component.bounds;
      Vector3 extents1 = bounds.extents;
      double num3 = (double) extents1.magnitude * (double) this.belowMargin;
      bounds = component.bounds;
      extents1 = bounds.extents;
      double magnitude3 = (double) extents1.magnitude;
      double num4 = num3 + magnitude3;
      Vector3 vector3_6 = down3 * (float) num4;
      Vector3 end1 = vector3_5 + vector3_6;
      Color green = Color.green;
      Debug.DrawLine(start1, end1, green, 10f);
    }
    Debug.Log((object) raycastHit.collider.gameObject, (UnityEngine.Object) raycastHit.collider.gameObject);
    return new Vector3?(vector3_2);
  }

  public Vector3? GetWallPosition(Vector3 startCast, float maxDistance = 100f)
  {
    Vector3 vector3 = startCast - Vector3.forward * 50f;
    maxDistance += 50f;
    RaycastHit[] source = Physics.RaycastAll(vector3, Vector3.forward, maxDistance, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.Terrain));
    if ((double) this.angle > 0.0)
      source = ((IEnumerable<RaycastHit>) source).Where<RaycastHit>((Func<RaycastHit, bool>) (h => (double) Vector2.Angle((Vector2) h.normal, Vector2.up) > (double) this.angle)).ToArray<RaycastHit>();
    RaycastHit[] array = ((IEnumerable<RaycastHit>) source).OrderBy<RaycastHit, float>((Func<RaycastHit, float>) (h => h.distance)).ToArray<RaycastHit>();
    Debug.DrawLine(vector3, vector3 + Vector3.up * maxDistance, Color.green, 10f);
    Debug.DrawLine(vector3, vector3 + Vector3.forward * maxDistance, Color.red, 10f);
    Debug.Log((object) $"hits: {array.Length}");
    Debug.Log((object) $"list{array}");
    RaycastHit[] raycastHitArray = array;
    int index = 0;
    return index < raycastHitArray.Length ? new Vector3?(raycastHitArray[index].point + Vector3.forward * this.penetrationRnage.PRndRange()) : new Vector3?();
  }

  public void RandomRotation()
  {
    this.transform.rotation = Quaternion.Euler((float) UnityEngine.Random.Range(0, 360), (float) UnityEngine.Random.Range(0, 360), (float) UnityEngine.Random.Range(0, 360));
  }

  public void RandomScale() => this.transform.localScale *= this.scaleRange.PRndRange();

  private void Start()
  {
  }

  private void Update()
  {
  }
}
