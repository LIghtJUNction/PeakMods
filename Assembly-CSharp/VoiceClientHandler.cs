// Decompiled with JetBrains decompiler
// Type: VoiceClientHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using UnityEngine;

#nullable disable
public class VoiceClientHandler : MonoBehaviour
{
  private static VoiceConnection m_VoiceConnection;
  private static Recorder m_LocalRecorder;

  private void Awake()
  {
    PunVoiceClient component = this.GetComponent<PunVoiceClient>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    else if ((UnityEngine.Object) PunVoiceClient.Instance != (UnityEngine.Object) component)
    {
      Debug.Log((object) "Already Found VoiceClient, Destroying...");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.transform.SetParent((Transform) null);
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
    }
  }

  private void Start()
  {
    VoiceClientHandler.m_VoiceConnection = this.GetComponent<VoiceConnection>();
    if (VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
      VoiceClientHandler.m_VoiceConnection.Client.StateChanged += new Action<ClientState, ClientState>(this.OnStateChanged);
    else
      VoiceClientHandler.InitNetworkVoice();
  }

  private void OnStateChanged(ClientState state, ClientState toState)
  {
    if (toState != ClientState.Joined)
      return;
    VoiceClientHandler.InitNetworkVoice();
  }

  public static void InitNetworkVoice()
  {
    if ((UnityEngine.Object) VoiceClientHandler.m_LocalRecorder == (UnityEngine.Object) null || (UnityEngine.Object) VoiceClientHandler.m_VoiceConnection == (UnityEngine.Object) null || VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
      return;
    VoiceClientHandler.m_VoiceConnection.Client.LoadBalancingPeer.OpChangeGroups(Array.Empty<byte>(), Array.Empty<byte>());
    VoiceClientHandler.m_LocalRecorder.InterestGroup = (byte) 0;
  }

  public static void LocalPlayerAssigned(Recorder r)
  {
    VoiceClientHandler.m_LocalRecorder = r;
    VoiceClientHandler.InitNetworkVoice();
  }
}
