// Decompiled with JetBrains decompiler
// Type: BingBongSpawnTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BingBongSpawnTool : MonoBehaviour
{
  public float spawnRate = 0.1f;
  public bool auto = true;
  public string folder = "0_Items/";
  public GameObject objectToSpawn;
  public bool bingbongInit;
  public BingBongSpawnTool.SpawnPos pos;
  public BingBongSpawnTool.SpawnRot rot;
  public float randomPosRadius;
  public float normalOffsetPos;
  private float counter;

  private void Update()
  {
    this.counter += Time.unscaledDeltaTime;
    if ((double) this.counter < (double) this.spawnRate || !this.auto && !Input.GetKeyDown(KeyCode.Mouse0) || this.auto && !Input.GetKey(KeyCode.Mouse0))
      return;
    this.counter = 0.0f;
    this.Spawn();
  }

  private void Spawn()
  {
    GameObject gameObject = PhotonNetwork.Instantiate(this.folder + this.objectToSpawn.name, this.GetPosition(), this.GetRotation());
    if (!this.bingbongInit)
      return;
    gameObject.GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.AllBuffered, (object) this.GetComponentInParent<PhotonView>().ViewID);
  }

  public Vector3 GetPosition()
  {
    Vector3 vector3 = this.transform.position;
    if (this.pos == BingBongSpawnTool.SpawnPos.RaycastPos)
    {
      RaycastHit raycastHit = HelperFunctions.LineCheck(this.transform.position, this.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical);
      if ((bool) (Object) raycastHit.transform)
        vector3 = raycastHit.point + raycastHit.normal * this.normalOffsetPos;
    }
    else if (this.pos == BingBongSpawnTool.SpawnPos.BingBong)
      vector3 = this.transform.TransformPoint(Vector3.forward * 2f);
    return vector3 + Random.insideUnitSphere * this.randomPosRadius;
  }

  public Quaternion GetRotation()
  {
    if (this.rot == BingBongSpawnTool.SpawnRot.BingBongRotation)
      return this.transform.rotation;
    if (this.rot == BingBongSpawnTool.SpawnRot.Random)
      return Random.rotation;
    if (this.rot == BingBongSpawnTool.SpawnRot.RaycastNormal)
      return Quaternion.LookRotation(HelperFunctions.LineCheck(this.transform.position, this.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical).normal);
    int rot = (int) this.rot;
    return Quaternion.identity;
  }

  public enum SpawnPos
  {
    BingBong,
    RaycastPos,
  }

  public enum SpawnRot
  {
    BingBongRotation,
    Random,
    RaycastNormal,
    Identity,
  }
}
