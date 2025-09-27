// Decompiled with JetBrains decompiler
// Type: Dynamite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;

#nullable disable
public class Dynamite : ItemComponent
{
  private TrackableNetworkObject trackable;
  public TrackNetworkedObject smokeVFXPrefab;
  public GameObject explosionPrefab;
  public float startingFuseTime;
  public float lightFuseRadius;
  [SerializeField]
  private float fuseTime;
  public Transform sparks;
  public Transform sparksPhotosensitive;
  private PhotosensitiveSetting setting;

  public override void Awake()
  {
    base.Awake();
    this.trackable = this.GetComponent<TrackableNetworkObject>();
    this.item.UIData.canPocket = false;
  }

  private void Start()
  {
    this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
  }

  public override void OnInstanceDataSet()
  {
    this.fuseTime = this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
  }

  private void Update()
  {
    if (!this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
    {
      this.TestLightWick();
    }
    else
    {
      if (!this.trackable.hasTracker)
        this.EnableFlareVisuals();
      if (this.setting.Value != OffOnMode.ON)
        this.sparks.gameObject.SetActive(true);
      else
        this.sparksPhotosensitive.gameObject.SetActive(true);
      this.fuseTime = this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
      this.item.SetUseRemainingPercentage(this.fuseTime / this.startingFuseTime);
      if (!this.photonView.IsMine)
        return;
      this.fuseTime -= Time.deltaTime;
      if ((double) this.fuseTime <= 0.0)
      {
        if ((UnityEngine.Object) Character.localCharacter.data.currentItem == (UnityEngine.Object) this.item)
        {
          Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f);
          Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
          Character.localCharacter.refs.afflictions.UpdateWeight();
        }
        this.photonView.RPC("RPC_Explode", RpcTarget.All);
        PhotonNetwork.Destroy(this.gameObject);
        this.item.ClearDataFromBackpack();
        this.fuseTime = 0.0f;
      }
      this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuseTime;
    }
  }

  [PunRPC]
  private void RPC_Explode()
  {
    UnityEngine.Object.Instantiate<GameObject>(this.explosionPrefab, this.transform.position, this.transform.rotation);
    this.gameObject.SetActive(false);
  }

  private void TestLightWick()
  {
    if (!PhotonNetwork.IsMasterClient || this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
      return;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((double) Vector3.Distance(allCharacter.Center, this.transform.position) < (double) this.lightFuseRadius)
        this.LightFlare();
    }
  }

  private FloatItemData SetupDefaultFuel()
  {
    return new FloatItemData()
    {
      Value = this.startingFuseTime
    };
  }

  [PunRPC]
  public void TriggerHelicopter() => Singleton<PeakHandler>.Instance.SummonHelicopter();

  public void LightFlare() => this.GetComponent<PhotonView>().RPC("SetFlareLitRPC", RpcTarget.All);

  [PunRPC]
  public void SetFlareLitRPC() => this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;

  public void EnableFlareVisuals()
  {
    Debug.Log((object) $"Lighting flare with photon ID {this.photonView.ViewID} with instance ID {this.trackable.instanceID}");
    UnityEngine.Object.Instantiate<TrackNetworkedObject>(this.smokeVFXPrefab, this.transform.position, this.transform.rotation).GetComponent<TrackNetworkedObject>().SetObject(this.trackable);
  }
}
