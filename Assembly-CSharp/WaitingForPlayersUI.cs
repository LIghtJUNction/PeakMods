// Decompiled with JetBrains decompiler
// Type: WaitingForPlayersUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class WaitingForPlayersUI : MonoBehaviour
{
  public Image[] scoutImages;
  public Color notReadyColor;

  private void Update()
  {
    List<Player> allPlayers = PlayerHandler.GetAllPlayers();
    for (int index = 0; index < this.scoutImages.Length; ++index)
      this.scoutImages[index].gameObject.SetActive(false);
    int index1 = 0;
    foreach (Player player in allPlayers)
    {
      bool hasClosedEndScreen = player.hasClosedEndScreen;
      Color color = Singleton<Customization>.Instance.skins[GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player.photonView.Owner).customizationData.currentSkin].color;
      if (index1 < this.scoutImages.Length)
      {
        this.scoutImages[index1].gameObject.SetActive(true);
        this.scoutImages[index1].color = hasClosedEndScreen ? color : this.notReadyColor;
      }
      ++index1;
    }
  }
}
