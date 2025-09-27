// Decompiled with JetBrains decompiler
// Type: PlayerGizmos
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PlayerGizmos : MonoBehaviour
{
  public List<GizmoInstance> gizmos = new List<GizmoInstance>();
  public static PlayerGizmos instance;
  public GameObject pointer;

  private void Start() => PlayerGizmos.instance = this;

  private void Update()
  {
    for (int index = this.gizmos.Count - 1; index >= 0; --index)
    {
      GizmoInstance gizmo = this.gizmos[index];
      if (gizmo == null)
      {
        this.gizmos.RemoveAt(index);
      }
      else
      {
        ++gizmo.framesSinceActivated;
        if (gizmo.framesSinceActivated > 5)
        {
          gizmo.giz.SetActive(false);
          this.gizmos.Remove(gizmo);
        }
      }
    }
  }

  public void DisplayGizmo(PlayerGizmos.GizmoType gizmoType, Vector3 pos, Vector3 direction)
  {
    GameObject gizmo = this.GetGizmo(gizmoType);
    GizmoInstance gizmoInstance = this.Contains(gizmo);
    if (gizmoInstance != null)
      gizmoInstance.framesSinceActivated = 0;
    else
      this.gizmos.Add(new GizmoInstance()
      {
        giz = gizmo,
        framesSinceActivated = 0
      });
    gizmo.SetActive(true);
    gizmo.transform.position = pos;
    gizmo.transform.rotation = Quaternion.LookRotation(direction);
  }

  private GameObject GetGizmo(PlayerGizmos.GizmoType gizmoType)
  {
    return gizmoType == PlayerGizmos.GizmoType.Pointer ? this.pointer : (GameObject) null;
  }

  private GizmoInstance Contains(GameObject gizmo)
  {
    foreach (GizmoInstance gizmo1 in this.gizmos)
    {
      if ((Object) gizmo1.giz == (Object) gizmo)
        return gizmo1;
    }
    return (GizmoInstance) null;
  }

  public enum GizmoType
  {
    Pointer,
  }
}
