// Decompiled with JetBrains decompiler
// Type: CharacterRagdoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#nullable disable
[DefaultExecutionOrder(-99)]
public class CharacterRagdoll : MonoBehaviour
{
  public float massMultiplier = 1f;
  public List<Bodypart> partList = new List<Bodypart>();
  public Dictionary<BodypartType, Bodypart> partDict = new Dictionary<BodypartType, Bodypart>();
  private List<Rigidbody> rigidbodies = new List<Rigidbody>();
  private Character character;
  public PhysicsMaterial slipperyMat;
  public PhysicsMaterial normalMat;
  internal List<Collider> colliderList = new List<Collider>();
  private PlayableGraph m_PlayableGraph;
  private bool firstFrame = true;
  private Quaternion rotationBefore;

  private void Awake()
  {
    this.character = this.GetComponentInParent<Character>();
    foreach (Bodypart componentsInChild in this.GetComponentsInChildren<Bodypart>())
      this.RegisterBodypart(componentsInChild);
  }

  private void SetPhysicsMats()
  {
    foreach (Bodypart part in this.partList)
      part.SetPhysicsMaterial(this.GetFrictionType(), this.slipperyMat, this.normalMat);
  }

  public void ToggleCollision(bool enableCollision)
  {
    for (int index = 0; index < this.colliderList.Count; ++index)
      this.colliderList[index].enabled = enableCollision;
  }

  public void ToggleKinematic(bool enableKinematic)
  {
    this.character.data.isKinecmatic = enableKinematic;
    for (int index = 0; index < this.rigidbodies.Count; ++index)
      this.rigidbodies[index].isKinematic = enableKinematic;
  }

  private Bodypart.FrictionType GetFrictionType()
  {
    return (double) this.character.data.currentRagdollControll < 0.89999997615814209 ? Bodypart.FrictionType.Grippy : Bodypart.FrictionType.Slippery;
  }

  private void Start()
  {
    this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
    if ((bool) (Object) this.character.refs.ikRigBuilder)
      this.character.refs.ikRigBuilder.Build(this.character.refs.animator.playableGraph);
    this.character.refs.animator.playableGraph.Evaluate(0.0f);
    PlayableGraph playableGraph = this.character.refs.animator.playableGraph;
    playableGraph.Stop();
    playableGraph = this.character.refs.animator.playableGraph;
    playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
  }

  private void OnDestroy()
  {
    if (!this.m_PlayableGraph.IsValid())
      return;
    this.m_PlayableGraph.Destroy();
  }

  private void RegisterBodypart(Bodypart bodypart)
  {
    this.partList.Add(bodypart);
    this.partDict.Add(bodypart.partType, bodypart);
    this.rigidbodies.Add(bodypart.Rig);
    bodypart.Rig.mass *= this.massMultiplier;
  }

  public void FixedUpdate()
  {
    this.SetPhysicsMats();
    if (this.firstFrame)
    {
      this.firstFrame = false;
    }
    else
    {
      if ((bool) (Object) this.character.data.currentItem)
        this.character.refs.animations.PrepIK();
      this.RotateCharacter();
      this.character.refs.ikRigBuilder.SyncLayers();
      this.character.refs.ikRigBuilder.Evaluate(Time.fixedDeltaTime);
      this.character.refs.animator.playableGraph.Evaluate(Time.fixedDeltaTime);
      this.character.refs.animations.ConfigureIK();
      for (int index = 0; index < this.partList.Count; ++index)
      {
        this.partList[index].SaveAnimationData();
        this.DrawLines(this.partList[index].jointParent, this.partList[index]);
      }
      this.SaveAdditionalTransformPositions();
      this.ResetRotation();
      for (int index = 0; index < this.partList.Count; ++index)
        this.partList[index].ResetTransform();
    }
  }

  public void SnapToAnimation()
  {
    for (int index = 0; index < this.character.refs.ragdoll.partList.Count; ++index)
      this.character.refs.ragdoll.partList[index].SnapToAnim();
  }

  private void DrawLines(Bodypart parent, Bodypart part)
  {
    if (!(bool) (Object) parent)
      return;
    Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), this.character.GetAnimationRelativePosition(parent.transform.position), Color.white);
    Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), part.Rig.position, Color.red);
  }

  private void RotateCharacter()
  {
    this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
    this.character.SetRotation();
  }

  private void ResetRotation()
  {
    this.character.refs.rigCreator.transform.rotation = this.rotationBefore;
  }

  private void SaveAdditionalTransformPositions()
  {
    Bodypart bodypart1 = this.character.GetBodypart(BodypartType.Head);
    this.character.data.targetHeadHeight = (bodypart1.transform.position - this.character.refs.rigCreator.transform.position).y;
    this.character.refs.animationHeadTransform.position = bodypart1.transform.position;
    this.character.refs.animationHeadTransform.rotation = bodypart1.transform.rotation;
    this.character.refs.animationLookTransform.position = bodypart1.transform.position;
    this.character.refs.animationLookTransform.rotation = Quaternion.Euler((float) (-(double) this.character.data.lookValues.y * 0.5), this.character.data.lookValues.x, 0.0f);
    Bodypart bodypart2 = this.character.GetBodypart(BodypartType.Hip);
    this.character.data.targetHipHeight = (bodypart2.transform.position - this.character.refs.rigCreator.transform.position).y;
    this.character.refs.animationHipTransform.position = bodypart2.transform.position;
    this.character.refs.animationHipTransform.rotation = bodypart2.transform.rotation;
    if (!(bool) (Object) this.character.data.currentItem)
      return;
    this.character.refs.animationItemTransform.position = this.character.refs.animationLookTransform.TransformPoint(this.character.data.currentItem.defaultPos);
    this.character.refs.animationItemTransform.rotation = Quaternion.LookRotation(this.character.data.lookDirection * this.character.data.currentItem.defaultForward.z + this.character.data.lookDirection_Right * this.character.data.currentItem.defaultForward.x + this.character.data.lookDirection_Up * this.character.data.currentItem.defaultForward.y);
  }

  public void HaltBodyVelocity()
  {
    foreach (Rigidbody rigidbody in this.rigidbodies)
    {
      rigidbody.linearVelocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;
    }
  }

  public void MoveAllRigsInDirection(Vector3 delta)
  {
    foreach (Rigidbody rigidbody in this.rigidbodies)
      rigidbody.MovePosition(rigidbody.position + delta);
  }

  internal void SetInterpolation(bool interpolateEnabled)
  {
    foreach (Bodypart part in this.partList)
      part.Rig.interpolation = interpolateEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
  }
}
