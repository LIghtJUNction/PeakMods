// Decompiled with JetBrains decompiler
// Type: Scoutmaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Scoutmaster : MonoBehaviour
{
  public float reachForce;
  private float targetForcedUntil;
  private Character _currentTarget;
  internal static List<Scoutmaster> AllScoutmasters = new List<Scoutmaster>();
  public Character discovered;
  private ScoutmasterAnimVars animVars;
  public float achievementDistance;
  private int STRENGTHID = Shader.PropertyToID("_Strength");
  private int GRAINMULTID = Shader.PropertyToID("_GrainMult");
  private Character character;
  private PhotonView view;
  public Material mat;
  private float sinceLookForTarget;
  private float distanceToTarget;
  private float sinceAnyoneCanSeeMe = 10f;
  private float achievementTestTick;
  private float attackHeightDelta = 100f;
  private float tpCounter;
  public float targetHasSeenMeCounter;
  private float sinceSeenTarget;
  private bool isThrowing;
  private float chillForSeconds;
  private float maxAggroHeight = 825f;

  private bool targetForced => (double) Time.time < (double) this.targetForcedUntil;

  public static bool GetPrimaryScoutmaster(out Scoutmaster scoutmaster)
  {
    if (Scoutmaster.AllScoutmasters.Count == 0)
    {
      scoutmaster = (Scoutmaster) null;
      return false;
    }
    scoutmaster = Scoutmaster.AllScoutmasters[0];
    return true;
  }

  public Character currentTarget
  {
    get => this._currentTarget;
    set
    {
      if (this.targetForced)
        return;
      this._currentTarget = value;
    }
  }

  private void OnEnable() => Scoutmaster.AllScoutmasters.Add(this);

  internal void SetCurrentTarget(Character setCurrentTarget, float forceForTime = 0.0f)
  {
    if (!((Object) setCurrentTarget != (Object) this.currentTarget))
      return;
    this.view.RPC("RPCA_SetCurrentTarget", RpcTarget.All, (object) ((Object) setCurrentTarget == (Object) null ? -1 : setCurrentTarget.photonView.ViewID), (object) forceForTime);
  }

  [PunRPC]
  private void RPCA_SetCurrentTarget(int targetViewID, float forceForTime)
  {
    this.currentTarget = targetViewID != -1 ? PhotonNetwork.GetPhotonView(targetViewID).GetComponent<Character>() : (Character) null;
    if ((double) forceForTime <= 0.0)
      return;
    this.targetForcedUntil = Time.time + forceForTime;
  }

  private void OnDestroy() => this.mat.SetFloat(this.STRENGTHID, 0.0f);

  private void OnDisable()
  {
    this.mat.SetFloat(this.STRENGTHID, 0.0f);
    Scoutmaster.AllScoutmasters.Remove(this);
  }

  private void Start()
  {
    this.animVars = this.GetComponentInChildren<ScoutmasterAnimVars>();
    this.character = this.GetComponent<Character>();
    this.view = this.GetComponent<PhotonView>();
    this.character.data.isScoutmaster = true;
    this.mat.SetFloat(this.STRENGTHID, 0.0f);
    this.mat.SetFloat(this.GRAINMULTID, GUIManager.instance.photosensitivity ? 0.0f : 1f);
  }

  private void CalcVars()
  {
    this.sinceLookForTarget += Time.deltaTime;
    bool flag1 = (bool) (Object) this.currentTarget && this.CanSeeTarget(this.currentTarget);
    if ((bool) (Object) this.currentTarget)
    {
      if (!flag1)
        this.sinceSeenTarget += Time.deltaTime;
      else
        this.sinceSeenTarget = 0.0f;
    }
    else
      this.sinceSeenTarget = 0.0f;
    if ((bool) (Object) this.currentTarget)
      this.distanceToTarget = Vector3.Distance(this.character.Center, this.currentTarget.Center);
    this.sinceAnyoneCanSeeMe += Time.deltaTime;
    if (this.AnyoneCanSeeMe())
      this.sinceAnyoneCanSeeMe = 0.0f;
    if ((bool) (Object) this.currentTarget)
    {
      bool flag2 = (double) Vector3.Distance(this.character.Center, this.currentTarget.Center) < 10.0;
      bool flag3 = (Object) HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Head, HelperFunctions.LayerType.TerrainMap).transform == (Object) null;
      if ((double) Vector3.Angle(this.currentTarget.data.lookDirection, this.character.Center - this.currentTarget.Head) > 70.0)
        flag3 = false;
      if (flag2 & flag3)
        this.targetHasSeenMeCounter += Time.deltaTime * 1f;
      else if (flag3)
        this.targetHasSeenMeCounter += Time.deltaTime * 0.3f;
      else if (flag2 & flag1)
        this.targetHasSeenMeCounter += Time.deltaTime * 0.15f;
      else
        this.targetHasSeenMeCounter = Mathf.MoveTowards(this.targetHasSeenMeCounter, 0.0f, Time.deltaTime * 0.1f);
    }
    else
      this.targetHasSeenMeCounter = 0.0f;
  }

  private bool CanSeeTarget(Character currentTarget)
  {
    return (Object) HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap).transform == (Object) null;
  }

  private void DoVisuals()
  {
    float b = 0.0f;
    if ((bool) (Object) this.currentTarget)
      this.currentTarget.data.myersDistance = Vector3.Distance(this.character.Center, this.currentTarget.Center);
    if ((bool) (Object) this.currentTarget && this.currentTarget.IsLocal)
    {
      b = Mathf.InverseLerp(50f, 5f, this.distanceToTarget);
      this.mat.SetFloat(this.GRAINMULTID, GUIManager.instance.photosensitivity ? 0.0f : 1f);
    }
    this.mat.SetFloat(this.STRENGTHID, Mathf.Lerp(this.mat.GetFloat(this.STRENGTHID), b, Time.deltaTime * 0.5f));
  }

  private void FixedUpdate()
  {
    if (!this.animVars.reaching || !((Object) this.character.data.grabbedPlayer == (Object) null) || !(bool) (Object) this.currentTarget)
      return;
    Rigidbody bodypartRig = this.character.GetBodypartRig(BodypartType.Hand_R);
    Vector3 normalized = (this.currentTarget.Center - bodypartRig.transform.position).normalized;
    bodypartRig.AddForce(normalized * this.reachForce, ForceMode.Acceleration);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.achievementDistance);
  }

  private void Update()
  {
    this.UpdateAchievement();
    this.DoVisuals();
    if (!this.view.IsMine)
      return;
    this.tpCounter += Time.deltaTime;
    this.ResetInput();
    this.CalcVars();
    if ((double) this.chillForSeconds > 0.0)
      this.chillForSeconds -= Time.deltaTime;
    else if ((Object) this.currentTarget == (Object) null)
    {
      this.EvasiveBehaviour();
      this.LookForTarget();
    }
    else
    {
      if ((double) this.distanceToTarget > 80.0)
        this.TeleportCloseToTarget();
      else
        this.Chase();
      this.VerifyTarget();
    }
  }

  private void UpdateAchievement()
  {
    this.achievementTestTick += Time.deltaTime;
    if ((double) this.achievementTestTick <= 1.0)
      return;
    this.achievementTestTick = 0.0f;
    this.TestAchievement();
  }

  private void TestAchievement()
  {
    if ((double) Vector3.Distance(this.character.Center, Character.localCharacter.Center) > (double) this.achievementDistance)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MentorshipBadge);
  }

  private void VerifyTarget()
  {
    if (this.ViableTargets() < 2)
    {
      this.SetCurrentTarget((Character) null);
    }
    else
    {
      Character closestOther = this.GetClosestOther(this.currentTarget);
      Character highestCharacter1 = this.GetHighestCharacter((Character) null);
      Character highestCharacter2 = this.GetHighestCharacter(highestCharacter1);
      if ((double) highestCharacter1.Center.y > (double) this.maxAggroHeight)
        this.SetCurrentTarget((Character) null);
      else if ((Object) this.currentTarget != (Object) highestCharacter1)
        this.SetCurrentTarget((Character) null);
      else if ((double) highestCharacter1.Center.y < (double) highestCharacter2.Center.y + (double) this.attackHeightDelta - 20.0)
      {
        this.SetCurrentTarget((Character) null);
      }
      else
      {
        if ((double) Vector3.Distance(closestOther.Center, this.currentTarget.Center) >= 15.0)
          return;
        this.SetCurrentTarget((Character) null);
      }
    }
  }

  private Character GetClosestOther(Character currentTarget)
  {
    List<Character> allCharacters = Character.AllCharacters;
    float num1 = float.MaxValue;
    Character closestOther = (Character) null;
    foreach (Character character in allCharacters)
    {
      if (!character.isBot && !((Object) character == (Object) currentTarget))
      {
        float num2 = Vector3.Distance(character.Center, currentTarget.Center);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          closestOther = character;
        }
      }
    }
    return closestOther;
  }

  private void EvasiveBehaviour()
  {
    if (!(bool) (Object) this.discovered)
      this.discovered = this.GetPlayerWhoSeesMe();
    if (!(bool) (Object) this.discovered)
      return;
    this.Flee();
    if ((double) this.sinceAnyoneCanSeeMe <= 0.5)
      return;
    this.TeleportFarAway();
  }

  public void TeleportFarAway()
  {
    if ((double) this.tpCounter < 5.0)
      return;
    this.tpCounter = 0.0f;
    this.view.RPC("WarpPlayerRPC", RpcTarget.All, (object) new Vector3(0.0f, 0.0f, 5000f), (object) false);
    this.view.RPC("StopClimbingRpc", RpcTarget.All, (object) 0.0f);
    this.discovered = (Character) null;
  }

  private Character GetPlayerWhoSeesMe()
  {
    Vector3 to = this.character.Center + Vector3.up * Random.Range(-0.5f, 0.5f);
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!allCharacter.isBot && (double) Vector3.Angle(to - allCharacter.Head, allCharacter.data.lookDirection) <= 80.0 && (Object) HelperFunctions.LineCheck(allCharacter.Head, to, HelperFunctions.LayerType.TerrainMap).transform == (Object) null)
        return allCharacter;
    }
    return (Character) null;
  }

  private void Flee()
  {
    Vector3 targetPos = this.character.Center + (this.character.Center - this.discovered.Center).normalized * 10f;
    if (this.character.data.isClimbing)
    {
      this.ClimbTowards(targetPos, 1f);
    }
    else
    {
      this.WalkTowards(targetPos, 1f);
      this.character.input.sprintIsPressed = true;
    }
  }

  private bool AnyoneCanSeeMe()
  {
    Vector3 pos1 = this.character.Head + Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
    Vector3 pos2 = this.character.HipPos() - Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
    return this.AnyoneCanSeePos(pos1) || this.AnyoneCanSeePos(pos2);
  }

  private bool AnyoneCanSeePos(Vector3 pos)
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!allCharacter.isBot && (double) Vector3.Angle(pos - allCharacter.Head, allCharacter.data.lookDirection) <= 80.0)
      {
        if ((Object) HelperFunctions.LineCheck(allCharacter.Head, pos, HelperFunctions.LayerType.TerrainMap).transform == (Object) null)
        {
          Debug.DrawLine(allCharacter.Head, pos, Color.blue);
          return true;
        }
        Debug.DrawLine(allCharacter.Head, pos, Color.red);
      }
    }
    return false;
  }

  private void TeleportCloseToTarget() => this.Teleport(this.currentTarget, 50f, 70f);

  private void Teleport(
    Character target,
    float minDistanceToTarget = 35f,
    float maxDistanceToTarget = 45f,
    float maxHeightDifference = 15f)
  {
    if ((double) this.tpCounter < 5.0)
      return;
    this.tpCounter = 0.0f;
    Debug.Log((object) "Trying to Teleport");
    if ((Object) target == (Object) null)
      target = this.GetHighestCharacter((Character) null);
    Vector3 center = this.character.Center;
    int num1 = 50;
    while (num1 > 0)
    {
      --num1;
      Vector3 onUnitSphere = Random.onUnitSphere;
      Vector3 from = target.Center + Vector3.up * 500f + onUnitSphere * 95f;
      Vector3 vector3 = Vector3.down;
      if (num1 < 25)
      {
        from = target.Center + Vector3.up;
        vector3 = Random.onUnitSphere;
      }
      RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + vector3 * 1000f, HelperFunctions.LayerType.TerrainMap);
      if ((bool) (Object) raycastHit.transform)
      {
        float num2 = Vector3.Distance(raycastHit.point, target.Center);
        float num3 = Mathf.Abs(raycastHit.point.y - target.Center.y);
        if ((double) num2 < (double) maxDistanceToTarget && (double) num3 < (double) maxHeightDifference && (double) num2 > (double) minDistanceToTarget && !this.AnyoneCanSeePos(raycastHit.point + Vector3.up))
        {
          Debug.Log((object) "Teleporting");
          this.view.RPC("WarpPlayerRPC", RpcTarget.All, (object) (raycastHit.point + Vector3.up), (object) false);
          this.view.RPC("StopClimbingRpc", RpcTarget.All, (object) 0.0f);
          this.discovered = (Character) null;
          break;
        }
      }
    }
  }

  private void Chase()
  {
    if ((double) this.sinceSeenTarget > 30.0 && !this.AnyoneCanSeeMe())
    {
      this.sinceSeenTarget = 0.0f;
      this.TeleportCloseToTarget();
      if ((double) Random.value >= 0.10000000149011612)
        return;
      this.currentTarget = (Character) null;
    }
    else if (this.character.data.isClimbing)
    {
      this.ClimbTowards(this.currentTarget.Head, 1f);
      if ((double) this.currentTarget.Center.y >= (double) this.character.Center.y || (bool) (Object) HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Center, HelperFunctions.LayerType.TerrainMap).transform)
        return;
      this.character.refs.climbing.StopClimbing();
    }
    else if ((bool) (Object) this.character.data.grabbedPlayer)
    {
      this.HoldPlayer();
    }
    else
    {
      this.LookAt(this.currentTarget.Head);
      float num = Vector3.Distance(this.character.Center, this.currentTarget.Center);
      if ((double) num > 5.0 || (double) this.targetHasSeenMeCounter > 1.0)
        this.WalkTowards(this.currentTarget.Head, 1f);
      if ((double) this.targetHasSeenMeCounter <= 1.0)
        return;
      this.character.input.sprintIsPressed = (double) num < 15.0;
      if ((double) Vector3.Distance(this.character.Center, this.currentTarget.Center) >= 3.0 || (double) this.character.data.sinceClimb <= 1.0 || !this.character.data.isGrounded)
        return;
      this.character.input.useSecondaryIsPressed = true;
    }
  }

  private void StandStill()
  {
  }

  private void ResetInput() => this.character.input.ResetInput();

  private void HoldPlayer()
  {
    this.currentTarget.data.sinceGrounded = 0.0f;
    this.character.input.useSecondaryIsPressed = true;
    Vector3 lookDirection = this.character.data.lookDirection with
    {
      y = 0.6f
    };
    lookDirection.Normalize();
    this.character.data.lookValues = (Vector2) HelperFunctions.DirectionToLook(lookDirection);
    if (this.isThrowing)
      return;
    this.view.RPC("RPCA_Throw", RpcTarget.All);
  }

  [PunRPC]
  public void RPCA_Throw() => this.StartCoroutine(this.IThrow());

  private IEnumerator IThrow()
  {
    this.isThrowing = true;
    if (this.view.IsMine)
      this.RotateToMostEvilThrowDirection();
    if (this.currentTarget.IsLocal)
      GamefeelHandler.instance.AddPerlinShake(15f, 0.5f);
    GamefeelHandler.instance.AddPerlinShake(3f, 3f);
    float c = 0.0f;
    while ((double) c < 3.2000000476837158)
    {
      this.currentTarget.data.lookValues = (Vector2) HelperFunctions.DirectionToLook(this.character.Head - this.currentTarget.Head);
      c += Time.deltaTime;
      yield return (object) null;
    }
    Vector3 vector3 = -this.character.data.lookDirection with
    {
      y = 0.0f
    };
    vector3.Normalize();
    vector3.y = 0.3f;
    this.character.refs.grabbing.Throw(vector3 * 1500f, 3f);
    this.currentTarget.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, true);
    this.isThrowing = false;
    this.chillForSeconds = 2f;
  }

  private void RotateToMostEvilThrowDirection()
  {
    Vector3[] circularDirections = HelperFunctions.GetCircularDirections(10);
    float num1 = 10f;
    float num2 = 1000f;
    Vector3 center = this.character.Center;
    Vector3 vector3_1 = this.character.data.lookDirection_Flat;
    float num3 = 0.0f;
    foreach (Vector3 vector3_2 in circularDirections)
    {
      Vector3 vector3_3 = center + vector3_2 * num1;
      if (!(bool) (Object) HelperFunctions.LineCheck(center, vector3_3, HelperFunctions.LayerType.TerrainMap).transform)
      {
        RaycastHit raycastHit = HelperFunctions.LineCheck(vector3_3, center + vector3_3 + Vector3.down * num2, HelperFunctions.LayerType.TerrainMap);
        if ((bool) (Object) raycastHit.transform && (double) raycastHit.distance > (double) num3)
        {
          vector3_1 = vector3_2;
          num3 = raycastHit.distance;
        }
      }
    }
    this.character.data.lookValues = (Vector2) HelperFunctions.DirectionToLook(-vector3_1);
  }

  private void ClimbTowards(Vector3 targetPos, float mult)
  {
    this.LookAt(targetPos);
    this.character.input.movementInput = new Vector2(Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f), mult);
    this.character.data.currentStamina = 1f;
  }

  private void WalkTowards(Vector3 targetPos, float mult)
  {
    this.LookAt(targetPos);
    float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
    if ((double) Vector3.Distance(this.character.Center, targetPos) < 5.0)
    {
      if ((double) num < 2.5)
        mult *= 0.0f;
      else if ((double) num < 1.5)
        mult *= -1f;
    }
    this.character.input.movementInput = new Vector2(0.0f, mult);
    this.character.refs.climbing.TryClimb();
    if (!((Object) HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap).transform == (Object) null))
      return;
    this.character.input.jumpWasPressed = true;
  }

  private void LookAt(Vector3 lookAtPos)
  {
    this.character.data.lookValues = (Vector2) HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
  }

  private int ViableTargets()
  {
    List<Character> allCharacters = Character.AllCharacters;
    int num = 0;
    foreach (Character character in allCharacters)
    {
      if (!character.isBot && !character.data.dead && !character.data.fullyPassedOut)
        ++num;
    }
    return num;
  }

  private void LookForTarget()
  {
    if (this.ViableTargets() < 2 || (double) this.sinceLookForTarget < 30.0)
      return;
    this.sinceLookForTarget = 0.0f;
    if ((double) Random.value > 0.10000000149011612)
      return;
    Character highestCharacter1 = this.GetHighestCharacter((Character) null);
    Character highestCharacter2 = this.GetHighestCharacter(highestCharacter1);
    if ((double) highestCharacter1.Center.y <= (double) highestCharacter2.Center.y + (double) this.attackHeightDelta || (double) highestCharacter1.Center.y >= (double) this.maxAggroHeight)
      return;
    this.SetCurrentTarget(highestCharacter1);
  }

  private Character GetHighestCharacter(Character ignoredCharacter)
  {
    List<Character> allCharacters = Character.AllCharacters;
    Character highestCharacter = (Character) null;
    foreach (Character character in allCharacters)
    {
      if (!character.isBot && !character.data.dead && !character.data.fullyPassedOut && !((Object) character == (Object) ignoredCharacter) && ((Object) highestCharacter == (Object) null || (double) character.Center.y > (double) highestCharacter.Center.y))
        highestCharacter = character;
    }
    return highestCharacter;
  }
}
