// Decompiled with JetBrains decompiler
// Type: Mob
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

#nullable disable
public class Mob : MonoBehaviourPunCallbacks
{
  public float movementSpeed = 5f;
  public float turnRate = 300f;
  public float aggroDistance = 5f;
  public float closeToTargetFactor = 1f;
  public float overShootFactor = 1f;
  [SerializeField]
  public float distanceToTarget;
  public float closestDistance;
  private Vector3 targetPos;
  private float untilNextPatrolPos;
  private Vector3 patrolPos;
  public float attackStartDistance = 2.5f;
  public float attackDistance = 1.5f;
  public float attackCooldown = 2f;
  public float maxVelocityToStartFlipping = 5f;
  public float minTimeToStartFlipping = 1f;
  public float maxStandingAngle = 70f;
  private Vector3 fC;
  private Vector3 fV;
  private Vector3 uC;
  private Vector3 uV;
  public float spring = 35f;
  public float drag = 15f;
  public Transform mesh;
  public Animator anim;
  private Rigidbody rig;
  [SerializeField]
  private bool attacking;
  private float timeSinceSpawned;
  public Transform visuals;
  private float lastStartedFlipping;
  [SerializeField]
  private Mob.MobState _mobState;
  private static readonly int WALKSPEED = Animator.StringToHash("WalkSpeed");
  private Vector3 startPos;
  private Vector3 startUp;
  private float fallTick;
  [SerializeField]
  internal bool forceNoMovement;
  [SerializeField]
  private float currentAttackCooldown;
  [SerializeField]
  private float inRangeForAttackTime;
  [SerializeField]
  private Character _targetChar;
  private float startFlippingTick;
  private Vector3 normal;
  private bool hitGround = true;
  private float lastFell;
  private RaycastHit groundCheckHitWalking;
  private RaycastHit groundCheckHitDown;
  private Vector3 lastPos;

  public Character forcedCharacterTarget { get; private set; }

  public void SetForcedTarget(Character character) => this.forcedCharacterTarget = character;

  internal Mob.MobState mobState
  {
    get => this._mobState;
    set
    {
      if (this._mobState == value)
        return;
      this._mobState = value;
      if (value != Mob.MobState.Flipping)
        return;
      this.lastStartedFlipping = Time.time;
    }
  }

  private void Awake()
  {
    this.rig = this.GetComponent<Rigidbody>();
    this.timeSinceSpawned = Time.time;
  }

  private void Start()
  {
    this.fC = this.transform.forward;
    this.uC = this.transform.up;
    this.normal = this.transform.up;
    this.ResetPatrolCounter();
    this.GetNewPatrolPos();
    this.startPos = this.transform.position;
    this.lastPos = this.transform.position;
    this.startUp = this.transform.up;
  }

