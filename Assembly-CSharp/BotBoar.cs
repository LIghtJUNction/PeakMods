// Decompiled with JetBrains decompiler
// Type: BotBoar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BotBoar : MonoBehaviour
{
  private Bot bot;
  private Rigidbody rig_g;
  private Character character;
  private Vector3 startPosition;
  public float timeSinceSawTarget;
  public Character potentialTarget;
  public float timeLookingAtTarget;
  public float timeSprinting;
  private bool flee;
  private float outOfSightTime;

  private void Awake()
  {
    this.bot = this.GetComponentInChildren<Bot>();
    this.character = this.GetComponent<Character>();
  }

  private void Start()
  {
  }

  public void ClearTarget() => this.bot.ClearTarget();

  private void Update()
  {
    this.bot.navigator.SetAgentVelocity(this.character.GetBodypart(BodypartType.Torso).Rig.linearVelocity);
    if ((double) this.bot.timeSprinting > 3.0)
      this.bot.IsSprinting = false;
    if (this.flee)
    {
      Debug.Log((object) "Fleeing");
      if (this.bot.TargetCharacter != null && (double) this.outOfSightTime < 4.0)
      {
        this.bot.FleeFromPoint(this.bot.TargetCharacter.Center);
        if (this.bot.CanSee(this.bot.TargetCharacter.Head, this.bot.Center, 20f, 360f))
        {
          Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.green);
          this.outOfSightTime = 0.0f;
        }
        else
        {
          Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.red);
          this.outOfSightTime += Time.deltaTime;
        }
      }
      else
      {
        this.flee = false;
        this.outOfSightTime = 0.0f;
        this.bot.ClearTarget();
        this.potentialTarget = (Character) null;
      }
    }
    else if ((bool) (Object) this.bot.TargetCharacter)
    {
      Debug.Log((object) "Chasing");
      Vector3? toTargetCharacter = this.bot.DistanceToTargetCharacter;
      Vector3 valueOrDefault;
      if (toTargetCharacter.HasValue)
      {
        valueOrDefault = toTargetCharacter.GetValueOrDefault();
        if ((double) valueOrDefault.magnitude > 4.0)
          goto label_14;
      }
      if ((double) this.bot.timeWithTarget > 15.0)
      {
        toTargetCharacter = this.bot.DistanceToTargetCharacter;
        if (toTargetCharacter.HasValue)
        {
          valueOrDefault = toTargetCharacter.GetValueOrDefault();
          if ((double) valueOrDefault.magnitude <= 2.0)
            goto label_15;
        }
        else
          goto label_15;
      }
      else
        goto label_15;
label_14:
      this.bot.ClearTarget();
label_15:
      this.bot.Chase();
      this.bot.CanSeeTarget();
      if ((double) this.bot.timeSinceSawTarget > 5.0)
        this.bot.ClearTarget();
      if (this.bot.IsSprinting)
        return;
      this.flee = true;
    }
    else
    {
      if (this.potentialTarget != null)
      {
        Debug.Log((object) "Looking at target");
        if (!this.bot.CanSee(this.bot.HeadPosition, this.potentialTarget.Center))
        {
          this.potentialTarget = (Character) null;
          this.timeLookingAtTarget = 0.0f;
          return;
        }
        this.bot.StandStill();
        this.bot.LookAtPoint(this.potentialTarget.Center);
        this.timeLookingAtTarget += Time.deltaTime;
        if ((double) this.timeLookingAtTarget > 4.0)
        {
          this.bot.TargetCharacter = this.potentialTarget;
          this.bot.IsSprinting = true;
          this.potentialTarget = (Character) null;
          this.timeLookingAtTarget = 0.0f;
        }
      }
      if (!((Object) this.potentialTarget == (Object) null))
        return;
      this.bot.Patrol();
      this.potentialTarget = this.bot.LookForPlayerHead(this.bot.HeadPosition, 20f)?.GetComponentInParent<Character>();
    }
  }
}
