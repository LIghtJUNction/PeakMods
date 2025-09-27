// Decompiled with JetBrains decompiler
// Type: VersionString
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using TMPro;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class VersionString : MonoBehaviour
{
  private TextMeshProUGUI m_text;

  private void Start() => this.m_text = this.GetComponent<TextMeshProUGUI>();

  private void Update()
  {
    BuildVersion buildVersion = new BuildVersion(Application.version);
    this.m_text.text = buildVersion.ToString();
    if (string.IsNullOrEmpty(buildVersion.BuildName))
      this.m_text.text = "v" + buildVersion.ToString();
    if (!PhotonNetwork.InRoom)
      return;
    ConnectionService service = GameHandler.GetService<ConnectionService>();
    if (service == null || !(service.StateMachine.CurrentState is InRoomState currentState) || string.IsNullOrEmpty(currentState.verifiedLobby))
      return;
    TextMeshProUGUI text = this.m_text;
    text.text = $"{text.text} - {PhotonNetwork.CloudRegion} - {currentState.verifiedLobby}";
  }
}
