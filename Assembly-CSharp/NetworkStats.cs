// Decompiled with JetBrains decompiler
// Type: NetworkStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class NetworkStats : Singleton<NetworkStats>
{
  public long m_bytesReceivedLastSecond;
  public long m_lastRecievedDelta;
  public long m_bytesSentLastSecond;
  public long m_lastSentDelta;
  private float m_timer;
  private Dictionary<string, ulong> m_binaryStreamsByType = new Dictionary<string, ulong>();
  private Dictionary<string, ulong> m_binaryStreamsByTypeSecond = new Dictionary<string, ulong>();
  private Dictionary<string, ulong> m_binaryStreamsByTypeDelta = new Dictionary<string, ulong>();

  private void Update()
  {
    this.m_timer += Time.deltaTime;
    if ((double) this.m_timer <= 1.0)
      return;
    --this.m_timer;
    this.m_lastRecievedDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn - this.m_bytesReceivedLastSecond;
    this.m_bytesReceivedLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn;
    this.m_lastSentDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut - this.m_bytesSentLastSecond;
    this.m_bytesSentLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut;
    foreach (KeyValuePair<string, ulong> keyValuePair in this.m_binaryStreamsByType)
      UpdateEntry(keyValuePair.Key, keyValuePair.Value);
    UpdateEntry("VoiceData", (ulong) PhotonVoiceStats.bytesSent);

    void UpdateEntry(string key, ulong value)
    {
      if (this.m_binaryStreamsByTypeSecond.ContainsKey(key))
      {
        ulong num1 = this.m_binaryStreamsByTypeSecond[key];
        ulong num2 = value - num1;
        this.m_binaryStreamsByTypeDelta[key] = num2;
      }
      this.m_binaryStreamsByTypeSecond[key] = value;
    }
  }

  public static void RegisterBytesSent<T>(ulong bytesSent)
  {
    System.Type type = typeof (T);
    if (!Singleton<NetworkStats>.Instance.m_binaryStreamsByType.ContainsKey(type.Name))
      Singleton<NetworkStats>.Instance.m_binaryStreamsByType.Add(type.Name, 0UL);
    Singleton<NetworkStats>.Instance.m_binaryStreamsByType[type.Name] += bytesSent;
  }

  public List<(string, ulong)> GetBytesSent()
  {
    return this.m_binaryStreamsByType.Select<KeyValuePair<string, ulong>, (string, ulong)>((Func<KeyValuePair<string, ulong>, (string, ulong)>) (pair => (pair.Key, pair.Value))).ToList<(string, ulong)>();
  }

  public List<(string, ulong)> GetBytesDeltaSent()
  {
    return this.m_binaryStreamsByTypeDelta.Select<KeyValuePair<string, ulong>, (string, ulong)>((Func<KeyValuePair<string, ulong>, (string, ulong)>) (pair => (pair.Key, pair.Value))).ToList<(string, ulong)>();
  }
}
