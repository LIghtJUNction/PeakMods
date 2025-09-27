// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.NamePickGui
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.Demo;

[RequireComponent(typeof (ChatGui))]
public class NamePickGui : MonoBehaviour
{
  private const string UserNamePlayerPref = "NamePickUserName";
  public ChatGui chatNewComponent;
  public InputField idInput;

  public void Start()
  {
    this.chatNewComponent = Object.FindFirstObjectByType<ChatGui>();
    string str = PlayerPrefs.GetString("NamePickUserName");
    if (string.IsNullOrEmpty(str))
      return;
    this.idInput.text = str;
  }

  public void EndEditOnEnter()
  {
    if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
      return;
    this.StartChat();
  }

  public void StartChat()
  {
    ChatGui firstObjectByType = Object.FindFirstObjectByType<ChatGui>();
    firstObjectByType.UserName = this.idInput.text.Trim();
    firstObjectByType.Connect();
    this.enabled = false;
    PlayerPrefs.SetString("NamePickUserName", firstObjectByType.UserName);
  }
}
