// Decompiled with JetBrains decompiler
// Type: BeeSwarm
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zorro.Settings;

#nullable disable
public class BeeSwarm : MonoBehaviourPun
{
  public int beehiveID;
  public Beehive beehive;
  [SerializeField]
  private float beehiveDangerTick;
  [SerializeField]
  private float beeDispersalTick;
  private float beehiveDangerTime;
  private Rigidbody rb;
  public Character currentAggroCharacter;
  public float defaultAggroDistance;
  public float hiveAggroDistance;
  public float deAggroDistance;
  public float maxHiveDistance;
  public float movementForce;
  public float movementForceAngry;
  public float beesAngerTimeHiveStolen = 8f;
  public float beesAngerTimeHiveDestroyed = 20f;
  public float beesDispersalTime = 6f;
  public float poisonOverTime;
  public float poisonOverTimeAngry;
  public StatusField stingerField;
  public ParticleSystem beeParticles;
  public Material bingBongMaterial;
  public ParticleSystemForceField beeForceField;
  private Vector3 lastSawBeehivePosition;
  private float currentHiveDistance;
  private float currentLastSawHiveDistance;
  public bool beesAngry;
  public AudioSource beeIdleLoop;
  public AudioSource beeAngryLoop;
  private BugPhobiaSetting setting;
  private bool hiveDestroyed;
  private bool dispersing;

  private bool canSeeHive => (double) this.currentHiveDistance <= (double) this.maxHiveDistance;

