// Decompiled with JetBrains decompiler
// Type: WallNavigatorTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class WallNavigatorTest : MonoBehaviour, ISerializationCallbackReceiver
{
  public float fDistance = 3f;
  public NavMeshSurface surface;
  private NavMeshTriangulation triangulation;
  public float sphereSize;
  private Color color;

  private void Start()
  {
  }

  private void Update()
  {
  }

  private void TryFindValidPath()
  {
    this.color = Color.red;
    if (((IEnumerable<Vector3>) this.triangulation.vertices).Where<Vector3>((Func<Vector3, bool>) (vert => (double) Vector3.Distance(this.transform.position, vert) < (double) this.sphereSize)).ToList<Vector3>().Count <= 0)
      return;
    this.color = Color.green;
  }

  private void Print()
  {
    Debug.Log((object) $"Verts{this.triangulation.vertices.Length}, Indices{this.triangulation.indices.Length}, Areas{this.triangulation.areas.Length}");
  }

  private void OnDrawGizmosSelected()
  {
    this.TryFindValidPath();
    Gizmos.color = this.color;
    Gizmos.DrawWireSphere(this.transform.position, this.sphereSize);
  }

  public void OnBeforeSerialize() => this.triangulation = NavMesh.CalculateTriangulation();

  public void OnAfterDeserialize()
  {
  }
}
