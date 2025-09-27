// Decompiled with JetBrains decompiler
// Type: MagicBean
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MagicBean : ItemComponent
{
  private bool isPlanted;
  public float timeToPlant;
  public MagicBeanVine plantPrefab;
  public float snapToVerticalAngle = 15f;
  private List<Vector3> raycastSpotsTest = new List<Vector3>();
  private RaycastHit raycastResult;
  private Vector3 averageNormal;

  public void Update()
  {
    if (!this.photonView.IsMine)
      return;
    if (this.item.itemState == ItemState.Held)
    {
      this.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData = true;
    }
    else
    {
      if (!PhotonNetwork.IsMasterClient || !this.isPlanted)
        return;
      this.timeToPlant -= Time.deltaTime;
      if ((double) this.timeToPlant > 0.0)
        return;
      float vineDistance = this.GetVineDistance(this.transform.position, this.averageNormal);
      this.photonView.RPC("GrowVineRPC", RpcTarget.All, (object) this.transform.position, (object) this.averageNormal, (object) vineDistance);
      this.GrowVineRPC(this.transform.position, this.averageNormal, vineDistance);
      PhotonNetwork.Destroy(this.gameObject);
    }
  }

  private void DebugValue()
  {
    if (this.HasData(DataEntryKey.Used))
      Debug.Log((object) this.GetData<BoolItemData>(DataEntryKey.Used).Value);
    else
      Debug.Log((object) "No data");
  }

  public override void OnInstanceDataSet()
  {
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!this.photonView.IsMine || this.item.itemState != ItemState.Ground || !this.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData || !HelperFunctions.IsLayerInLayerMask(HelperFunctions.LayerType.TerrainMap, collision.gameObject.layer))
      return;
    this.item.SetKinematicNetworked(true, this.item.transform.position, this.item.transform.rotation);
    this.DoNormalRaycasts(collision.contacts[0].point, collision.contacts[0].normal);
    this.isPlanted = true;
  }

  private float GetVineDistance(Vector3 startPos, Vector3 direction)
  {
    RaycastHit[] raycastHitArray = HelperFunctions.LineCheckAll(startPos, startPos + direction * this.plantPrefab.maxLength, HelperFunctions.LayerType.TerrainMap);
    float vineDistance = this.plantPrefab.maxLength;
    foreach (RaycastHit raycastHit in raycastHitArray)
    {
      if ((double) raycastHit.distance > 0.699999988079071 && (double) raycastHit.distance < (double) vineDistance)
        vineDistance = raycastHit.distance;
    }
    return vineDistance;
  }

  [PunRPC]
  protected void GrowVineRPC(Vector3 pos, Vector3 direction, float maxLength)
  {
    MagicBeanVine magicBeanVine = Object.Instantiate<MagicBeanVine>(this.plantPrefab, pos, Quaternion.identity);
    magicBeanVine.transform.up = direction;
    magicBeanVine.maxLength = maxLength;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    foreach (Vector3 center in this.raycastSpotsTest)
    {
      Gizmos.DrawSphere(center, 0.1f);
      Gizmos.DrawLine(this.transform.position, this.transform.position + this.averageNormal);
    }
  }

  private void TestRaycast() => this.DoNormalRaycasts(this.transform.position, Vector3.up);

  private void DoNormalRaycasts(Vector3 centralHit, Vector3 centralNormal)
  {
    this.raycastSpotsTest.Clear();
    List<Vector3> vector3List = new List<Vector3>();
    float num = 0.2f;
    for (int x = -1; x <= 1; ++x)
    {
      for (int z = -1; z <= 1; ++z)
      {
        if (x != 0 || z != 0)
        {
          Vector3 vector3 = Vector3.ProjectOnPlane(new Vector3((float) x, 0.0f, (float) z), centralNormal).normalized * num;
          Vector3 from = centralHit + vector3 + centralNormal;
          this.raycastSpotsTest.Add(from);
          this.raycastResult = HelperFunctions.LineCheck(from, from - centralNormal * 2f, HelperFunctions.LayerType.TerrainMap);
          if ((Object) this.raycastResult.collider != (Object) null)
            vector3List.Add(this.raycastResult.normal);
        }
      }
      Vector3 vector3_1 = centralNormal;
      foreach (Vector3 vector3_2 in vector3List)
        vector3_1 += vector3_2;
      this.averageNormal = vector3_1.normalized;
      if ((double) Vector3.Angle(this.averageNormal, Vector3.up) < (double) this.snapToVerticalAngle)
        this.averageNormal = Vector3.up;
    }
  }
}
