// Decompiled with JetBrains decompiler
// Type: NetworkStatsPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public class NetworkStatsPage : DebugPage
{
  private Label m_label;
  private NetworkStats stats;
  private const long OneKb = 1024 /*0x0400*/;
  private const long OneMb = 1048576 /*0x100000*/;
  private const long OneGb = 1073741824 /*0x40000000*/;
  private const long OneTb = 1099511627776 /*0x010000000000*/;

  public NetworkStatsPage(NetworkStats networkStats)
  {
    this.stats = networkStats;
    this.styleSheets.Add(SingletonAsset<CoreGlobalDependencies>.Instance.DebugPageStyleSheets);
    PhotonNetwork.NetworkStatisticsEnabled = true;
    this.m_label = new Label();
    this.m_label.AddToClassList("info");
    this.Add((VisualElement) this.m_label);
  }

  public override void Update()
  {
    base.Update();
    string str1 = $"bytes in: {NetworkStatsPage.ToPrettySize(this.stats.m_lastRecievedDelta * 8L)}/s, out: {NetworkStatsPage.ToPrettySize(this.stats.m_lastSentDelta * 8L)}/s";
    List<(string, ulong)> bytesDeltaSent = this.stats.GetBytesDeltaSent();
    bytesDeltaSent.Sort((Comparison<(string, ulong)>) ((t1, t2) => t2.Item2.CompareTo(t1.Item2)));
    foreach ((string, ulong) tuple in bytesDeltaSent)
    {
      string str2 = tuple.Item1;
      str1 = $"{str1}{Environment.NewLine}{str2}: {NetworkStatsPage.ToPrettySize((long) tuple.Item2)}/s";
    }
    this.m_label.text = str1;
  }

  public static string ToPrettySize(long value, int decimalPlaces = 0)
  {
    double num1 = Math.Round((double) value / 1099511627776.0, decimalPlaces);
    double num2 = Math.Round((double) value / 1073741824.0, decimalPlaces);
    double num3 = Math.Round((double) value / 1048576.0, decimalPlaces);
    double num4 = Math.Round((double) value / 1024.0, decimalPlaces);
    if (num1 > 1.0)
      return $"{num1}Tb";
    if (num2 > 1.0)
      return $"{num2}Gb";
    if (num3 > 1.0)
      return $"{num3}Mb";
    return num4 <= 1.0 ? $"{Math.Round((double) value, decimalPlaces)}B" : $"{num4}Kb";
  }
}
