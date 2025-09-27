// Decompiled with JetBrains decompiler
// Type: Bot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class Bot : MonoBehaviour
{
  public Vector3 targetPos_Set;
  public Vector3 navigationDirection_read;
  public bool targetIsReachable;
  public float remainingNavDistance;
  public float timeSincePatrolEnded;
  public float timeWithTarget;
  public float timeWithoutTarget;
  public Navigator navigator;
  public Transform centerTransform;
  public float timeSinceSawTarget;
  public float timeSprinting;
  private bool isSprinting;
  private Vector3 lookDirection;
  private Vector2 movementInput;
  private Character targetCharacter;
  private NavMeshHit? fleePoint;
  private NavMeshHit? patrolHit = new NavMeshHit?(new NavMeshHit());

  public Vector3 LookDirection
  {
    get => this.lookDirection;
    set => this.lookDirection = value;
  }

  public Vector2 MovementInput
  {
    get => this.movementInput;
    set => this.movementInput = value;
  }

  public bool IsSprinting
  {
    get => this.isSprinting;
    set => this.isSprinting = value;
  }

  public Vector3 Center => this.centerTransform.position;

  [CanBeNull]
  public Character TargetCharacter
  {
    get => this.targetCharacter;
    set => this.targetCharacter = value;
  }

  public Vector3? DistanceToTargetCharacter
  {
    get
    {
      return !((UnityEngine.Object) this.TargetCharacter == (UnityEngine.Object) null) ? new Vector3?() : new Vector3?(this.TargetCharacter.Center - this.Center);
    }
  }

  public Vector3 HeadPosition => this.Center + Vector3.up;

  private void Awake() => this.navigator = this.GetComponentInChildren<Navigator>();

  private void Update()
  {
    this.timeSprinting = this.IsSprinting ? this.timeSprinting + Time.deltaTime : 0.0f;
    this.timeSincePatrolEnded += Time.deltaTime;
    if ((UnityEngine.Object) this.targetCharacter != (UnityEngine.Object) null)
    {
      this.timeWithTarget += Time.deltaTime;
      this.timeWithoutTarget = 0.0f;
    }
    else
    {
      this.timeWithoutTarget += 0.0f;
      this.timeWithTarget = 0.0f;
    }
    if ((double) this.timeSincePatrolEnded > 0.20000000298023224)
      this.patrolHit = new NavMeshHit?();
    Debug.DrawLine(this.Center, this.targetPos_Set, Color.cyan);
    Debug.DrawLine(this.Center, this.Center + this.navigationDirection_read, Color.blue);
    Debug.DrawLine(this.Center + Vector3.up, this.Center + Vector3.up + this.lookDirection, Color.yellow);
  }

  private void Start() => this.LookDirection = this.transform.forward;

  public void ClearTarget() => this.targetCharacter = (Character) null;

  public bool CanSee(Vector3 from, Vector3 to, float maxDistance = 70f, float maxAngle = 110f)
  {
    return (double) Vector3.Distance(from, to) <= (double) maxDistance && (double) Vector3.Angle(this.lookDirection, to - from) <= (double) maxAngle && !(bool) (UnityEngine.Object) HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap).transform;
  }

  public Rigidbody LookForPlayerHead(Vector3 searcherHeadPos, float maxRange = 70f, float maxAngle = 110f)
  {
    using (IEnumerator<Character> enumerator = ((IEnumerable<Character>) UnityEngine.Object.FindObjectsByType<Character>(FindObjectsSortMode.None)).Where<Character>((Func<Character, bool>) (character => !character.isBot)).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        Character current = enumerator.Current;
        if ((UnityEngine.Object) current == (UnityEngine.Object) null)
        {
          Debug.Log((object) "No player found");
          return (Rigidbody) null;
        }
        if ((double) Vector3.Distance(this.Center, current.TorsoPos()) > (double) maxRange || (double) Vector3.Angle(current.TorsoPos() - this.Center, this.lookDirection) > (double) maxAngle)
          return (Rigidbody) null;
        Bodypart bodypart = current.GetBodypart(BodypartType.Head);
        Debug.DrawLine(searcherHeadPos, bodypart.Rig.position, Color.red);
        if ((bool) (UnityEngine.Object) HelperFunctions.LineCheck(searcherHeadPos, bodypart.Rig.position, HelperFunctions.LayerType.TerrainMap).transform)
          return (Rigidbody) null;
        Debug.Log((object) "Found player head", (UnityEngine.Object) bodypart.Rig);
        return bodypart.Rig;
      }
    }
    return (Rigidbody) null;
  }

  public void Patrol()
  {
    this.timeSincePatrolEnded = 0.0f;
    NavMeshHit hit;
    if ((!this.patrolHit.HasValue || (double) this.remainingNavDistance < 1.0) && this.navigator.TryGetPointOnNavMeshCloseTo(PatrolBoss.me.GetPoint(), out hit))
    {
      this.patrolHit = new NavMeshHit?(hit);
      this.targetPos_Set = this.patrolHit.Value.position;
    }
    this.MoveForward();
    this.LookInDirection(this.navigationDirection_read);
  }

  public void RotateThenMove(Vector3 dir, float rotationSpeed = 3f)
  {
    if ((double) HelperFunctions.FlatAngle(dir, this.lookDirection) < 5.0)
      this.MoveForward();
    else
      this.StandStill();
    this.LookInDirection(dir, rotationSpeed);
  }

  public void StandStill() => this.MovementInput = new Vector2(0.0f, 0.0f);

  public void MoveForward() => this.MovementInput = new Vector2(0.0f, 1f);

  public void Chase()
  {
    if ((UnityEngine.Object) this.TargetCharacter == (UnityEngine.Object) null)
    {
      this.StandStill();
      Debug.Log((object) "No target character");
    }
    else
    {
      this.targetPos_Set = this.TargetCharacter.Center;
      this.MoveForward();
      this.LookInDirection(this.navigationDirection_read);
    }
  }

  public void LookAtPoint(Vector3 point, float rotationSpeed = 3f)
  {
    this.LookInDirection((point - this.Center).normalized, rotationSpeed);
  }

  public void LookInDirection(Vector3 direction, float rotationSpeed = 3f)
  {
    this.LookDirection = Vector3.RotateTowards(this.LookDirection, direction, Time.deltaTime * rotationSpeed, 0.0f);
  }

  public bool CanSeeTarget(float maxDistance = 20f, float maxAngle = 120f)
  {
    if (this.TargetCharacter != null && this.CanSee(this.HeadPosition, this.TargetCharacter.Center, maxDistance, maxAngle))
    {
      this.timeSinceSawTarget = 0.0f;
      return true;
    }
    this.timeSinceSawTarget += Time.deltaTime;
    return false;
  }

  public void FleeFromPoint(Vector3 point)
  {
    NavMeshHit hit;
    if ((!this.fleePoint.HasValue || (double) this.remainingNavDistance < 2.0) && this.navigator.TryGetPointOnNavMeshCloseTo(this.Center + (this.Center - point).normalized * 6f, out hit))
    {
      this.fleePoint = new NavMeshHit?(hit);
      this.targetPos_Set = this.fleePoint.Value.position;
    }
    this.MoveForward();
    this.LookInDirection(this.navigationDirection_read);
  }
}
