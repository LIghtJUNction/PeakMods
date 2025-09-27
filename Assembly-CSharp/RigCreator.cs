// Decompiled with JetBrains decompiler
// Type: RigCreator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RigCreator : MonoBehaviour
{
  [HideInInspector]
  public bool aboutToClear;
  public float springMultiplier = 1f;
  public List<RigPart> parts;

  public void StartClear() => this.aboutToClear = true;

  public void ClearNo() => this.ClearStates();

  public void ClearYes()
  {
    this.ClearStates();
    this.ClearDataAndRig();
  }

  public void AutoGenerate()
  {
    this.FindParts();
    this.GenerateData();
  }

  private void ClearStates() => this.aboutToClear = false;

  private void GenerateData()
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if (this.parts[index].justCreated)
        this.InitPart(this.parts[index]);
      else
        this.ApplyPartData(this.parts[index]);
      this.parts[index].justCreated = false;
    }
  }

  private void InitPart(RigPart part)
  {
    this.AutoGenerateCollidersForPart(part);
    this.AddRigidbodyToPart(part);
    this.AddJointToPart(part);
    this.AddBodyPartScript(part);
  }

  private void ApplyPartData(RigPart rigPart)
  {
    this.SyncCollidersFromData(rigPart);
    this.SyncRigidbodyFromData(rigPart);
    this.SyncJointFromData(rigPart);
    this.SyncBodypartScript(rigPart);
  }

  private void SyncBodypartScript(RigPart rigPart)
  {
    if ((bool) (Object) rigPart.transform.GetComponent<Bodypart>())
      return;
    this.AddBodyPartScript(rigPart);
  }

  private void SyncJointFromData(RigPart rigPart)
  {
    if (!((Object) rigPart.joint == (Object) null))
      return;
    this.AddJointToPart(rigPart);
  }

  private void SyncRigidbodyFromData(RigPart rigPart)
  {
    if (!((Object) rigPart.rig == (Object) null))
      return;
    this.AddRigidbodyToPart(rigPart);
  }

  private void AddRigidbodyToPart(RigPart rigPart)
  {
    Rigidbody rigidbody = rigPart.transform.gameObject.AddComponent<Rigidbody>();
    rigidbody.mass = rigPart.mass;
    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    RigCreatorRigidbody creatorRigidbody = rigPart.transform.gameObject.AddComponent<RigCreatorRigidbody>();
    creatorRigidbody.mass = rigPart.mass;
    rigPart.rig = rigidbody;
    rigPart.rigHandler = creatorRigidbody;
  }

  private void SyncCollidersFromData(RigPart rigPart)
  {
    for (int index = 0; index < rigPart.colliders.Count; ++index)
    {
      if ((Object) rigPart.colliders[index].colliderObject == (Object) null)
        rigPart.colliders[index] = this.CreateColliderObject(rigPart.colliders[index].colliderPosition, rigPart.colliders[index].colliderRotation, rigPart.colliders[index].colliderScale, rigPart.transform, rigPart.colliders[index].height, rigPart.colliders[index].radius, false);
    }
  }

  private RigCreatorColliderData CreateColliderObject(
    Vector3 position,
    Quaternion rotation,
    Vector3 scale,
    Transform parent,
    float height,
    float radius,
    bool isWorldSpace = true)
  {
    GameObject gameObject = new GameObject("RigCollider");
    if (isWorldSpace)
    {
      gameObject.transform.position = position;
      gameObject.transform.rotation = rotation;
    }
    gameObject.transform.SetParent(parent);
    if (!isWorldSpace)
    {
      gameObject.transform.localPosition = position;
      gameObject.transform.localRotation = rotation;
      gameObject.transform.localScale = scale;
    }
    CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
    capsuleCollider.direction = 2;
    capsuleCollider.radius = radius;
    capsuleCollider.height = height;
    return new RigCreatorColliderData()
    {
      colliderPosition = capsuleCollider.transform.position,
      colliderRotation = capsuleCollider.transform.rotation,
      colliderScale = capsuleCollider.transform.localScale,
      radius = capsuleCollider.radius,
      height = capsuleCollider.height,
      colliderObject = gameObject.AddComponent<RigCreatorCollider>()
    };
  }

  private void AddBodyPartScript(RigPart rigPart)
  {
    rigPart.transform.gameObject.AddComponent<Bodypart>().InitBodypart(rigPart.partType);
  }

  private void AddJointToPart(RigPart rigPart)
  {
    Rigidbody componentInParent = rigPart.transform.parent.GetComponentInParent<Rigidbody>();
    if (!(bool) (Object) componentInParent)
      return;
    ConfigurableJoint configurableJoint = this.SpawnJoint(rigPart.rig, componentInParent, rigPart.spring);
    rigPart.joint = configurableJoint;
    rigPart.jointHandler = rigPart.transform.gameObject.AddComponent<RigCreatorJoint>();
    rigPart.jointHandler.spring = rigPart.spring;
    rigPart.jointHandler.SetSpring(rigPart.spring);
  }

  internal ConfigurableJoint SpawnJoint(Rigidbody ownRig, Rigidbody otherRig, float spring)
  {
    ConfigurableJoint configurableJoint = ownRig.gameObject.AddComponent<ConfigurableJoint>();
    configurableJoint.lowAngularXLimit = configurableJoint.lowAngularXLimit with
    {
      limit = -177f
    };
    configurableJoint.highAngularXLimit = configurableJoint.highAngularXLimit with
    {
      limit = 177f
    };
    configurableJoint.angularYLimit = configurableJoint.angularYLimit with
    {
      limit = 177f
    };
    configurableJoint.angularZLimit = configurableJoint.angularZLimit with
    {
      limit = 177f
    };
    configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
    configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
    configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
    configurableJoint.xMotion = ConfigurableJointMotion.Locked;
    configurableJoint.yMotion = ConfigurableJointMotion.Locked;
    configurableJoint.zMotion = ConfigurableJointMotion.Locked;
    configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
    configurableJoint.connectedBody = otherRig;
    return configurableJoint;
  }

  private void AutoGenerateCollidersForPart(RigPart rigPart)
  {
    Transform transform = (Transform) null;
    float num1 = 0.0f;
    for (int index = rigPart.transform.childCount - 1; index >= 0; --index)
    {
      float num2 = Vector3.Distance(rigPart.transform.GetChild(index).position, rigPart.transform.position);
      if ((double) num2 > (double) num1)
      {
        num1 = num2;
        transform = rigPart.transform.GetChild(index);
      }
    }
    Vector3 position = Vector3.Lerp(rigPart.transform.position, transform.position, 0.5f);
    Quaternion rotation = Quaternion.LookRotation(transform.position - rigPart.transform.position);
    float height = Vector3.Distance(transform.position, rigPart.transform.position);
    rigPart.colliders.Add(this.CreateColliderObject(position, rotation, Vector3.one, rigPart.transform, height, 0.1f));
  }

  private void FindParts()
  {
    BodypartType bodypartType;
    for (int index = 0; index < 179; ++index)
    {
      if (this.Contains((BodypartType) index))
      {
        bodypartType = (BodypartType) index;
        Transform childRecursive = HelperFunctions.FindChildRecursive(bodypartType.ToString(), this.transform);
        if ((bool) (Object) childRecursive)
          this.GetPartFromPartType((BodypartType) index).transform = childRecursive;
      }
      else
      {
        bodypartType = (BodypartType) index;
        Transform childRecursive = HelperFunctions.FindChildRecursive(bodypartType.ToString(), this.transform);
        if ((bool) (Object) childRecursive)
          this.parts.Add(new RigPart()
          {
            transform = childRecursive,
            partType = (BodypartType) index,
            justCreated = true
          });
      }
    }
  }

  private RigPart GetPartFromPartType(BodypartType partType)
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if (this.parts[index].partType == partType)
        return this.parts[index];
    }
    return (RigPart) null;
  }

  private bool Contains(BodypartType targetType)
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if (this.parts[index].partType == targetType)
        return true;
    }
    return false;
  }

  private void ClearDataAndRig()
  {
    for (int index1 = this.parts.Count - 1; index1 >= 0; --index1)
    {
      for (int index2 = this.parts[index1].colliders.Count - 1; index2 >= 0; --index2)
        Object.DestroyImmediate((Object) this.parts[index1].colliders[index2].colliderObject.gameObject);
      this.parts[index1].colliders.Clear();
      Bodypart component = this.parts[index1].transform.GetComponent<Bodypart>();
      if ((bool) (Object) component)
        Object.DestroyImmediate((Object) component);
      Object.DestroyImmediate((Object) this.parts[index1].joint);
      if ((bool) (Object) this.parts[index1].jointHandler)
        Object.DestroyImmediate((Object) this.parts[index1].jointHandler);
      Object.DestroyImmediate((Object) this.parts[index1].rig);
      Object.DestroyImmediate((Object) this.parts[index1].rigHandler);
    }
    this.parts.Clear();
  }

  private RigPart GetPartFromJointObject(RigCreatorJoint jointObject)
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if ((Object) this.parts[index].jointHandler == (Object) jointObject)
        return this.parts[index];
    }
    return (RigPart) null;
  }

  private RigPart GetPartFromRigObject(RigCreatorRigidbody rigObject)
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if ((Object) this.parts[index].rigHandler == (Object) rigObject)
        return this.parts[index];
    }
    return (RigPart) null;
  }

  private RigPart GetPartFromColliderObject(RigCreatorCollider colliderObject)
  {
    for (int index1 = 0; index1 < this.parts.Count; ++index1)
    {
      for (int index2 = this.parts[index1].colliders.Count - 1; index2 >= 0; --index2)
      {
        if ((Object) this.parts[index1].colliders[index2].colliderObject == (Object) colliderObject)
          return this.parts[index1];
      }
    }
    return (RigPart) null;
  }

  private RigCreatorColliderData GetColliderDataFromColliderObject(RigCreatorCollider colliderObject)
  {
    for (int index1 = 0; index1 < this.parts.Count; ++index1)
    {
      for (int index2 = this.parts[index1].colliders.Count - 1; index2 >= 0; --index2)
      {
        if ((Object) this.parts[index1].colliders[index2].colliderObject == (Object) colliderObject)
          return this.parts[index1].colliders[index2];
      }
    }
    return (RigCreatorColliderData) null;
  }

  internal void RemoveCollider(RigCreatorCollider rigCreatorCollider)
  {
    RigCreatorColliderData fromColliderObject = this.GetColliderDataFromColliderObject(rigCreatorCollider);
    if (fromColliderObject == null)
      return;
    this.GetPartFromColliderObject(rigCreatorCollider)?.colliders.Remove(fromColliderObject);
  }

  internal void ColliderChanged(
    RigCreatorCollider rigCreatorCollider,
    Vector3 localPosition,
    Quaternion localRotation,
    Vector3 localScale,
    float height,
    float radius)
  {
    RigCreatorColliderData creatorColliderData = this.GetColliderDataFromColliderObject(rigCreatorCollider);
    if (creatorColliderData == null)
    {
      RigPart rigPart = this.GetPartFromColliderObject(rigCreatorCollider) ?? this.FindPartFromName(rigCreatorCollider.transform.parent.name);
      if (rigPart == null)
        return;
      creatorColliderData = new RigCreatorColliderData();
      creatorColliderData.colliderObject = rigCreatorCollider;
      rigPart.colliders.Add(creatorColliderData);
    }
    creatorColliderData.colliderPosition = localPosition;
    creatorColliderData.colliderRotation = localRotation;
    creatorColliderData.colliderScale = localScale;
    creatorColliderData.height = height;
    creatorColliderData.radius = radius;
  }

  internal void RigidbodyChanged(RigCreatorRigidbody rigObject, float mass)
  {
    this.GetPartFromRigObject(rigObject).mass = mass;
  }

  internal void JointChanged(RigCreatorJoint jointObject, float spring)
  {
    this.GetPartFromJointObject(jointObject).spring = spring;
  }

  private RigPart FindPartFromName(string targetName)
  {
    for (int index = 0; index < this.parts.Count; ++index)
    {
      if (this.parts[index].partType.ToString() == targetName)
        return this.parts[index];
    }
    return (RigPart) null;
  }
}
