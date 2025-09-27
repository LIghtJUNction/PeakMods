// Decompiled with JetBrains decompiler
// Type: CloudAPI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Networking;
using Zorro.Core;

#nullable disable
public static class CloudAPI
{
  public static void CheckVersion(Action<LoginResponse> response)
  {
    GameHandler.AddStatus<QueryingGameTimeStatus>((GameStatus) new QueryingGameTimeStatus());
    BuildVersion buildVersion = new BuildVersion(Application.version);
    string uri = "https://peaklogin-beta.azurewebsites.net/api/VersionCheck?version=" + buildVersion.ToMatchmaking();
    if (buildVersion.BuildName == "beta")
      uri = "https://peaklogin-beta.azurewebsites.net/api/VersionCheck?version=" + buildVersion.ToMatchmaking();
    Debug.Log((object) ("Sending GET Request to: " + uri));
    UnityWebRequest request = UnityWebRequest.Get(uri);
    request.SendWebRequest().completed += (Action<AsyncOperation>) (_ =>
    {
      GameHandler.ClearStatus<QueryingGameTimeStatus>();
      if (request.result == UnityWebRequest.Result.Success)
      {
        string text = request.downloadHandler.text;
        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(text);
        Debug.Log((object) ("Got message: " + text));
        Action<LoginResponse> action = response;
        if (action == null)
          return;
        action(loginResponse);
      }
      else
      {
        Debug.Log((object) ("Got error: " + request.error));
        if (request.result == UnityWebRequest.Result.ConnectionError)
          return;
        Action<LoginResponse> action = response;
        if (action == null)
          return;
        action(new LoginResponse() { VersionOkay = false });
      }
    });
  }

  public static void VerifyLobby(ulong lobbyID, Action<string> response)
  {
    string uri = "https://peaklogin.azurewebsites.net/api/VerifyLobby?lobby=" + lobbyID.ToString();
    Debug.Log((object) ("Sending GET Request to: " + uri));
    UnityWebRequest request = UnityWebRequest.Get(uri);
    request.SendWebRequest().completed += (Action<AsyncOperation>) (_ =>
    {
      GameHandler.ClearStatus<QueryingGameTimeStatus>();
      if (request.result == UnityWebRequest.Result.Success)
      {
        string text = request.downloadHandler.text;
        Debug.Log((object) ("Got message: " + text));
        Action<string> action = response;
        if (action == null)
          return;
        action(text);
      }
      else
        Debug.Log((object) ("Failed to verify lobby: " + request.error));
    });
  }
}
