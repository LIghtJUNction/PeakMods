// Decompiled with JetBrains decompiler
// Type: WindChillZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class WindChillZone : MonoBehaviour
{
  public Vector2 windTimeRangeOn;
  public Vector2 windTimeRangeOff;
  [Range(0.0f, 1f)]
  public float lightVolumeSampleThreshold_lower;
  [Range(0.0f, 1f)]
  public float lightVolumeSampleThreshold_margin;
  public Bounds windZoneBounds;
  internal Vector3 currentWindDirection;
  private Color gizmoColor = new Color(0.0f, 0.0f, 1f, 0.5f);
  private float untilSwitch;
  public float windChillPerSecond = 0.01f;
  public float windForce = 15f;
  internal float hasBeenActiveFor;
  private PhotonView view;
  public bool characterInsideBounds;
  public bool windActive;
  public float windPlayerFactor;
  public bool setSlippy;

  private void Awake() => this.windZoneBounds.center = this.transform.position;

  private void OnDrawGizmosSelected()
  {
    this.windZoneBounds.center = this.transform.position;
    Gizmos.color = this.gizmoColor;
    Gizmos.DrawCube(this.windZoneBounds.center, this.windZoneBounds.extents * 2f);
  }

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    this.HandleTime();
    this.characterInsideBounds = this.windZoneBounds.Contains(Character.observedCharacter.Center);
    if (this.windActive)
      this.hasBeenActiveFor += Time.deltaTime;
    else
      this.hasBeenActiveFor = 0.0f;
    if (this.characterInsideBounds && this.windActive)
    {
      if (!((Object) Character.observedCharacter == (Object) Character.localCharacter))
        return;
      this.ApplyCold();
    }
    else
      this.windPlayerFactor = 0.0f;
  }

  private void HandleTime()
  {
    this.untilSwitch -= Time.deltaTime;
    if ((double) this.untilSwitch >= 0.0 || !PhotonNetwork.IsMasterClient)
      return;
    this.view.RPC("RPCA_ToggleWind", RpcTarget.All, (object) !this.windActive, (object) this.RandomWindDirection());
    double nextWindTime = (double) this.GetNextWindTime(this.windActive);
  }

  private void FixedUpdate()
  {
    if ((Object) Character.localCharacter == (Object) null || !this.characterInsideBounds || !this.windActive || !((Object) Character.observedCharacter == (Object) Character.localCharacter))
      return;
    this.AddWindForceToCharacter();
  }

  private void ApplyCold()
  {
    this.windPlayerFactor = WindChillZone.GetWindIntensityAtPoint(Character.localCharacter.Center, this.lightVolumeSampleThreshold_lower, this.lightVolumeSampleThreshold_margin);
    Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.windPlayerFactor * this.windChillPerSecond * Time.deltaTime * Mathf.Clamp01(this.hasBeenActiveFor * 0.2f));
    if (!this.setSlippy)
      return;
    Character.localCharacter.data.slippy = Mathf.Clamp01(Mathf.Max(Character.localCharacter.data.slippy, this.windPlayerFactor * 10f));
  }

  private void AddWindForceToCharacter()
  {
    Character.localCharacter.AddForce(this.currentWindDirection * this.windForce * this.windPlayerFactor, 0.5f);
  }

  private Vector3 RandomWindDirection()
  {
    return Vector3.Lerp(Vector3.right * ((double) Random.value > 0.5 ? 1f : -1f), Vector3.forward, 0.2f).normalized;
  }

  internal static float GetWindIntensityAtPoint(
    Vector3 point,
    float thresholdLower,
    float thresholdMargin)
  {
    float num = LightVolume.Instance().SamplePositionAlpha(point);
    return (double) num <= (double) thresholdLower + (double) thresholdMargin ? ((double) num >= (double) thresholdLower ? Util.RangeLerp(0.0f, 1f, thresholdLower, thresholdLower + thresholdMargin, num) : 0.0f) : 1f;
  }

  [PunRPC]
  private void RPCA_ToggleWind(bool set, Vector3 windDir)
  {
    this.windActive = set;
    this.untilSwitch = this.GetNextWindTime(this.windActive);
    this.currentWindDirection = windDir;
  }

  private float GetNextWindTime(bool windActive)
  {
    return windActive ? Random.Range(this.windTimeRangeOn.x, this.windTimeRangeOn.y) : Random.Range(this.windTimeRangeOff.x, this.windTimeRangeOff.y);
  }
}
