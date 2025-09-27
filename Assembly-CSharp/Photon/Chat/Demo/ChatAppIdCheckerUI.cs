// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.ChatAppIdCheckerUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.Demo;

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
  public Text Description;
  public bool WizardOpenedOnce;

  public void Update()
  {
    string str = string.Empty;
    if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
      str = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
    this.Description.text = str;
  }
}
