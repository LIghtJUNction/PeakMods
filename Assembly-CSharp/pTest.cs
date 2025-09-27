// Decompiled with JetBrains decompiler
// Type: pTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class pTest : MonoBehaviour
{
  private NavMeshAgent agent;

  private void Awake()
  {
    this.agent = this.GetComponent<NavMeshAgent>();
    this.agent.updatePosition = false;
    this.agent.updateRotation = false;
  }

  private void Start()
  {
  }

  private void Update()
  {
  }

  private void OnDrawGizmosSelected()
  {
    BoxCollider boxCollider = this.GetComponent<BoxCollider>();
    Vector3 center = boxCollider.bounds.center;
    Collider[] array = ((IEnumerable<Collider>) Physics.OverlapBox(center, boxCollider.bounds.extents, boxCollider.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) boxCollider)).ToArray<Collider>();
    Debug.Log((object) $"position: {center}, extents: {boxCollider.bounds.extents}");
    foreach (UnityEngine.Object @object in array)
      Debug.Log((object) ("Collider: " + @object.name));
    Gizmos.color = array.Length != 0 ? Color.red : Color.green;
    Gizmos.DrawWireCube(center, boxCollider.bounds.extents * 2f);
  }
}
