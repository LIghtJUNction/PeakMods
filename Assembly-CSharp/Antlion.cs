// Decompiled with JetBrains decompiler
// Type: Antlion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Antlion : MonoBehaviour
{
  private GameObject head;
  private Material slideMat;
  private GameObject slideObj;
  private CollisionModifier collisionModifier;
  private ClimbModifierSurface climbModifierSurface;
  private float str;
  private float activeFor;
  private Animator anim;
  private PhotonView view;
  private bool bitLocalPlayer;
  public float escapeRadiusForAchievement;
  private Character closestTarget;
  private float attackCounter;
  private bool attacking;
  public GameObject luggage;
  private bool firstActivation;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    this.anim = this.GetComponentInChildren<Animator>(true);
    this.collisionModifier = this.GetComponentInChildren<CollisionModifier>();
    this.climbModifierSurface = this.GetComponentInChildren<ClimbModifierSurface>();
    this.climbModifierSurface.alwaysClimbableRange = 16f;
    this.collisionModifier.standableRange = 16f;
    this.head = this.transform.Find("Head").gameObject;
    this.slideObj = this.transform.Find("Hill/Slide").gameObject;
    this.slideMat = this.slideObj.GetComponent<MeshRenderer>().material;
    this.slideMat.SetFloat("_Str", this.str);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.escapeRadiusForAchievement);
  }

  private void Update()
  {
    this.TestAchievement();
    if (!this.attacking)
      this.attackCounter += Time.deltaTime;
    this.GetClosestTarget();
    if ((Object) this.closestTarget != (Object) null)
    {
      this.DoActive();
      this.activeFor += Time.deltaTime;
    }
    else
    {
      this.DoInactive();
      this.activeFor = 0.0f;
    }
  }

  private void DoActive()
  {
    if ((double) this.str > 0.949999988079071)
    {
      this.collisionModifier.hasStandableRange = true;
      this.climbModifierSurface.hasAlwaysClimbableRange = true;
    }
    if ((double) Vector3.Distance(this.transform.position, this.closestTarget.Center) < 5.0 && (double) this.attackCounter > 0.15000000596046448 && this.view.IsMine)
    {
      this.attackCounter = 0.0f;
      this.view.RPC("RPCA_Attack", RpcTarget.All, (object) this.closestTarget.refs.view.ViewID);
    }
    this.ActiveVisuals();
  }

  [PunRPC]
  public void RPCA_Attack(int targetID)
  {
    this.Attack(PhotonView.Find(targetID).GetComponent<Character>());
  }

  private void Attack(Character target)
  {
    this.StartCoroutine(IAttack());

    IEnumerator IAttack()
    {
      this.attacking = true;
      this.anim.SetBool(nameof (Attack), true);
      yield return (object) new WaitForSeconds(0.12f);
      this.anim.SetBool(nameof (Attack), false);
      target.refs.movement.ApplyExtraDrag(0.0f, true);
      target.Fall(0.1f);
      target.AddForce(((target.Center - this.transform.position).normalized + Vector3.up) * 500f, 0.5f);
      target.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.2f, true);
      if (target.IsLocal)
        this.bitLocalPlayer = true;
      this.attacking = false;
    }
  }

  private void SinkLuggage() => this.luggage.transform.DOLocalMoveY(-2f, 1f);

  private void GetClosestTarget()
  {
    float num1 = 12f;
    if ((Object) this.closestTarget != (Object) null)
      num1 = 16f;
    float num2 = num1;
    Character character = (Character) null;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num3 = Vector3.Distance(this.transform.position, allCharacter.Center);
      if ((double) num3 < (double) num1)
        allCharacter.ClampSinceGrounded(1f + Mathf.Clamp01(this.activeFor / 3f));
      if ((double) num3 < (double) num2)
      {
        num2 = num3;
        character = allCharacter;
      }
    }
    if (!((Object) character != (Object) this.closestTarget) || !this.view.IsMine)
      return;
    this.view.RPC("RPCA_SetClosestTarget", RpcTarget.All, (object) ((Object) character != (Object) null ? character.refs.view.ViewID : -1));
  }

  [PunRPC]
  private void RPCA_SetClosestTarget(int targetID)
  {
    if (targetID == -1)
      this.closestTarget = (Character) null;
    else
      this.closestTarget = PhotonView.Find(targetID).GetComponent<Character>();
  }

  private void DoInactive()
  {
    this.collisionModifier.hasStandableRange = false;
    this.climbModifierSurface.hasAlwaysClimbableRange = false;
    this.InactiveVisuals();
  }

  private void ActiveVisuals()
  {
    if (!this.firstActivation)
    {
      this.firstActivation = true;
      this.SinkLuggage();
    }
    if (this.anim.gameObject.activeInHierarchy)
      this.anim.SetBool("Active", true);
    if (!this.slideObj.activeSelf)
    {
      this.slideObj.SetActive(true);
      this.head.SetActive(true);
    }
    if ((bool) (Object) this.closestTarget)
      this.head.transform.rotation = Quaternion.Lerp(this.head.transform.rotation, Quaternion.LookRotation(this.closestTarget.Center - this.head.transform.position), Time.deltaTime * 2f);
    this.str = Mathf.MoveTowards(this.str, 1f, Time.deltaTime);
    this.slideMat.SetFloat("_Str", this.str);
  }

  private void InactiveVisuals()
  {
    if (this.anim.gameObject.activeInHierarchy)
      this.anim.SetBool("Active", false);
    this.str = Mathf.MoveTowards(this.str, 0.0f, Time.deltaTime);
    if (!this.slideObj.activeSelf)
      return;
    if ((double) this.str < 0.0099999997764825821)
    {
      this.slideObj.SetActive(false);
      this.head.SetActive(false);
    }
    this.slideMat.SetFloat("_Str", this.str);
  }

  private void TestAchievement()
  {
    if (!(bool) (Object) Character.localCharacter || !this.bitLocalPlayer)
      return;
    if (Character.localCharacter.data.dead)
    {
      this.bitLocalPlayer = false;
    }
    else
    {
      if (!Character.localCharacter.data.fullyConscious || (double) Vector3.Distance(this.transform.position, Character.localCharacter.Center) <= (double) this.escapeRadiusForAchievement)
        return;
      Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MegaentomologyBadge);
      this.bitLocalPlayer = false;
    }
  }
}
