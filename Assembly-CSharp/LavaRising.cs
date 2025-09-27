// Decompiled with JetBrains decompiler
// Type: LavaRising
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.PhotonUtility;

#nullable disable
public class LavaRising : Singleton<LavaRising>
{
  public Rigidbody lava;
  public Transform topTransform;
  public float initialWaitTime = 1f;
  public float travelTime = 60f;
  public bool debug;
  public float debugInitialWaitTime = 1f;
  public float debugTravelTime = 60f;
  private bool shownLavaRisingMessage;
  private ListenerHandle debugCommandHandle;
  private float syncTime;
  [SerializeField]
  private float startHeight;

  public float timeTraveled { get; set; }

  public bool started { get; set; }

  public bool ended { get; set; }

  public float secondsWaitedToStart { get; set; }

  protected override void Awake()
  {
    base.Awake();
    if (!this.debug)
      return;
    this.initialWaitTime = this.debugInitialWaitTime;
    this.travelTime = this.debugTravelTime;
  }

  private void Start()
  {
    this.startHeight = this.lava.transform.position.y;
    Debug.Log((object) ("Initialized lava height: " + this.startHeight.ToString()));
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
  }

  private void Update()
  {
    if (PhotonNetwork.IsMasterClient && Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.TheKiln || Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.Peak)
    {
      bool flag = false;
      this.syncTime += Time.deltaTime;
      if (!this.started && (double) this.secondsWaitedToStart <= 0.0 && Ascents.fogEnabled)
        this.StartWaiting();
      if (!this.started && (double) this.secondsWaitedToStart > 0.0)
      {
        this.secondsWaitedToStart += Time.deltaTime;
        if ((double) this.secondsWaitedToStart > (double) this.initialWaitTime && Ascents.fogEnabled)
        {
          flag = true;
          this.started = true;
        }
      }
      if ((double) this.syncTime > 15.0)
      {
        this.syncTime = 0.0f;
        flag = true;
      }
      if (flag)
      {
        Debug.Log((object) "Syncing Lava Rising to others...");
        GameUtils.instance.SyncLava(this.started, this.ended, this.timeTraveled, this.secondsWaitedToStart);
      }
    }
    if (!this.started || this.ended)
      return;
    if (!this.shownLavaRisingMessage)
    {
      GUIManager.instance.TheLavaRises();
      GamefeelHandler.instance.AddPerlinShake(5f, 3f);
      this.shownLavaRisingMessage = true;
      Debug.Log((object) "Lava rising started.");
    }
    this.timeTraveled += Time.deltaTime;
    this.lava.MovePosition(new Vector3(this.lava.transform.position.x, Mathf.Lerp(this.startHeight, this.topTransform.position.y, this.timeTraveled / this.travelTime), this.lava.transform.position.z));
    if ((double) this.timeTraveled <= (double) this.travelTime)
      return;
    this.EndRising();
  }

  public void RecieveLavaData(bool started, bool ended, float time, float timeWaited)
  {
    this.started = started;
    this.ended = ended;
    this.timeTraveled = time;
    this.secondsWaitedToStart = timeWaited;
    Debug.Log((object) $"Handle Lava Rising package: started: {started}, ended: {ended}, seconds waited: {this.secondsWaitedToStart}, time traveled: {this.timeTraveled} starting position: {this.startHeight} total time: {this.travelTime}");
  }

  public void StartWaiting()
  {
    if ((double) this.secondsWaitedToStart > 0.0)
    {
      Debug.LogError((object) "Tried to start waiting for lava rising but already rising!");
    }
    else
    {
      Debug.Log((object) "Starting wait for lava rising");
      this.secondsWaitedToStart = Time.deltaTime;
    }
  }

  private void EndRising()
  {
    Debug.Log((object) "Ending lava rising.");
    this.ended = true;
  }
}
