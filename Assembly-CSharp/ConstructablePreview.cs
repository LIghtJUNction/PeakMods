// Decompiled with JetBrains decompiler
// Type: ConstructablePreview
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ConstructablePreview : MonoBehaviour
{
  public GameObject enableIfValid;
  public GameObject enableIfInvalid;
  public List<ConstructablePreview.ConstructablePreviewAvoidanceSphere> avoidanceSpheres;

  public void SetValid(bool valid)
  {
    this.enableIfValid.SetActive(valid);
    this.enableIfInvalid.SetActive(!valid);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere avoidanceSphere in this.avoidanceSpheres)
      Gizmos.DrawWireSphere(this.transform.TransformPoint(avoidanceSphere.position), avoidanceSphere.radius);
  }

  public bool CollisionValid()
  {
    foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere avoidanceSphere in this.avoidanceSpheres)
    {
      if (Physics.CheckSphere(this.transform.TransformPoint(avoidanceSphere.position), avoidanceSphere.radius, (int) HelperFunctions.GetMask(avoidanceSphere.layerType), QueryTriggerInteraction.Ignore))
        return false;
    }
    return true;
  }

  [Serializable]
  public class ConstructablePreviewAvoidanceSphere
  {
    public Vector3 position;
    public float radius;
    public HelperFunctions.LayerType layerType;
  }
}
