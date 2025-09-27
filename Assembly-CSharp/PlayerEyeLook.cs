// Decompiled with JetBrains decompiler
// Type: PlayerEyeLook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class PlayerEyeLook : MonoBehaviour
{
  public bool lookingAtCharacter;
  private List<Character> characters = new List<Character>();
  public float distance;
  public float lookRange;
  public float listenRange;
  private Character lastCharacter;
  public float lookSmoothing;
  public Character character;
  public Renderer[] eyeRenderers;
  private Vector3 lookDir;
  public float lookAngleMax;
  private Vector3 lookDelta;
  private float RightDelta;
  private float UpDelta;
  public float lookAngle;
  public float xLookThreshold;
  [FormerlySerializedAs("leftRightMax")]
  public float XMax;
  [FormerlySerializedAs("upDownMax")]
  public float YMax;
  private Character localCharacter;
  private Vector2 eyePos = Vector2.zero;
  private Vector2 eyeTarget = Vector2.zero;
  private Vector3 lastViewDir;

  private void Start() => this.localCharacter = this.GetComponent<Character>();

  private void Update()
  {
    this.characters = Character.AllCharacters;
    this.distance = float.PositiveInfinity;
    for (int index = 0; index < this.characters.Count; ++index)
    {
      float num = Vector3.Distance(this.characters[index].Center, this.localCharacter.Center);
      if ((double) num < (double) this.distance && (Object) this.characters[index] != (Object) this.localCharacter)
      {
        this.distance = num;
        this.character = this.characters[index];
      }
      AnimatedMouth component = this.characters[index].GetComponent<AnimatedMouth>();
      if ((double) num < (double) this.listenRange && component.isSpeaking && (Object) this.characters[index] != (Object) this.localCharacter)
      {
        this.distance = num;
        this.character = this.characters[index];
      }
    }
    if ((Object) this.character != (Object) null)
    {
      this.lookDir = (this.character.Head - this.localCharacter.Head).normalized;
      this.lookDelta = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward - this.lookDir;
      this.transform.InverseTransformDirection(this.lookDelta);
      this.UpDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, this.lookDelta);
      this.RightDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, this.lookDelta);
      this.lookAngle = Vector3.Angle(this.localCharacter.data.lookDirection, this.lookDir);
    }
    if ((Object) this.character != (Object) null && (double) this.distance < (double) this.lookRange && (double) this.lookAngle < (double) this.lookAngleMax)
    {
      this.eyeTarget = new Vector2(this.RightDelta * -this.XMax, this.UpDelta * this.YMax);
      this.lookingAtCharacter = true;
    }
    else
    {
      this.lookingAtCharacter = false;
      Vector3 rhs = this.localCharacter.data.lookDirection - this.localCharacter.GetBodypart(BodypartType.Hip).transform.forward with
      {
        y = 0.0f
      };
      this.eyeTarget = new Vector2(Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, rhs) * this.XMax, Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, rhs) * -this.YMax);
    }
    float num1 = 1f;
    if ((Object) this.character != (Object) this.lastCharacter)
      num1 = 0.3f;
    this.eyePos = Vector2.Lerp(this.eyePos, this.eyeTarget, Time.deltaTime * this.lookSmoothing * num1);
    for (int index = 0; index < this.eyeRenderers.Length; ++index)
      this.eyeRenderers[index].material.SetVector("_EyePosition", (Vector4) this.eyePos);
    if ((double) Vector3.Distance(this.lastViewDir, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward) <= (double) this.xLookThreshold)
      return;
    this.lastViewDir = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
  }

  private void OnDrawGizmosSelected()
  {
    if ((Object) this.localCharacter == (Object) null)
      return;
    if (this.lookingAtCharacter)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawRay(this.localCharacter.Head, this.lookDir * this.lookRange);
    }
    else
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawRay(this.localCharacter.Head, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward with
      {
        y = 0.0f
      } * this.lookRange);
    }
    Gizmos.color = Color.red;
    Gizmos.DrawRay(this.localCharacter.Head, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward * this.lookRange);
  }
}
