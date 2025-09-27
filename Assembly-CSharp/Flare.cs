// Decompiled with JetBrains decompiler
// Type: Flare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Flare : ItemComponent
{
  private TrackableNetworkObject trackable;
  public TrackNetworkedObject flareVFXPrefab;
  public Color flareColor;

  public override void Awake()
  {
    base.Awake();
    this.trackable = this.GetComponent<TrackableNetworkObject>();
  }

  public override void OnInstanceDataSet()
  {
    if (!this.HasData(DataEntryKey.Color))
      return;
    this.flareColor = this.GetData<ColorItemData>(DataEntryKey.Color).Value;
  }

  private void Update()
  {
    bool flag = this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
    this.item.UIData.canPocket = !flag;
    this.item.UIData.canBackpack = !flag;
    if (flag && !this.trackable.hasTracker)
      this.EnableFlareVisuals();
    if (!flag || !(bool) (Object) this.item.holderCharacter || !Singleton<MountainProgressHandler>.Instance.IsAtPeak(this.item.holderCharacter.Center) || Singleton<PeakHandler>.Instance.summonedHelicopter)
      return;
    this.GetComponent<PhotonView>().RPC("TriggerHelicopter", RpcTarget.AllBuffered);
  }

  [PunRPC]
  public void TriggerHelicopter() => Singleton<PeakHandler>.Instance.SummonHelicopter();

  public void LightFlare()
  {
    this.GetComponent<PhotonView>().RPC("SetFlareLitRPC", RpcTarget.AllBuffered);
  }

  [PunRPC]
  public void SetFlareLitRPC()
  {
    if ((bool) (Object) this.item.holderCharacter)
    {
      this.flareColor = this.item.holderCharacter.refs.customization.PlayerColor;
      this.flareColor.a = 1f;
      this.GetData<ColorItemData>(DataEntryKey.Color).Value = this.flareColor;
      Debug.Log((object) ("Set flare color to " + this.GetData<ColorItemData>(DataEntryKey.Color).Value.ToString()));
    }
    this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
  }

  public void EnableFlareVisuals()
  {
    Debug.Log((object) $"Lighting flare with photon ID {this.photonView.ViewID} with instance ID {this.trackable.instanceID}");
    TrackNetworkedObject component = Object.Instantiate<TrackNetworkedObject>(this.flareVFXPrefab, this.transform.position, this.transform.rotation).GetComponent<TrackNetworkedObject>();
    component.SetObject(this.trackable);
    component.gameObject.GetComponent<ParticleSystem>().main.startColor = (ParticleSystem.MinMaxGradient) this.flareColor;
    Debug.Log((object) ("Lit flare with color " + this.flareColor.ToString()));
  }
}
