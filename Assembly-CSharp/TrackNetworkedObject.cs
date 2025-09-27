// Decompiled with JetBrains decompiler
// Type: TrackNetworkedObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class TrackNetworkedObject : MonoBehaviour
{
  public int trackedObjectID = -1;
  public TrackableNetworkObject trackedObject;
  public Vector3 offset;
  private int lostTrackableTick;

  private void OnEnable()
  {
    TrackableNetworkObject.OnTrackableObjectCreated += new Action<int>(this.TryReattachToTrackedObject);
  }

  private void OnDisable()
  {
    TrackableNetworkObject.OnTrackableObjectCreated -= new Action<int>(this.TryReattachToTrackedObject);
  }

  private void TryReattachToTrackedObject(int ID) => this.TryGetTrackedObject();

  private void TryGetTrackedObject()
  {
    if (this.trackedObjectID == -1)
    {
      Debug.LogError((object) "TrackNetworkObject has a value of -1. This should never happen.");
      this.enabled = false;
    }
    else
    {
      TrackableNetworkObject trackableObject = TrackableNetworkObject.GetTrackableObject(this.trackedObjectID);
      if ((UnityEngine.Object) trackableObject != (UnityEngine.Object) null)
      {
        this.SetObject(trackableObject);
        this.lostTrackableTick = 0;
      }
      else
      {
        ++this.lostTrackableTick;
        if (this.lostTrackableTick <= 20)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }
  }

  public void SetObject(TrackableNetworkObject trackableObject)
  {
    this.trackedObject = trackableObject;
    this.trackedObjectID = trackableObject.instanceID;
    this.trackedObject.currentTracker = this;
    Debug.Log((object) $"Object {this.gameObject.GetHashCode()} Reconnected to trackable object {this.trackedObjectID} with photon ID {trackableObject.photonView.ViewID}");
  }

  private void LateUpdate()
  {
    if ((UnityEngine.Object) this.trackedObject == (UnityEngine.Object) null)
      this.TryGetTrackedObject();
    if (!((UnityEngine.Object) this.trackedObject != (UnityEngine.Object) null))
      return;
    this.transform.rotation = this.trackedObject.transform.rotation;
    this.transform.position = this.trackedObject.transform.TransformPoint(this.offset);
  }
}
