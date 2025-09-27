// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.FriendItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.Demo;

public class FriendItem : MonoBehaviour
{
  public Text NameLabel;
  public Text StatusLabel;
  public Text Health;

  [HideInInspector]
  public string FriendId
  {
    set => this.NameLabel.text = value;
    get => this.NameLabel.text;
  }

  public void Awake() => this.Health.text = string.Empty;

  public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
  {
    string str1;
    switch (status)
    {
      case 1:
        str1 = "Invisible";
        break;
      case 2:
        str1 = "Online";
        break;
      case 3:
        str1 = "Away";
        break;
      case 4:
        str1 = "Do not disturb";
        break;
      case 5:
        str1 = "Looking For Game/Group";
        break;
      case 6:
        str1 = "Playing";
        break;
      default:
        str1 = "Offline";
        break;
    }
    this.StatusLabel.text = str1;
    if (!gotMessage)
      return;
    string str2 = string.Empty;
    if (message != null && message is string[] strArray && strArray.Length >= 2)
      str2 = strArray[1] + "%";
    this.Health.text = str2;
  }
}
