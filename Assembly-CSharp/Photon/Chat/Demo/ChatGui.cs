// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.ChatGui
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.Demo;

public class ChatGui : MonoBehaviour, IChatClientListener
{
  public string[] ChannelsToJoinOnConnect;
  public string[] FriendsList;
  public int HistoryLengthToFetch;
  private string selectedChannelName;
  public ChatClient chatClient;
  protected internal ChatAppSettings chatAppSettings;
  public GameObject missingAppIdErrorPanel;
  public GameObject ConnectingLabel;
  public RectTransform ChatPanel;
  public GameObject UserIdFormPanel;
  public InputField InputFieldChat;
  public Text CurrentChannelText;
  public Toggle ChannelToggleToInstantiate;
  public GameObject FriendListUiItemtoInstantiate;
  private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();
  private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();
  public bool ShowState = true;
  public GameObject Title;
  public Text StateText;
  public Text UserIdText;
  private static string HelpText = "\n    -- HELP --\nTo subscribe to channel(s) (channelnames are case sensitive) :  \n\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n\nTo leave channel(s):\n\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n\nTo switch the active channel\n\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n\tor\n\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n\nTo send a private message: (username are case sensitive)\n\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n\nTo change status:\n\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n<color=green>0</color> = Offline <color=green>1</color> = Invisible <color=green>2</color> = Online <color=green>3</color> = Away \n<color=green>4</color> = Do not disturb <color=green>5</color> = Looking For Group <color=green>6</color> = Playing\n\nTo clear the current chat tab (private chats get closed):\n\t<color=#E07B00>\\clear</color>";
  public int TestLength = 2048 /*0x0800*/;
  private byte[] testBytes = new byte[2048 /*0x0800*/];

  public string UserName { get; set; }

