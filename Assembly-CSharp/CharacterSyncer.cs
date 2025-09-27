// Decompiled with JetBrains decompiler
// Type: CharacterSyncer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

#nullable disable
[DefaultExecutionOrder(-100)]
public class CharacterSyncer : PhotonBinaryStreamSerializer<CharacterSyncData>
{
  private Character m_character;
  private Optionable<float3> lastPosition;
  private Optionable<float2> lastLook;
  private float sinceLastPackageUpdate;

  protected override void Awake()
  {
    base.Awake();
    this.m_character = this.GetComponent<Character>();
  }

  public override CharacterSyncData GetDataToWrite()
  {
    return new CharacterSyncData()
    {
      hipLocation = (float3) this.m_character.GetBodypart(BodypartType.Hip).Rig.position,
      lookValues = (float2) this.m_character.data.lookValues,
      movementInput = this.m_character.input.movementInput,
      sprintIsPressed = this.m_character.input.sprintIsPressed,
      sinceGrounded = this.m_character.data.sinceGrounded,
      ropePercent = this.m_character.data.ropePercent,
      ropeClimbing = this.m_character.data.isRopeClimbing,
      averageVelocity = (float3) this.GetAverageVelocity(),
      isClimbing = this.m_character.data.isClimbing,
      isGrounded = this.m_character.data.isGrounded,
      climbPos = (float3) this.m_character.data.climbPos,
      stammina = this.m_character.data.currentStamina,
      extraStammina = this.m_character.data.extraStamina,
      spectateZoom = this.m_character.data.spectateZoom,
      isChargingThrow = this.m_character.refs.items.isChargingThrow ? 1f : 0.0f
    };
  }

  public Vector3 GetAverageVelocity()
  {
    Vector3 zero = Vector3.zero;
    foreach (Bodypart part in this.m_character.refs.ragdoll.partList)
      zero += part.Rig.linearVelocity;
    return zero / (float) this.m_character.refs.ragdoll.partList.Count;
  }

  public override void OnDataReceived(CharacterSyncData data)
  {
    this.sinceLastPackageUpdate = 0.0f;
    base.OnDataReceived(data);
    this.lastPosition = Optionable<float3>.Some((float3) this.m_character.GetBodypart(BodypartType.Hip).Rig.position);
    this.lastLook = Optionable<float2>.Some((float2) this.m_character.data.lookValues);
    Vector3 averageVelocity = this.GetAverageVelocity();
    Vector3 vector3 = (Vector3) data.averageVelocity - averageVelocity;
    foreach (Bodypart part in this.m_character.refs.ragdoll.partList)
    {
      if (!part.Rig.isKinematic)
        part.Rig.linearVelocity += vector3;
    }
    this.m_character.input.movementInput = data.movementInput;
    this.m_character.input.sprintIsPressed = data.sprintIsPressed;
    if ((double) Mathf.Abs(this.m_character.data.sinceGrounded - data.sinceGrounded) > 2.0)
      this.m_character.data.sinceGrounded = data.sinceGrounded;
    if (data.ropeClimbing)
      this.m_character.data.ropePercent = data.ropePercent;
    if (data.isClimbing)
      this.m_character.data.climbPos = (Vector3) data.climbPos;
    this.m_character.data.currentStamina = data.stammina;
    this.m_character.data.extraStamina = data.extraStammina;
    this.m_character.data.spectateZoom = data.spectateZoom;
    this.m_character.refs.items.isChargingThrow = (double) data.isChargingThrow > 0.5;
  }

  private void Update()
  {
    if (this.photonView.IsMine || this.RemoteValue.IsNone || this.lastLook.IsNone)
      return;
    double num = 1.0 / (double) PhotonNetwork.SerializationRate;
    this.sinceLastPackageUpdate += Time.deltaTime;
    this.m_character.data.lookValues = Vector2.Lerp((Vector2) this.lastLook.Value, (Vector2) this.RemoteValue.Value.lookValues, this.sinceLastPackageUpdate / (float) num);
  }

  private void FixedUpdate()
  {
    if (this.photonView.IsMine || this.RemoteValue.IsNone || this.lastPosition.IsNone || (bool) (Object) this.m_character.data.carrier || this.m_character.warping)
      return;
    this.InterpolateRigPositions();
  }

  private void InterpolateRigPositions()
  {
    Vector3 hipLocation = (Vector3) this.RemoteValue.Value.hipLocation;
    double num1 = 1.0 / (double) PhotonNetwork.SerializationRate;
    this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
    float t = this.sinceLastPackage / (float) num1;
    Vector3 vector3 = Vector3.Lerp((Vector3) this.lastPosition.Value, hipLocation, t);
    Vector3 position = this.m_character.GetBodypart(BodypartType.Hip).Rig.position;
    Vector3 delta = vector3 - position;
    if ((double) delta.magnitude > 10.0)
    {
      this.m_character.refs.ragdoll.MoveAllRigsInDirection(delta);
    }
    else
    {
      delta.y *= 0.0f;
      float f = vector3.y - position.y;
      float num2 = Mathf.InverseLerp(0.3f, 0.6f, Mathf.Abs(f)) * Mathf.Sign(f);
      this.m_character.refs.ragdoll.MoveAllRigsInDirection((delta + Vector3.up * num2) * 0.1f);
    }
  }
}