  protected void Awake()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.lastSawBeehivePosition = this.transform.position;
  }

  private void Start()
  {
    this.setting = GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>();
    if (this.setting == null || this.setting.Value != OffOnMode.ON)
      return;
    this.beeParticles.GetComponent<ParticleSystemRenderer>().material = this.bingBongMaterial;
  }

  public void SetBeehive(Beehive hive)
  {
    this.beehiveID = hive.instanceID;
    this.beehive = hive;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.defaultAggroDistance);
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.hiveAggroDistance);
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(this.transform.position, this.deAggroDistance);
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(this.transform.position, this.maxHiveDistance);
  }

  private void Update()
  {
    if (this.dispersing || !PhotonNetwork.InRoom)
      return;
    if (this.photonView.IsMine)
    {
      bool flag = (double) this.beehiveDangerTick > 0.0;
      if (this.beesAngry != flag)
        this.photonView.RPC("SetBeesAngryRPC", RpcTarget.AllBuffered, (object) flag);
    }
    this.stingerField.statusAmountPerSecond = this.beesAngry ? this.poisonOverTimeAngry : this.poisonOverTime;
    if ((Object) this.beehive == (Object) null)
      this.TryGetBeehive();
    this.UpdateAggro();
    if (!this.photonView.IsMine || !this.beesAngry)
      return;
    this.beehiveDangerTick = Mathf.Max(this.beehiveDangerTick - Time.deltaTime, 0.0f);
  }

  [PunRPC]
  public void SetBeesAngryRPC(bool angry)
  {
    Debug.Log((object) $"Setting bees angry: {angry}");
    this.beesAngry = angry;
  }

  private void FixedUpdate()
  {
    if (!PhotonNetwork.InRoom)
      return;
    this.UpdateBeehavior();
  }

  private void UpdateBeehavior()
  {
    if (!this.photonView.IsMine)
      return;
    this.currentHiveDistance = (Object) this.beehive == (Object) null ? float.MaxValue : Vector3.Distance(this.beehive.transform.position, this.transform.position);
    this.currentLastSawHiveDistance = Vector3.Distance(this.lastSawBeehivePosition, this.transform.position);
    if ((Object) this.currentAggroCharacter == (Object) null)
    {
      this.rb.AddForce((this.lastSawBeehivePosition - this.transform.position).normalized * (this.movementForce * Time.fixedDeltaTime), ForceMode.Acceleration);
      this.UpdateDisperse();
      this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0.0f, Time.deltaTime * 2f);
      this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0.75f, Time.deltaTime * 2f);
    }
    else
    {
      this.rb.AddForce((this.currentAggroCharacter.Center - this.transform.position).normalized * ((this.beesAngry ? this.movementForceAngry : this.movementForce) * Time.fixedDeltaTime), ForceMode.Acceleration);
      this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0.75f, Time.deltaTime * 2f);
      this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0.0f, Time.deltaTime * 2f);
    }
  }

  private void UpdateDisperse()
  {
    if ((Object) this.currentAggroCharacter == (Object) null && !this.canSeeHive)
    {
      this.beeDispersalTick += Time.fixedDeltaTime;
      if ((double) this.beeDispersalTick < (double) this.beesDispersalTime)
        return;
      this.Disperse();
    }
    else
      this.beeDispersalTick = 0.0f;
  }

  private void GetAngry(float time) => this.beehiveDangerTick = time;

  public void HiveDestroyed(Vector3 atPosition)
  {
    if ((double) Vector3.Distance(this.transform.position, atPosition) > (double) this.hiveAggroDistance)
      return;
    this.hiveDestroyed = true;
    this.lastSawBeehivePosition = atPosition;
    this.GetAngry(this.beesAngerTimeHiveDestroyed);
  }

  private void Disperse() => this.photonView.RPC("DisperseRPC", RpcTarget.All);

  [PunRPC]
  public void DisperseRPC()
  {
    this.dispersing = true;
    this.StartCoroutine(this.DisperseRoutine());
  }

  private IEnumerator DisperseRoutine()
  {
    BeeSwarm beeSwarm = this;
    float tick = 0.0f;
    if ((bool) (Object) beeSwarm.stingerField)
      Object.Destroy((Object) beeSwarm.stingerField.gameObject);
    while ((double) tick < 1.0)
    {
      ParticleSystem.EmissionModule emission = beeSwarm.beeParticles.emission;
      emission.rateOverTimeMultiplier = Mathf.Max(emission.rateOverTimeMultiplier - Time.deltaTime, 0.0f);
      float min = Mathf.Max(beeSwarm.beeForceField.gravity.constantMin - Time.deltaTime, 0.0f);
      float max = Mathf.Max(beeSwarm.beeForceField.gravity.constantMax - Time.deltaTime, 0.0f);
      beeSwarm.beeForceField.gravity = new ParticleSystem.MinMaxCurve(min, max);
      tick += Time.deltaTime;
      yield return (object) null;
    }
    while ((double) tick < 4.0)
    {
      tick += Time.deltaTime;
      yield return (object) null;
    }
    if (beeSwarm.photonView.IsMine)
      PhotonNetwork.Destroy(beeSwarm.gameObject);
  }

  private void UpdateAggro()
  {
    if (!this.photonView.IsMine)
      return;
    this.TryDeAggro();
    if ((Object) this.beehive != (Object) null && this.canSeeHive)
    {
      this.lastSawBeehivePosition = this.beehive.transform.position;
      if ((Object) this.beehive.item.holderCharacter != (Object) null)
      {
        this.beehiveDangerTick = this.beesAngerTimeHiveStolen;
        this.currentAggroCharacter = this.beehive.item.holderCharacter;
        return;
      }
    }
    if (!((Object) this.currentAggroCharacter == (Object) null))
      return;
    float num1 = float.MaxValue;
    Character character = (Character) null;
    if ((Object) this.beehive != (Object) null && (double) this.currentLastSawHiveDistance > (double) this.maxHiveDistance - (double) this.hiveAggroDistance)
      return;
    float num2 = this.beesAngry ? this.hiveAggroDistance : this.defaultAggroDistance;
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      Character allCharacter = Character.AllCharacters[index];
      float num3 = Vector3.Distance(allCharacter.Center, this.transform.position);
      if (allCharacter.data.fullyConscious && (double) num3 < (double) num2 && (double) num3 < (double) num1)
      {
        num1 = num3;
        character = allCharacter;
      }
    }
    this.currentAggroCharacter = character;
  }

  private void TryDeAggro()
  {
    if (!(bool) (Object) this.currentAggroCharacter)
      return;
    if (!this.currentAggroCharacter.data.fullyConscious)
      this.currentAggroCharacter = (Character) null;
    else if (!this.hiveDestroyed && (double) this.currentLastSawHiveDistance > (double) this.maxHiveDistance)
    {
      this.currentAggroCharacter = (Character) null;
    }
    else
    {
      if ((double) Vector3.Distance(this.currentAggroCharacter.Center, this.transform.position) <= (this.beesAngry ? (double) this.hiveAggroDistance : (double) this.deAggroDistance))
        return;
      this.currentAggroCharacter = (Character) null;
    }
  }

  private void TryGetBeehive()
  {
    Beehive beehive = Beehive.GetBeehive(this.beehiveID);
    if (!((Object) beehive != (Object) null))
      return;
    this.beehive = beehive;
    beehive.currentBees = this;
    Debug.Log((object) $"Reattached to beehive #{this.beehiveID}");
  }
}
