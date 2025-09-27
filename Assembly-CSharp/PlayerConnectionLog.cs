// Decompiled with JetBrains decompiler
// Type: PlayerConnectionLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

#nullable disable
public class PlayerConnectionLog : MonoBehaviourPunCallbacks
{
  public TextMeshProUGUI text;
  private List<string> currentLog = new List<string>();
  private StringBuilder sb = new StringBuilder();
  public Color joinedColor;
  public Color leftColor;
  public Color userColor;
  public SFX_Instance sfxJoin;
  public SFX_Instance sfxLeave;

  private void Awake()
  {
    GlobalEvents.OnAchievementThrown += new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown);
  }

  private void OnDestroy()
  {
    GlobalEvents.OnAchievementThrown -= new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown);
  }

  private void RebuildString()
  {
    this.sb.Clear();
    foreach (string str in this.currentLog)
    {
      this.sb.Append(str);
      this.sb.Append("\n");
    }
    this.text.text = this.sb.ToString();
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!newPlayer.IsLocal && newPlayer.NickName == "Bing Bong")
      return;
    string newValue = $"{this.GetColorTag(this.userColor)} {newPlayer.NickName}</color>";
    this.AddMessage($"{this.GetColorTag(this.joinedColor)}{LocalizedText.GetText("JOINEDTHEEXPEDITION").Replace("#", newValue)}</color>");
    if (!(bool) (UnityEngine.Object) this.sfxJoin)
      return;
    this.sfxJoin.Play();
  }

  public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
  {
    if (newPlayer.IsLocal || newPlayer.NickName == "Bing Bong")
      return;
    string newValue = $"{this.GetColorTag(this.userColor)} {newPlayer.NickName}</color>";
    this.AddMessage($"{this.GetColorTag(this.leftColor)}{LocalizedText.GetText("LEFTTHEEXPEDITION").Replace("#", newValue)}</color>");
    if (!(bool) (UnityEngine.Object) this.sfxLeave)
      return;
    this.sfxLeave.Play();
  }

  public void TestAddJoin()
  {
    string newValue = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
    this.AddMessage($"{this.GetColorTag(this.joinedColor)}{LocalizedText.GetText("JOINEDTHEEXPEDITION").Replace("#", newValue)}</color>");
  }

  public void TestAddLeft()
  {
    string newValue = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
    this.AddMessage($"{this.GetColorTag(this.leftColor)}{LocalizedText.GetText("LEFTTHEEXPEDITION").Replace("#", newValue)}</color>");
  }

  private string GetColorTag(Color c) => $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>";

  private void AddMessage(string s)
  {
    this.currentLog.Add(s);
    this.RebuildString();
    this.StartCoroutine(this.TimeoutMessageRoutine());
  }

  private IEnumerator TimeoutMessageRoutine()
  {
    yield return (object) new WaitForSeconds(8f);
    this.currentLog.RemoveAt(0);
    this.RebuildString();
  }

  private void TestAchievementThrown(ACHIEVEMENTTYPE type)
  {
    if (!Application.isEditor && !Debug.isDebugBuild)
      return;
    string str = $"{this.GetColorTag(this.userColor)} {type.ToString()}</color>";
    this.AddMessage($"{this.GetColorTag(this.joinedColor)}Got Badge: </color>{str}");
  }
}
