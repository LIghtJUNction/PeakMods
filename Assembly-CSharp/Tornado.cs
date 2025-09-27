// Decompiled with JetBrains decompiler
// Type: Tornado
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Tornado : MonoBehaviour
{
  public Animator tornadoSFX;
  public float force;
  public float range = 25f;
  public float captureDistance = 10f;
  public AnimationCurve inStrC;
  public AnimationCurve upStrC;
  private PhotonView view;
  private float lifeTime;
  private bool dying;
  private Vector3 tornadoPos;
  public float tornadoLifetimeMin = 30f;
  public float tornadoLifetimeMax = 120f;
  private Transform targetParent;
  public Transform target;
  private float syncCounter;
  private float selectNewTargetInSeconds;
  private Vector3 vel;
  private List<Character> ignoredCharacters = new List<Character>();
  private List<Character> caughtCharacters = new List<Character>();
  private float counter;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    this.transform.localScale = Vector3.zero;
    this.lifeTime = Random.Range(this.tornadoLifetimeMin, this.tornadoLifetimeMax);
    this.tornadoPos = this.transform.position;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(this.transform.position, this.range);
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.captureDistance);
  }

  private void Update()
  {
    if (this.view.IsMine)
    {
      this.syncCounter += Time.deltaTime;
      if ((double) this.syncCounter > 0.5)
      {
        this.syncCounter = 0.0f;
        this.view.RPC("RPCA_SyncTornado", RpcTarget.All, (object) this.vel);
      }
    }
    if (!this.dying)
    {
      this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one, Time.deltaTime * 0.25f);
      this.lifeTime -= Time.deltaTime;
      if ((double) this.lifeTime < 0.0 && this.view.IsMine)
        this.view.RPC("RPCA_TornadoDie", RpcTarget.All);
    }
    else
    {
      this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, Vector3.zero, Time.deltaTime * 0.2f);
      this.tornadoSFX.SetBool("Die", true);
      if ((double) this.transform.localScale.x < 0.0099999997764825821 && this.view.IsMine)
        PhotonNetwork.Destroy(this.gameObject);
    }
    if (this.view.IsMine)
      this.TargetSelection();
    this.Movement();
  }

  [PunRPC]
  private void RPCA_SyncTornado(Vector3 syncVel) => this.vel = syncVel;

  [PunRPC]
  private void RPCA_TornadoDie() => this.dying = true;

  private void TargetSelection()
  {
    this.selectNewTargetInSeconds -= Time.deltaTime;
    if ((bool) (Object) this.target && (double) this.selectNewTargetInSeconds >= 0.0 && (!(bool) (Object) this.target || (double) HelperFunctions.FlatDistance(this.transform.position, this.target.position) >= 10.0))
      return;
    this.selectNewTargetInSeconds = Random.Range(5f, 30f);
    this.PickTarget();
  }

  private void PickTarget()
  {
    this.view.RPC("RPCA_SelectTargetPos", RpcTarget.All, (object) Random.Range(0, this.targetParent.childCount));
  }

  [PunRPC]
  public void RPCA_SelectTargetPos(int targetID)
  {
    this.target = this.targetParent.GetChild(targetID);
  }

  private void Movement()
  {
    if ((Object) this.target == (Object) null)
      return;
    this.vel = FRILerp.Lerp(this.vel, (this.target.position - this.tornadoPos).Flat().normalized * 15f, 0.15f);
    this.tornadoPos += this.vel * Time.deltaTime;
    RaycastHit groundPosRaycast = HelperFunctions.GetGroundPosRaycast(this.tornadoPos + Vector3.up * 200f, HelperFunctions.LayerType.Terrain);
    if ((bool) (Object) groundPosRaycast.transform && (double) Vector3.Distance(this.tornadoPos, groundPosRaycast.point) < 100.0)
      this.transform.position = groundPosRaycast.point;
    else
      this.transform.position = this.tornadoPos;
  }

  private void FixedUpdate()
  {
    if ((double) this.transform.localScale.x < 0.10000000149011612)
    {
      if (this.caughtCharacters.Count > 0)
        this.caughtCharacters.Clear();
      if (this.ignoredCharacters.Count <= 0)
        return;
      this.ignoredCharacters.Clear();
    }
    else
    {
      this.AttractCharacters();
      this.CapturedCharacter();
      this.Feedback();
    }
  }

  private void CapturedCharacter()
  {
    if (this.caughtCharacters.Count == 0)
      return;
    foreach (Character target in new List<Character>((IEnumerable<Character>) this.caughtCharacters))
    {
      if (!this.ignoredCharacters.Contains(target) && !((Object) target == (Object) null))
      {
        float num = 15f;
        Vector3 vector3_1 = (target.Center - this.transform.position).Flat();
        Vector3 rhs = vector3_1.normalized * num;
        Vector3 vector3_2 = this.transform.position + rhs;
        float y = HelperFunctions.GetGroundPos(vector3_2, HelperFunctions.LayerType.Terrain).y;
        if ((double) y > (double) vector3_2.y)
          vector3_2.y = y;
        Vector3 vector3_3 = (vector3_2 - target.Center).Flat();
        vector3_1 = Vector3.Cross(Vector3.up, rhs);
        Vector3 normalized = vector3_1.normalized;
        target.AddForce(normalized * this.force);
        target.AddForce(vector3_3 * this.force * 0.2f);
        target.AddForce(Vector3.up * (19f + Mathf.Abs(this.Height(target) * 1f)));
        target.ClampSinceGrounded(0.5f);
        if (target.IsLocal)
        {
          target.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 200f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 200f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Torso).AddTorque(rhs.normalized * 100f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Hip).AddTorque(rhs.normalized * 100f, ForceMode.Acceleration);
        }
        else
        {
          target.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 500f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 500f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Torso).AddTorque(rhs.normalized * 500f, ForceMode.Acceleration);
          target.GetBodypartRig(BodypartType.Hip).AddTorque(rhs.normalized * 500f, ForceMode.Acceleration);
        }
        target.refs.movement.ApplyExtraDrag(0.95f, true);
        target.RPCA_Fall(0.5f);
        if (target.IsLocal && this.LetTargetGo(target, vector3_2))
          this.view.RPC("RPCA_ThrowPlayer", RpcTarget.All, (object) target.refs.view.ViewID);
      }
    }
  }

  private bool LetTargetGo(Character target, Vector3 orbitSpot)
  {
    return (double) this.Height(target) > 50.0 && (double) target.data.avarageVelocity.x > 0.0 != (double) target.Center.x > 0.0 || (double) Vector3.Distance(orbitSpot, target.Center) > 30.0 || target.IsStuck();
  }

  private float Height(Character target) => target.Center.y - this.transform.position.y;

  [PunRPC]
  private void RPCA_ThrowPlayer(int targetView)
  {
    Character target = PhotonView.Find(targetView).GetComponent<Character>();
    if (this.caughtCharacters.Contains(target))
      this.caughtCharacters.Remove(target);
    this.StartCoroutine(IIgnoreChar());

    IEnumerator IIgnoreChar()
    {
      this.ignoredCharacters.Add(target);
      float c = 0.0f;
      while ((double) c < 3.0)
      {
        target.ClampSinceGrounded(2f);
        c += Time.fixedDeltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
      yield return (object) new WaitForSeconds(2f);
      if ((bool) (Object) target && this.ignoredCharacters.Contains(target))
        this.ignoredCharacters.Remove(target);
    }
  }

  private void Feedback()
  {
    this.counter += Time.deltaTime;
    if ((double) this.counter <= 0.20000000298023224)
      return;
    GamefeelHandler.instance.AddPerlinShakeProximity(this.transform.position, 3f, 0.4f, maxProximity: this.range * 1f);
    this.counter = 0.0f;
  }

  private void AttractCharacters()
  {
    float num1 = this.transform.localScale.x * this.range;
    float num2 = this.transform.localScale.x * this.captureDistance;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!this.ignoredCharacters.Contains(allCharacter) && !this.caughtCharacters.Contains(allCharacter))
      {
        float num3 = HelperFunctions.FlatDistance(this.transform.position, allCharacter.Center);
        if ((double) num3 <= (double) num1 && (double) this.Height(allCharacter) >= -10.0 && (double) this.Height(allCharacter) <= 50.0)
        {
          float time = Mathf.Clamp01((float) (1.0 - (double) num3 / (double) num1));
          float num4 = this.inStrC.Evaluate(time);
          float num5 = this.upStrC.Evaluate(time);
          Vector3 normalized = (this.transform.position - allCharacter.Center).Flat().normalized;
          float num6 = 1f;
          if (allCharacter.data.isCrouching)
            num6 = 0.25f;
          allCharacter.AddForce(normalized * this.force * num4 * 1.2f * num6 + Vector3.up * this.force * num5 * num6, 0.8f);
          if ((double) num3 < (double) num2 && allCharacter.IsLocal && !allCharacter.IsStuck())
            this.view.RPC("RPCA_CaptureCharacter", RpcTarget.All, (object) allCharacter.refs.view.ViewID);
        }
      }
    }
  }

  [PunRPC]
  private void RPCA_CaptureCharacter(int targetViewID)
  {
    this.caughtCharacters.Add(PhotonView.Find(targetViewID).GetComponent<Character>());
  }

  [PunRPC]
  internal void RPCA_InitTornado(int targetViewID)
  {
    this.view = this.GetComponent<PhotonView>();
    this.targetParent = PhotonView.Find(targetViewID).transform.Find("TornadoPoints");
  }
}