  public void Start()
  {
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
    this.UserIdText.text = "";
    this.StateText.text = "";
    this.StateText.gameObject.SetActive(true);
    this.UserIdText.gameObject.SetActive(true);
    this.Title.SetActive(true);
    this.ChatPanel.gameObject.SetActive(false);
    this.ConnectingLabel.SetActive(false);
    if (string.IsNullOrEmpty(this.UserName))
      this.UserName = "user" + (Environment.TickCount % 99).ToString();
    this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
    bool flag = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
    this.missingAppIdErrorPanel.SetActive(!flag);
    this.UserIdFormPanel.gameObject.SetActive(flag);
    if (flag)
      return;
    Debug.LogError((object) "You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
  }

  public void Connect()
  {
    this.UserIdFormPanel.gameObject.SetActive(false);
    this.chatClient = new ChatClient((IChatClientListener) this);
    this.chatClient.UseBackgroundWorkerForSending = true;
    this.chatClient.AuthValues = new Photon.Chat.AuthenticationValues(this.UserName);
    this.chatClient.ConnectUsingSettings(this.chatAppSettings);
    this.ChannelToggleToInstantiate.gameObject.SetActive(false);
    Debug.Log((object) ("Connecting as: " + this.UserName));
    this.ConnectingLabel.SetActive(true);
  }

  public void OnDestroy()
  {
    if (this.chatClient == null)
      return;
    this.chatClient.Disconnect();
  }

  public void OnApplicationQuit()
  {
    if (this.chatClient == null)
      return;
    this.chatClient.Disconnect();
  }

  public void Update()
  {
    if (this.chatClient != null)
      this.chatClient.Service();
    if ((UnityEngine.Object) this.StateText == (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    else
      this.StateText.gameObject.SetActive(this.ShowState);
  }

  public void OnEnterSend()
  {
    if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
      return;
    this.SendChatMessage(this.InputFieldChat.text);
    this.InputFieldChat.text = "";
  }

  public void OnClickSend()
  {
    if (!((UnityEngine.Object) this.InputFieldChat != (UnityEngine.Object) null))
      return;
    this.SendChatMessage(this.InputFieldChat.text);
    this.InputFieldChat.text = "";
  }

  private void SendChatMessage(string inputLine)
  {
    if (string.IsNullOrEmpty(inputLine))
      return;
    if ("test".Equals(inputLine))
    {
      if (this.TestLength != this.testBytes.Length)
        this.testBytes = new byte[this.TestLength];
      this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, (object) this.testBytes, true);
    }
    bool isPrivate = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
    string empty = string.Empty;
    if (isPrivate)
      empty = this.selectedChannelName.Split(':')[1];
    if (inputLine[0].Equals('\\'))
    {
      string[] strArray1 = inputLine.Split(new char[1]
      {
        ' '
      }, 2);
      if (strArray1[0].Equals("\\help"))
        this.PostHelpToCurrentChannel();
      if (strArray1[0].Equals("\\state"))
      {
        int status = 0;
        List<string> stringList = new List<string>();
        stringList.Add("i am state " + status.ToString());
        string[] strArray2 = strArray1[1].Split(' ', ',');
        if (strArray2.Length != 0)
          status = int.Parse(strArray2[0]);
        if (strArray2.Length > 1)
          stringList.Add(strArray2[1]);
        this.chatClient.SetOnlineStatus(status, (object) stringList.ToArray());
      }
      else if ((strArray1[0].Equals("\\subscribe") || strArray1[0].Equals("\\s")) && !string.IsNullOrEmpty(strArray1[1]))
        this.chatClient.Subscribe(strArray1[1].Split(' ', ','));
      else if ((strArray1[0].Equals("\\unsubscribe") || strArray1[0].Equals("\\u")) && !string.IsNullOrEmpty(strArray1[1]))
        this.chatClient.Unsubscribe(strArray1[1].Split(' ', ','));
      else if (strArray1[0].Equals("\\clear"))
      {
        if (isPrivate)
        {
          this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
        }
        else
        {
          ChatChannel channel;
          if (!this.chatClient.TryGetChannel(this.selectedChannelName, isPrivate, out channel))
            return;
          channel.ClearMessages();
        }
      }
      else if (strArray1[0].Equals("\\msg") && !string.IsNullOrEmpty(strArray1[1]))
      {
        string[] strArray3 = strArray1[1].Split(new char[2]
        {
          ' ',
          ','
        }, 2);
        if (strArray3.Length < 2)
          return;
        this.chatClient.SendPrivateMessage(strArray3[0], (object) strArray3[1]);
      }
      else if ((strArray1[0].Equals("\\join") || strArray1[0].Equals("\\j")) && !string.IsNullOrEmpty(strArray1[1]))
      {
        string[] strArray4 = strArray1[1].Split(new char[2]
        {
          ' ',
          ','
        }, 2);
        if (this.channelToggles.ContainsKey(strArray4[0]))
          this.ShowChannel(strArray4[0]);
        else
          this.chatClient.Subscribe(new string[1]
          {
            strArray4[0]
          });
      }
      else
        Debug.Log((object) $"The command '{strArray1[0]}' is invalid.");
    }
    else if (isPrivate)
      this.chatClient.SendPrivateMessage(empty, (object) inputLine);
    else
      this.chatClient.PublishMessage(this.selectedChannelName, (object) inputLine);
  }

  public void PostHelpToCurrentChannel() => this.CurrentChannelText.text += ChatGui.HelpText;

  public void DebugReturn(DebugLevel level, string message)
  {
    if (level == DebugLevel.ERROR)
      Debug.LogError((object) message);
    else if (level == DebugLevel.WARNING)
      Debug.LogWarning((object) message);
    else
      Debug.Log((object) message);
  }

  public void OnConnected()
  {
    if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length != 0)
      this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
    this.ConnectingLabel.SetActive(false);
    this.UserIdText.text = "Connected as " + this.UserName;
    this.ChatPanel.gameObject.SetActive(true);
    if (this.FriendsList != null && this.FriendsList.Length != 0)
    {
      this.chatClient.AddFriends(this.FriendsList);
      foreach (string friends in this.FriendsList)
      {
        if ((UnityEngine.Object) this.FriendListUiItemtoInstantiate != (UnityEngine.Object) null && friends != this.UserName)
          this.InstantiateFriendButton(friends);
      }
    }
    if ((UnityEngine.Object) this.FriendListUiItemtoInstantiate != (UnityEngine.Object) null)
      this.FriendListUiItemtoInstantiate.SetActive(false);
    this.chatClient.SetOnlineStatus(2);
  }

  public void OnDisconnected()
  {
    Debug.Log((object) "OnDisconnected()");
    this.ConnectingLabel.SetActive(false);
  }

  public void OnChatStateChange(ChatState state) => this.StateText.text = state.ToString();

  public void OnSubscribed(string[] channels, bool[] results)
  {
    foreach (string channel in channels)
    {
      this.chatClient.PublishMessage(channel, (object) "says 'hi'.");
      if ((UnityEngine.Object) this.ChannelToggleToInstantiate != (UnityEngine.Object) null)
        this.InstantiateChannelButton(channel);
    }
    Debug.Log((object) ("OnSubscribed: " + string.Join(", ", channels)));
    this.ShowChannel(channels[0]);
  }

  public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
  {
    Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", (object) channel, (object) users.Length, (object) properties.ToStringFull());
  }

  private void InstantiateChannelButton(string channelName)
  {
    if (this.channelToggles.ContainsKey(channelName))
    {
      Debug.Log((object) "Skipping creation for an existing channel toggle.");
    }
    else
    {
      Toggle toggle = UnityEngine.Object.Instantiate<Toggle>(this.ChannelToggleToInstantiate);
      toggle.gameObject.SetActive(true);
      toggle.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
      toggle.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);
      this.channelToggles.Add(channelName, toggle);
    }
  }

