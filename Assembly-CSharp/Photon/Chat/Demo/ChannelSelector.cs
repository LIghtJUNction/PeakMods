// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.ChannelSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.Demo;

public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
  public string Channel;

  public void SetChannel(string channel)
  {
    this.Channel = channel;
    this.GetComponentInChildren<Text>().text = this.Channel;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    Object.FindFirstObjectByType<ChatGui>().ShowChannel(this.Channel);
  }
}
