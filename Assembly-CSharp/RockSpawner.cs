// Decompiled with JetBrains decompiler
// Type: RockSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RockSpawner : MonoBehaviour
{
  public Vector2 area;
  public GameObject[] rocks;
  public int nrOfSpawns = 500;
  public float downMove;
  public RockSpawner.OriginalRotation rotation;
  public bool raycast = true;
  public float minScale = 1f;
  public float maxScale = 2f;
  public float maxRotation = 1f;
  public float rotationPow;

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawLine(this.transform.position - this.area.y * 0.5f * this.transform.forward, this.transform.position + this.area.y * 0.5f * this.transform.forward);
    Gizmos.DrawLine(this.transform.position - this.area.x * 0.5f * this.transform.right, this.transform.position + this.area.x * 0.5f * this.transform.right);
  }

  public void Go()
  {
    this.Clear();
    for (int index = 0; index < this.nrOfSpawns; ++index)
      this.DoSpawn();
  }

  private void Clear()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.transform.GetChild(index).gameObject);
  }

  private void DoSpawn()
  {
    RockSpawner.ReturnData? randomPoint = this.GetRandomPoint();
    if (!randomPoint.HasValue)
      return;
    GameObject rock = this.rocks[Random.Range(0, this.rocks.Length)];
    Quaternion a = rock.transform.rotation;
    if (this.rotation == RockSpawner.OriginalRotation.RaycastNormal)
      a = HelperFunctions.GetRandomRotationWithUp(randomPoint.Value.normal);
    Quaternion rotation = Quaternion.Lerp(a, Random.rotation, Mathf.Pow(Random.value, this.rotationPow) * this.maxRotation);
    GameObject gameObject = Object.Instantiate<GameObject>(rock, randomPoint.Value.pos, rotation, this.transform);
    gameObject.transform.position += this.transform.up * -this.downMove;
    gameObject.transform.Rotate(this.transform.eulerAngles, Space.World);
    gameObject.transform.localScale *= Random.Range(this.minScale, this.maxScale);
    Physics.SyncTransforms();
  }

  private RockSpawner.ReturnData? GetRandomPoint()
  {
    Vector3 from = this.transform.position + this.transform.right * Mathf.Lerp((float) (-(double) this.area.x * 0.5), this.area.x * 0.5f, Random.value) + this.transform.forward * Mathf.Lerp((float) (-(double) this.area.y * 0.5), this.area.y * 0.5f, Random.value);
    if (!this.raycast)
      return new RockSpawner.ReturnData?(new RockSpawner.ReturnData()
      {
        pos = from,
        normal = Vector3.up
      });
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + this.transform.up * -5000f, HelperFunctions.LayerType.Terrain);
    if (!(bool) (Object) raycastHit.transform)
      return new RockSpawner.ReturnData?();
    return new RockSpawner.ReturnData?(new RockSpawner.ReturnData()
    {
      pos = raycastHit.point,
      normal = raycastHit.normal
    });
  }

  public enum OriginalRotation
  {
    PrefabRotation,
    RaycastNormal,
  }

  private struct ReturnData
  {
    public Vector3 pos;
    public Vector3 normal;
  }
}
