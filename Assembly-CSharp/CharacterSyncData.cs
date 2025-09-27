// Decompiled with JetBrains decompiler
// Type: CharacterSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
public struct CharacterSyncData : IBinarySerializable
{
  public float3 hipLocation;
  public float2 lookValues;
  public Vector2 movementInput;
  public bool sprintIsPressed;
  public float sinceGrounded;
  public bool ropeClimbing;
  public float ropePercent;
  public float3 averageVelocity;
  public bool isClimbing;
  public bool isGrounded;
  public float3 climbPos;
  public float stammina;
  public float extraStammina;
  public float spectateZoom;
  public float isChargingThrow;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat3(this.hipLocation);
    serializer.WriteHalf2(new half2((half) this.lookValues.x, (half) this.lookValues.y));
    CharacterSyncData.Flags flags = CharacterSyncData.Flags.NONE;
    if (this.sprintIsPressed)
      flags |= CharacterSyncData.Flags.SPRINT;
    if ((double) this.movementInput.x > 0.0099999997764825821)
      flags |= CharacterSyncData.Flags.WALK_RIGHT;
    if ((double) this.movementInput.x < -0.0099999997764825821)
      flags |= CharacterSyncData.Flags.WALK_LEFT;
    if ((double) this.movementInput.y > 0.0099999997764825821)
      flags |= CharacterSyncData.Flags.WALK_FORWARD;
    if ((double) this.movementInput.y < -0.0099999997764825821)
      flags |= CharacterSyncData.Flags.WALK_BACKWARD;
    if (this.ropeClimbing)
      flags |= CharacterSyncData.Flags.ROPE_CLIMBING;
    if (this.isClimbing)
      flags |= CharacterSyncData.Flags.CLIMBING;
    if (this.isGrounded)
      flags |= CharacterSyncData.Flags.IS_GROUNDED;
    serializer.WriteByte((byte) flags);
    serializer.WriteHalf((half) this.sinceGrounded);
    if (this.ropeClimbing)
      serializer.WriteHalf((half) this.ropePercent);
    serializer.WriteHalf3((half3) this.averageVelocity);
    if (this.isClimbing)
      serializer.WriteHalf3((half3) this.climbPos);
    serializer.WriteHalf((half) this.stammina);
    serializer.WriteHalf((half) this.extraStammina);
    serializer.WriteHalf((half) this.spectateZoom);
    serializer.WriteHalf((half) this.isChargingThrow);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.hipLocation = deserializer.ReadFloat3();
    this.lookValues = (float2) new Vector2((float) deserializer.ReadHalf(), (float) deserializer.ReadHalf());
    CharacterSyncData.Flags lhs = (CharacterSyncData.Flags) deserializer.ReadByte();
    Vector2 zero = Vector2.zero;
    this.sprintIsPressed = lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.SPRINT);
    if (lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.WALK_RIGHT))
      ++zero.x;
    if (lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.WALK_LEFT))
      --zero.x;
    if (lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.WALK_FORWARD))
      ++zero.y;
    if (lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.WALK_BACKWARD))
      --zero.y;
    this.movementInput = zero;
    this.sinceGrounded = (float) deserializer.ReadHalf();
    this.ropeClimbing = lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.ROPE_CLIMBING);
    if (this.ropeClimbing)
      this.ropePercent = (float) deserializer.ReadHalf();
    this.averageVelocity = (float3) deserializer.ReadHalf3();
    this.isClimbing = lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.CLIMBING);
    this.isGrounded = lhs.HasFlagUnsafe<CharacterSyncData.Flags>(CharacterSyncData.Flags.IS_GROUNDED);
    if (this.isClimbing)
      this.climbPos = (float3) deserializer.ReadHalf3();
    this.stammina = (float) deserializer.ReadHalf();
    this.extraStammina = (float) deserializer.ReadHalf();
    this.spectateZoom = (float) deserializer.ReadHalf();
    this.isChargingThrow = (float) deserializer.ReadHalf();
  }

  [System.Flags]
  public enum Flags : byte
  {
    NONE = 0,
    SPRINT = 1,
    ROPE_CLIMBING = 2,
    WALK_RIGHT = 4,
    WALK_LEFT = 8,
    WALK_FORWARD = 16, // 0x10
    WALK_BACKWARD = 32, // 0x20
    CLIMBING = 64, // 0x40
    IS_GROUNDED = 128, // 0x80
  }
}