  private void InstantiateFriendButton(string friendId)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.FriendListUiItemtoInstantiate);
    gameObject.gameObject.SetActive(true);
    FriendItem component = gameObject.GetComponent<FriendItem>();
    component.FriendId = friendId;
    gameObject.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);
    this.friendListItemLUT[friendId] = component;
  }

  public void OnUnsubscribed(string[] channels)
  {
    foreach (string channel in channels)
    {
      if (this.channelToggles.ContainsKey(channel))
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.channelToggles[channel].gameObject);
        this.channelToggles.Remove(channel);
        Debug.Log((object) $"Unsubscribed from channel '{channel}'.");
        if (channel == this.selectedChannelName && this.channelToggles.Count > 0)
        {
          IEnumerator<KeyValuePair<string, Toggle>> enumerator = (IEnumerator<KeyValuePair<string, Toggle>>) this.channelToggles.GetEnumerator();
          enumerator.MoveNext();
          this.ShowChannel(enumerator.Current.Key);
          enumerator.Current.Value.isOn = true;
        }
      }
      else
        Debug.Log((object) $"Can't unsubscribe from channel '{channel}' because you are currently not subscribed to it.");
    }
  }

  public void OnGetMessages(string channelName, string[] senders, object[] messages)
  {
    if (!channelName.Equals(this.selectedChannelName))
      return;
    this.ShowChannel(this.selectedChannelName);
  }

  public void OnPrivateMessage(string sender, object message, string channelName)
  {
    this.InstantiateChannelButton(channelName);
    if (message is byte[] numArray)
      Debug.Log((object) ("Message with byte[].Length: " + numArray.Length.ToString()));
    if (!this.selectedChannelName.Equals(channelName))
      return;
    this.ShowChannel(channelName);
  }

  public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
  {
    Debug.LogWarning((object) ("status: " + $"{user} is {status}. Msg:{message}"));
    if (!this.friendListItemLUT.ContainsKey(user))
      return;
    FriendItem friendItem = this.friendListItemLUT[user];
    if (!((UnityEngine.Object) friendItem != (UnityEngine.Object) null))
      return;
    friendItem.OnFriendStatusUpdate(status, gotMessage, message);
  }

  public void OnUserSubscribed(string channel, string user)
  {
    Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", (object) channel, (object) user);
  }

  public void OnUserUnsubscribed(string channel, string user)
  {
    Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", (object) channel, (object) user);
  }

  public void OnChannelPropertiesChanged(
    string channel,
    string userId,
    Dictionary<object, object> properties)
  {
    Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", (object) channel, (object) userId, (object) properties.ToStringFull());
  }

  public void OnUserPropertiesChanged(
    string channel,
    string targetUserId,
    string senderUserId,
    Dictionary<object, object> properties)
  {
    Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", (object) channel, (object) targetUserId, (object) senderUserId, (object) properties.ToStringFull());
  }

  public void OnErrorInfo(string channel, string error, object data)
  {
    Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", (object) channel, (object) error, data);
  }

  public void AddMessageToSelectedChannel(string msg)
  {
    ChatChannel channel = (ChatChannel) null;
    if (!this.chatClient.TryGetChannel(this.selectedChannelName, out channel))
      Debug.Log((object) ("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName));
    else
      channel?.Add("Bot", (object) msg, 0);
  }

  public void ShowChannel(string channelName)
  {
    if (string.IsNullOrEmpty(channelName))
      return;
    ChatChannel channel = (ChatChannel) null;
    if (!this.chatClient.TryGetChannel(channelName, out channel))
    {
      Debug.Log((object) ("ShowChannel failed to find channel: " + channelName));
    }
    else
    {
      this.selectedChannelName = channelName;
      this.CurrentChannelText.text = channel.ToStringMessages();
      Debug.Log((object) ("ShowChannel: " + this.selectedChannelName));
      foreach (KeyValuePair<string, Toggle> channelToggle in this.channelToggles)
        channelToggle.Value.isOn = channelToggle.Key == channelName;
    }
  }

  public void OpenDashboard() => Application.OpenURL("https://dashboard.photonengine.com");
}
