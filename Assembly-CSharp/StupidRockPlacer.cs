// Decompiled with JetBrains decompiler
// Type: StupidRockPlacer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class StupidRockPlacer : MonoBehaviour
{
  public List<GameObject> rocks;
  public Transform pieceRoot;
  public int amount = 10;
  public Transform rockParent;
  public List<GameObject> lastPlaced = new List<GameObject>();

  public Vector3 size => this.transform.localScale.xyz();

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawWireCube(this.transform.position + this.size / 2f, this.size);
  }

  public void Clear()
  {
    if (!(bool) (UnityEngine.Object) this.rockParent)
      return;
    this.rockParent.KillAllChildren(true, undoable: true);
  }

  private void Start()
  {
  }

  private void ValidatePool()
  {
    foreach (Transform transform in ((IEnumerable<Transform>) this.pieceRoot.GetComponentsInChildren<Transform>()).Where<Transform>((Func<Transform, bool>) (t => (UnityEngine.Object) t != (UnityEngine.Object) this.pieceRoot)).ToList<Transform>())
    {
      transform.gameObject.GetOrAddComponent<PutMeInWall>();
      transform.gameObject.layer = LayerMask.NameToLayer("Terrain");
      PExt.DirtyObj((UnityEngine.Object) transform.gameObject);
    }
  }

  public void Go()
  {
    this.rockParent = (Transform) null;
    this.rockParent = this.transform.parent.Find("Rocks: " + this.gameObject.name);
    if (!(bool) (UnityEngine.Object) this.rockParent)
    {
      this.rockParent = new GameObject("Rocks: " + this.gameObject.name).transform;
      this.rockParent.SetParent(this.transform.parent);
    }
    this.rockParent.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
    this.rocks = ((IEnumerable<PutMeInWall>) this.pieceRoot.GetComponentsInChildren<PutMeInWall>()).Select<PutMeInWall, GameObject>((Func<PutMeInWall, GameObject>) (x => x.gameObject)).ToList<GameObject>();
    this.lastPlaced = new List<GameObject>();
    int num = 0;
    for (int index = 0; index < this.amount || num > this.amount * 10; ++index)
    {
      ++num;
      Vector3 startCast = this.transform.position + new Vector3(this.size.x.Rand(), this.size.y.Rand(), 0.0f);
      GameObject random = this.rocks.GetRandom<GameObject>();
      Vector3? wallPosition = random.GetComponent<PutMeInWall>().GetWallPosition(startCast, this.transform.localScale.z);
      if (!wallPosition.HasValue)
      {
        --index;
      }
      else
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(random, wallPosition.Value, ExtQuaternion.RandomRotation());
        gameObject.transform.SetParent(this.rockParent);
        PutMeInWall component;
        if (!gameObject.TryGetComponent<PutMeInWall>(out component))
          component = gameObject.AddComponent<PutMeInWall>();
        component.gameObject.SetActive(true);
        this.lastPlaced.Add(gameObject);
        component.RandomScale();
        Physics.SyncTransforms();
        PExt.DirtyObj((UnityEngine.Object) gameObject);
      }
    }
  }

  public void RemoveLastPlaced()
  {
    foreach (UnityEngine.Object @object in this.lastPlaced)
    {
      int num = @object == (UnityEngine.Object) null ? 1 : 0;
    }
    this.lastPlaced = new List<GameObject>();
  }

  private void Update()
  {
  }
}
