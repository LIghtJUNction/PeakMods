// Decompiled with JetBrains decompiler
// Type: ArrowShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class ArrowShooter : MonoBehaviourPunCallbacks
{
  [FormerlySerializedAs("damage")]
  public int damagePips;
  public float force;
  public float range;
  public float castRadius;
  public float movementThreshold;
  public float movementCooldown;
  public Arrow arrowPrefab;
  public List<Arrow> arrows = new List<Arrow>();
  public int maxArrows = 100;
  private PhotonView view;
  public float reloadTime;
  private bool reloading;
  public Transform shooter;
  public Transform target;
  private Vector3 hitTarget;
  private Vector3 targetLastPosition;
  private float moveAcumulator;
  public Character targetCharacter;
  public ParticleSystem trailParticles;
  public ParticleSystem firedParticles;
  public ParticleSystem emptyParticles;
  public bool empty;
  private bool initialized;

  private void Awake() => this.view = this.GetComponent<PhotonView>();

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    if (!this.view.IsMine)
      return;
    this.view.RPC("WarningArrows_RPC", RpcTarget.AllBuffered, (object) UnityEngine.Random.Range(1, 5));
  }

  private void Start()
  {
  }

  private void Update()
  {
    if (this.empty || this.reloading)
      return;
    RaycastHit hitInfo;
    if (Physics.SphereCast(this.transform.position, this.castRadius, this.shooter.forward, out hitInfo, this.range))
    {
      bool flag = false;
      if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
      {
        this.targetCharacter = hitInfo.collider.gameObject.GetComponentInParent<Character>();
        this.target = hitInfo.collider.transform;
        this.hitTarget = hitInfo.point;
        flag = true;
      }
      if (!flag)
      {
        this.target = hitInfo.collider.transform;
        this.hitTarget = hitInfo.point;
      }
    }
    if ((Object) this.target != (Object) null)
    {
      this.moveAcumulator += Vector3.Distance(this.target.position, this.targetLastPosition);
      if ((double) this.moveAcumulator > 0.0)
        this.moveAcumulator -= this.movementCooldown * Time.deltaTime;
      this.targetLastPosition = this.target.position;
    }
    else
      this.moveAcumulator = 0.0f;
    if ((double) this.moveAcumulator <= (double) this.movementThreshold)
      return;
    this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered);
  }

  public void testFire()
  {
    if (!this.view.IsMine)
      return;
    this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered, (object) (this.transform.position + this.transform.forward));
  }

  [PunRPC]
  public void FireArrow_RPC()
  {
    this.firedParticles.Play();
    Vector3 forward = this.hitTarget - this.transform.position;
    ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.trailParticles, this.transform.position + forward * 0.5f, Quaternion.identity);
    particleSystem.shape.radius = Vector3.Distance(this.hitTarget, this.transform.position) / 2f;
    particleSystem.transform.rotation = Quaternion.LookRotation(forward, this.transform.up);
    if ((Object) this.targetCharacter != (Object) null)
      this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, (float) this.damagePips * 0.025f);
    Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, this.hitTarget, Quaternion.identity);
    arrow.transform.rotation = (Quaternion) quaternion.LookRotation((float3) forward, (float3) Vector3.up);
    arrow.transform.parent = this.target;
    arrow.stuckArrow(true);
    Rigidbody component;
    if (this.target.gameObject.TryGetComponent<Rigidbody>(out component))
      component.AddForce(forward.normalized * this.force, ForceMode.Impulse);
    this.arrows.Add(arrow);
    this.checkMaxArrows();
    this.StartCoroutine(Reload());

    IEnumerator Reload()
    {
      this.target = (Transform) null;
      this.moveAcumulator = 0.0f;
      this.targetCharacter = (Character) null;
      this.reloading = true;
      yield return (object) new WaitForSeconds(this.reloadTime);
      this.reloading = false;
    }
  }

  [PunRPC]
  public void WarningArrows_RPC(int count)
  {
    for (int index = 0; index < count; ++index)
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.position + (this.transform.up * UnityEngine.Random.Range(-1f, 1f) + this.transform.right * UnityEngine.Random.Range(-1f, 1f)), this.transform.forward, out hitInfo, this.range))
      {
        MonoBehaviour.print((object) hitInfo.collider.gameObject.name);
        Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, hitInfo.point, Quaternion.identity);
        arrow.stuckArrow(true);
        arrow.transform.rotation = (Quaternion) quaternion.LookRotation((float3) (hitInfo.point - this.transform.position), (float3) Vector3.up);
        arrow.transform.Rotate(new Vector3((float) UnityEngine.Random.Range(-10, 10), (float) UnityEngine.Random.Range(-10, 10), (float) UnityEngine.Random.Range(-10, 10)));
        arrow.transform.parent = hitInfo.transform;
      }
    }
  }

  public void checkMaxArrows()
  {
    if (this.arrows.Count < this.maxArrows)
      return;
    this.emptyParticles.Play();
    this.empty = true;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position + this.shooter.forward * this.range, this.castRadius);
    Gizmos.DrawLine(this.transform.position, this.transform.position + this.shooter.forward * this.range);
    Gizmos.DrawRay(this.transform.position, this.hitTarget - this.transform.position);
  }
}
