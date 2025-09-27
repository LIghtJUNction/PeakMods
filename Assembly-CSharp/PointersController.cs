// Decompiled with JetBrains decompiler
// Type: PointersController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.PUN;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonVoiceView))]
public class PointersController : MonoBehaviour
{
  [SerializeField]
  private GameObject pointerDown;
  [SerializeField]
  private GameObject pointerUp;
  private PhotonVoiceView photonVoiceView;

  private void Awake()
  {
    this.photonVoiceView = this.GetComponent<PhotonVoiceView>();
    this.SetActiveSafe(this.pointerUp, false);
    this.SetActiveSafe(this.pointerDown, false);
  }

  private void Update()
  {
    this.SetActiveSafe(this.pointerDown, this.photonVoiceView.IsSpeaking);
    this.SetActiveSafe(this.pointerUp, this.photonVoiceView.IsRecording);
  }

  private void SetActiveSafe(GameObject go, bool active)
  {
    if (!((Object) go != (Object) null) || go.activeSelf == active)
      return;
    go.SetActive(active);
  }
}
