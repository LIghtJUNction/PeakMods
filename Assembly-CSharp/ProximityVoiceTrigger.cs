// Decompiled with JetBrains decompiler
// Type: ProximityVoiceTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (Collider))]
[RequireComponent(typeof (Rigidbody))]
public class ProximityVoiceTrigger : VoiceComponent
{
  private List<byte> groupsToAdd = new List<byte>();
  private List<byte> groupsToRemove = new List<byte>();
  [SerializeField]
  private byte[] subscribedGroups;
  private PhotonVoiceView photonVoiceView;
  private PhotonView photonView;

  public byte TargetInterestGroup
  {
    get
    {
      return (Object) this.photonView != (Object) null ? (byte) this.photonView.OwnerActorNr : (byte) 0;
    }
  }

  protected override void Awake()
  {
    this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
    this.photonView = this.GetComponentInParent<PhotonView>();
    this.GetComponent<Collider>().isTrigger = true;
    this.IsLocalCheck();
  }

  private void ToggleTransmission()
  {
    if (!((Object) this.photonVoiceView.RecorderInUse != (Object) null))
      return;
    byte targetInterestGroup = this.TargetInterestGroup;
    if ((int) this.photonVoiceView.RecorderInUse.InterestGroup != (int) targetInterestGroup)
    {
      this.Logger.Log(LogLevel.Info, "Setting RecorderInUse's InterestGroup to {0}", (object) targetInterestGroup);
      this.photonVoiceView.RecorderInUse.InterestGroup = targetInterestGroup;
    }
    this.photonVoiceView.RecorderInUse.RecordingEnabled = true;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!this.IsLocalCheck())
      return;
    ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
    if (!((Object) component != (Object) null))
      return;
    byte targetInterestGroup = component.TargetInterestGroup;
    this.Logger.Log(LogLevel.Debug, "OnTriggerEnter {0}", (object) targetInterestGroup);
    if ((int) targetInterestGroup == (int) this.TargetInterestGroup || targetInterestGroup == (byte) 0 || this.groupsToAdd.Contains(targetInterestGroup))
      return;
    this.groupsToAdd.Add(targetInterestGroup);
  }

  private void OnTriggerExit(Collider other)
  {
    if (!this.IsLocalCheck())
      return;
    ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
    if (!((Object) component != (Object) null))
      return;
    byte targetInterestGroup = component.TargetInterestGroup;
    this.Logger.Log(LogLevel.Debug, "OnTriggerExit {0}", (object) targetInterestGroup);
    if ((int) targetInterestGroup == (int) this.TargetInterestGroup || targetInterestGroup == (byte) 0)
      return;
    if (this.groupsToAdd.Contains(targetInterestGroup))
      this.groupsToAdd.Remove(targetInterestGroup);
    if (this.groupsToRemove.Contains(targetInterestGroup))
      return;
    this.groupsToRemove.Add(targetInterestGroup);
  }

  protected void Update()
  {
    if (!PunVoiceClient.Instance.Client.InRoom)
    {
      this.subscribedGroups = (byte[]) null;
    }
    else
    {
      if (!this.IsLocalCheck())
        return;
      if (this.groupsToAdd.Count > 0 || this.groupsToRemove.Count > 0)
      {
        byte[] groupsToAdd = (byte[]) null;
        byte[] groupsToRemove = (byte[]) null;
        if (this.groupsToAdd.Count > 0)
          groupsToAdd = this.groupsToAdd.ToArray();
        if (this.groupsToRemove.Count > 0)
          groupsToRemove = this.groupsToRemove.ToArray();
        this.Logger.Log(LogLevel.Info, "client of actor number {0} trying to change groups, to_be_removed#={1} to_be_added#={2}", (object) this.TargetInterestGroup, (object) this.groupsToRemove.Count, (object) this.groupsToAdd.Count);
        if (PunVoiceClient.Instance.Client.OpChangeGroups(groupsToRemove, groupsToAdd))
        {
          if (this.subscribedGroups != null)
          {
            List<byte> byteList = new List<byte>();
            for (int index = 0; index < this.subscribedGroups.Length; ++index)
              byteList.Add(this.subscribedGroups[index]);
            for (int index = 0; index < this.groupsToRemove.Count; ++index)
            {
              if (byteList.Contains(this.groupsToRemove[index]))
                byteList.Remove(this.groupsToRemove[index]);
            }
            for (int index = 0; index < this.groupsToAdd.Count; ++index)
            {
              if (!byteList.Contains(this.groupsToAdd[index]))
                byteList.Add(this.groupsToAdd[index]);
            }
            this.subscribedGroups = byteList.ToArray();
          }
          else
            this.subscribedGroups = groupsToAdd;
          this.groupsToAdd.Clear();
          this.groupsToRemove.Clear();
        }
        else
          this.Logger.Log(LogLevel.Error, "Error changing groups");
      }
      this.ToggleTransmission();
    }
  }

  private bool IsLocalCheck()
  {
    if (this.photonView.IsMine)
      return true;
    if (this.enabled)
    {
      this.Logger.Log(LogLevel.Info, "Disabling ProximityVoiceTrigger as does not belong to local player, actor number {0}", (object) this.TargetInterestGroup);
      this.enabled = false;
    }
    return false;
  }
}