  private void GetNewPatrolPos()
  {
    Vector3 onUnitSphere = Random.onUnitSphere;
    Vector3 from = this.startPos + this.startUp * 1f;
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + onUnitSphere * 15f, HelperFunctions.LayerType.TerrainMap);
    if ((bool) (Object) raycastHit.transform)
      this.patrolPos = raycastHit.point;
    else
      this.untilNextPatrolPos = 0.0f;
  }

  private void ResetPatrolCounter() => this.untilNextPatrolPos = Random.Range(0.5f, 5f);

  private bool dead => this.mobState == Mob.MobState.Dead;

  private void Update()
  {
    this.anim.SetFloat(Mob.WALKSPEED, this.mobState == Mob.MobState.Walking ? 1f : 0.0f);
    if (this.dead)
    {
      this.HandleDeath();
    }
    else
    {
      if ((double) Time.time - (double) this.timeSinceSpawned < 0.20000000298023224)
        return;
      this.GetTargetPos();
      this.CalcVars();
      if (this.photonView.IsMine)
      {
        if (this.forceNoMovement)
        {
          this.Attacking();
          this.Targeting();
          return;
        }
        if (this.mobState == Mob.MobState.Walking)
        {
          this.Attacking();
          this.Targeting();
          this.Patrol();
        }
      }
      else if (this.forceNoMovement || this.mobState == Mob.MobState.Walking)
        this.Attacking();
      if (this.forceNoMovement)
        return;
      this.Jiggling();
    }
  }

  private void FixedUpdate()
  {
    if (this.mobState == Mob.MobState.Dead)
    {
      this.rig.constraints = RigidbodyConstraints.None;
    }
    else
    {
      if ((double) Time.time - (double) this.timeSinceSpawned < 0.20000000298023224)
        return;
      this.DoGroundRaycast();
      this.lastPos = this.transform.position;
      if (this.forceNoMovement)
        this.rig.constraints = RigidbodyConstraints.None;
      else if (this.mobState == Mob.MobState.RigidbodyControlled)
      {
        this.rig.constraints = RigidbodyConstraints.None;
        this.fallTick = 0.0f;
        this.TestStartFlippingMyself();
      }
      else
      {
        this.rig.angularVelocity = Vector3.zero;
        this.rig.linearVelocity = Vector3.zero;
        this.rig.constraints = RigidbodyConstraints.FreezeRotation;
        if (this.mobState == Mob.MobState.Walking)
        {
          this.Movement();
          this.GroundSnapping();
          this.SetRotationWalking();
          this.TestFalling();
        }
        if (this.mobState != Mob.MobState.Flipping)
          return;
        this.SetRotationFlipping();
        if (!this.hitGround || (double) Time.time - (double) this.lastStartedFlipping <= 1.0)
          return;
        this.mobState = Mob.MobState.Walking;
      }
    }
  }

  private void LateUpdate()
  {
    if ((double) Time.time - (double) this.timeSinceSpawned < 0.20000000298023224 || this.mobState != Mob.MobState.Walking)
      return;
    this.VisualSnapping();
  }

  private void HandleDeath() => this.anim.SetBool("Cooked", true);

  private void TestFalling()
  {
    if (!this.hitGround)
    {
      this.fallTick += Time.fixedDeltaTime;
      if ((double) this.fallTick <= 0.5)
        return;
      this.lastFell = Time.time;
      this.mobState = Mob.MobState.RigidbodyControlled;
    }
    else
      this.fallTick = 0.0f;
  }

  private bool flippingMyself => this.mobState == Mob.MobState.Flipping;

  private void Attacking()
  {
    if ((bool) (Object) this.targetChar)
    {
      if (!this.attacking)
      {
        if ((double) this.distanceToTarget < (double) this.attackStartDistance && (double) this.currentAttackCooldown > (double) this.attackCooldown && this.photonView.IsMine)
          this.photonView.RPC("RPC_StartAttack", RpcTarget.All);
        this.inRangeForAttackTime = 0.0f;
      }
      if (!this.attacking)
        return;
      if ((double) this.inRangeForAttackTime == 0.0)
        this.anim.SetTrigger("Attack");
      this.inRangeForAttackTime += Time.deltaTime;
      if ((double) this.inRangeForAttackTime <= 1.0)
        return;
      if ((double) this.distanceToTarget < (double) this.attackDistance)
        this.InflictAttack(this.targetChar);
      this.currentAttackCooldown = 0.0f;
      this.attacking = false;
    }
    else
      this.inRangeForAttackTime = 0.0f;
  }

  [PunRPC]
  protected void RPC_StartAttack() => this.attacking = true;

  private Character targetChar
  {
    get => this._targetChar;
    set
    {
      if (!((Object) value != (Object) this._targetChar))
        return;
      this._targetChar = value;
      if (this.photonView.IsMine)
        this.photonView.RPC("RPC_SyncTargetCharacter", RpcTarget.Others, (object) ((Object) value == (Object) null ? -1 : value.photonView.ViewID));
      this.GetTargetPos();
    }
  }

  [PunRPC]
  protected void RPC_SyncTargetCharacter(int viewID)
  {
    if (viewID == -1)
    {
      this.targetChar = (Character) null;
    }
    else
    {
      PhotonView photonView = PhotonView.Find(viewID);
      Character component;
      if (!(bool) (Object) photonView || !photonView.TryGetComponent<Character>(out component))
        return;
      this.targetChar = component;
    }
  }

  protected virtual void InflictAttack(Character character)
  {
  }

  private void Targeting()
  {
    if (this.attacking && !((Object) this.targetChar == (Object) null))
      return;
    this.targetChar = (Character) null;
    float num1 = this.aggroDistance;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (allCharacter.data.fullyConscious)
      {
        float num2 = Vector3.Distance(this.transform.position, allCharacter.Center);
        if ((double) num2 < (double) num1 && (!((Object) this.forcedCharacterTarget != (Object) null) || !((Object) allCharacter != (Object) this.forcedCharacterTarget)) && !allCharacter.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.PoisonOverTime, out Affliction _) && !(bool) (Object) HelperFunctions.LineCheck(this.Center(), allCharacter.Center, HelperFunctions.LayerType.TerrainMap).transform)
        {
          num1 = num2;
          this.targetChar = allCharacter;
        }
      }
    }
  }

  private void Jiggling()
  {
    this.fV = FRILerp.Lerp(this.fV, (this.transform.forward - this.fC) * this.spring, this.drag);
    this.uV = FRILerp.Lerp(this.uV, (this.transform.up - this.uC) * this.spring, this.drag);
    this.fC += this.fV * Time.deltaTime;
    this.uC += this.uV * Time.deltaTime;
    this.mesh.rotation = Quaternion.LookRotation(this.fC, this.uC);
  }

  private void Patrol()
  {
    this.untilNextPatrolPos -= Time.deltaTime * Mathf.Lerp(30f, 1f, this.overShootFactor);
    if ((double) this.untilNextPatrolPos > 0.0)
      return;
    this.ResetPatrolCounter();
    this.GetNewPatrolPos();
  }

  private void CalcVars()
  {
    this.currentAttackCooldown += Time.deltaTime;
    this.distanceToTarget = Vector3.Distance(this.transform.position, this.targetPos);
    if ((double) this.distanceToTarget < (double) this.closestDistance)
      this.closestDistance = this.distanceToTarget;
    this.overShootFactor = Mathf.InverseLerp(0.02f, 0.0f, Mathf.Clamp01(this.distanceToTarget - this.closestDistance));
    this.closeToTargetFactor = Mathf.InverseLerp(1.2f, 1.5f, this.distanceToTarget);
  }

  private void SetRotationWalking()
  {
    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(this.targetPos - this.transform.position, this.normal).normalized, this.normal), this.turnRate * Time.deltaTime * this.overShootFactor);
  }

  private void SetRotationFlipping()
  {
    Vector3 b = Vector3.up;
    if ((bool) (Object) this.groundCheckHitDown.collider)
      b = this.groundCheckHitDown.normal;
    this.transform.up = Vector3.Slerp(this.transform.up, b, 8f * Time.fixedDeltaTime);
  }

  private void TestStartFlippingMyself()
  {
    if ((double) Time.time - (double) this.lastFell > 0.5 && (double) this.rig.linearVelocity.magnitude < (double) this.maxVelocityToStartFlipping)
    {
      this.startFlippingTick += Time.fixedDeltaTime;
      if ((double) this.startFlippingTick <= (double) this.minTimeToStartFlipping)
        return;
      this.mobState = Mob.MobState.Flipping;
    }
    else
      this.startFlippingTick = 0.0f;
  }

  private Vector3 GetTargetPos()
  {
    Vector3 v2 = this.patrolPos;
    if ((bool) (Object) this.targetChar)
      v2 = this.targetChar.Center;
    if (!this.targetPos.Same(v2, 0.1f))
    {
      this.targetPos = v2;
      this.closestDistance = float.PositiveInfinity;
    }
    return this.targetPos;
  }

  private bool DoGroundRaycast()
  {
    this.hitGround = false;
    this.groundCheckHitWalking = new RaycastHit();
    this.groundCheckHitDown = new RaycastHit();
    this.groundCheckHitWalking = HelperFunctions.LineCheck(this.lastPos, this.transform.position, HelperFunctions.LayerType.TerrainMap);
    this.groundCheckHitDown = HelperFunctions.LineCheck(this.transform.position, this.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap);
    if (!(bool) (Object) this.groundCheckHitWalking.transform)
      this.groundCheckHitWalking = HelperFunctions.LineCheck(this.Center(), this.Under(), HelperFunctions.LayerType.TerrainMap);
    if ((bool) (Object) this.groundCheckHitWalking.transform)
    {
      Vector3 normal = this.groundCheckHitWalking.normal;
      if ((double) Vector3.Angle(normal, Vector3.up) < (double) this.maxStandingAngle)
      {
        this.normal = normal;
        this.hitGround = true;
        return true;
      }
    }
    return false;
  }

  private void GroundSnapping()
  {
    if (!(bool) (Object) this.groundCheckHitWalking.transform)
      return;
    this.rig.MovePosition(this.groundCheckHitWalking.point);
  }

  private void VisualSnapping()
  {
    if ((bool) (Object) this.groundCheckHitWalking.transform)
      this.visuals.transform.position = this.groundCheckHitWalking.point;
    else
      this.visuals.transform.localPosition = Vector3.zero;
  }

  private Vector3 Under() => this.transform.position - this.normal * 1f;

  private Vector3 Center() => this.transform.position + this.normal * 0.2f;

  private void Movement()
  {
    this.transform.position += this.transform.forward * this.movementSpeed * Time.deltaTime * this.overShootFactor * this.closeToTargetFactor;
  }

  internal enum MobState
  {
    RigidbodyControlled,
    Walking,
    Flipping,
    Dead,
  }
}
