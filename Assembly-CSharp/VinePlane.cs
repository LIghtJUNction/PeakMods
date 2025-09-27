// Decompiled with JetBrains decompiler
// Type: VinePlane
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class VinePlane : MonoBehaviour
{
  public SkinnedMeshRenderer skinnedMeshRenderer;
  public SkinnedMeshRenderer skinnedMeshRendererLeaves;
  public MeshCollider meshCollider;
  private Mesh bakedMesh;
  public Transform bonesParent;
  public float raycastStartLength = 1f;
  public float raycastEndLength = 5f;
  public LayerMask mask;
  public float distanceToCorner = 5f;
  public Transform centerBone;
  public Material vineMatNormal;
  public Material vineMatPoison;
  public Material vineMatThorns;
  public Material editingMaterial;
  public VinePlane.VineType vineType;
  public float lift = 0.1f;
  public float planeLiftAmount = 0.5f;
  public float planeLiftPow = 5f;
  public bool liveEdit;
  public List<Vector3> defaultPositions = new List<Vector3>();
  public List<Quaternion> defaultRotations = new List<Quaternion>();

  private void Start() => this.UpdateCollider();

  private void OnValidate()
  {
    if (Application.isPlaying || !this.liveEdit)
      return;
    this.Blast();
  }

  public void Blast()
  {
    this.meshCollider.enabled = false;
    this.RestoreDefaults();
    for (int index = 0; index < this.bonesParent.childCount; ++index)
    {
      Transform child = this.bonesParent.GetChild(index);
      RaycastHit hitInfo;
      if (Physics.Linecast(child.transform.position + child.transform.up * this.raycastStartLength, child.transform.position - child.transform.up * (this.raycastStartLength + this.raycastEndLength), out hitInfo, this.mask.value, QueryTriggerInteraction.Ignore))
      {
        if (child.gameObject.activeSelf)
          child.transform.position = hitInfo.point + this.transform.up * this.lift * this.GetDistanceFromCorner(index);
        else
          child.transform.position = hitInfo.point;
        Plane plane = new Plane(this.transform.up, this.transform.position);
        if (child.gameObject.activeSelf)
        {
          float num = Mathf.Pow(Mathf.Clamp01(Mathf.Abs(plane.GetDistanceToPoint(child.transform.position) / this.raycastEndLength)), this.planeLiftPow);
          child.transform.position += this.transform.up * num * this.planeLiftAmount;
        }
      }
    }
    if (!this.liveEdit)
      this.Bake();
    else
      this.skinnedMeshRenderer.material = this.editingMaterial;
  }

  public void Bake()
  {
    this.meshCollider.enabled = true;
    this.liveEdit = false;
    this.UpdateCollider();
    if (this.vineType == VinePlane.VineType.Normal)
      this.skinnedMeshRenderer.material = this.vineMatNormal;
    else if (this.vineType == VinePlane.VineType.Thorns)
      this.skinnedMeshRenderer.material = this.vineMatThorns;
    else if (this.vineType == VinePlane.VineType.Poison)
      this.skinnedMeshRenderer.material = this.vineMatPoison;
    this.skinnedMeshRendererLeaves.material = this.skinnedMeshRenderer.material;
  }

  private float GetDistanceFromCorner(int index)
  {
    return Mathf.InverseLerp(this.distanceToCorner, 0.0f, Vector3.Distance(this.bonesParent.GetChild(index).position, this.centerBone.position));
  }

  private void RestoreDefaultsButton()
  {
    this.RestoreDefaults();
    this.Bake();
  }

  private void RestoreDefaults()
  {
    for (int index = 0; index < this.bonesParent.childCount; ++index)
    {
      this.bonesParent.GetChild(index).localPosition = this.defaultPositions[index];
      this.bonesParent.GetChild(index).localRotation = this.defaultRotations[index];
    }
    for (int index = 0; index < this.bonesParent.childCount; ++index)
    {
      if ((double) Mathf.Abs(this.bonesParent.GetChild(index).localPosition.y) > 3.9000000953674316)
        this.bonesParent.GetChild(index).gameObject.SetActive(false);
      else
        this.bonesParent.GetChild(index).gameObject.SetActive(true);
    }
  }

  private void SetDefaultsBECAREFUL()
  {
    this.defaultPositions.Clear();
    this.defaultRotations.Clear();
    for (int index = 0; index < this.bonesParent.childCount; ++index)
    {
      this.defaultPositions.Add(this.bonesParent.GetChild(index).localPosition);
      this.defaultRotations.Add(this.bonesParent.GetChild(index).localRotation);
    }
    this.Bake();
  }

  private void UpdateCollider()
  {
    this.skinnedMeshRenderer.ResetBounds();
    this.bakedMesh = new Mesh();
    this.skinnedMeshRenderer.BakeMesh(this.bakedMesh, true);
    this.meshCollider.sharedMesh = (Mesh) null;
    this.meshCollider.sharedMesh = this.bakedMesh;
  }

  private void OnDrawGizmos()
  {
    Plane plane = new Plane(this.transform.up, this.transform.position);
    for (int index = 0; index < this.bonesParent.childCount; ++index)
    {
      if (this.bonesParent.GetChild(index).gameObject.activeSelf)
      {
        double num = (double) Mathf.Abs(plane.GetDistanceToPoint(this.bonesParent.GetChild(index).transform.position));
        Gizmos.color = new Color((float) num, (float) num, (float) num);
        Gizmos.DrawSphere(this.bonesParent.GetChild(index).transform.position, 0.1f);
      }
    }
  }

  public enum VineType
  {
    Normal,
    Poison,
    Thorns,
  }
}
