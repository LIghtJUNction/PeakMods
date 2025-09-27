// Decompiled with JetBrains decompiler
// Type: SimpleDrawMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SimpleDrawMesh : MonoBehaviour
{
  public DrawPool[] drawPools;
  private bool poolsGathered;
  private Matrix4x4[] matrices;
  public float cullDistance = 10f;
  public Transform distanceCheckObject;

  private void Start() => this.GatherPools();

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.distanceCheckObject.position, this.cullDistance);
  }

  private void Update() => this.drawMeshes();

  public void drawMeshes()
  {
    if (!this.poolsGathered || (bool) (Object) Character.localCharacter && (bool) (Object) this.distanceCheckObject && (double) Vector3.Distance(Character.localCharacter.Center, this.distanceCheckObject.position) > (double) this.cullDistance)
      return;
    for (int index = 0; index < this.drawPools.Length; ++index)
      Graphics.DrawMeshInstanced(this.drawPools[index].mesh, 0, this.drawPools[index].material, this.drawPools[index].matricies, this.drawPools[index].matricies.Length);
  }

  public void GatherPools()
  {
    for (int index1 = 0; index1 < this.drawPools.Length; ++index1)
    {
      Transform[] componentsInChildren = this.drawPools[index1].transformsParent.GetComponentsInChildren<Transform>();
      this.drawPools[index1].matricies = new Matrix4x4[componentsInChildren.Length];
      for (int index2 = 1; index2 < componentsInChildren.Length; ++index2)
        this.drawPools[index1].matricies[index2] = Matrix4x4.TRS(componentsInChildren[index2].position, componentsInChildren[index2].rotation, componentsInChildren[index2].localScale);
    }
    this.poolsGathered = true;
  }
}
