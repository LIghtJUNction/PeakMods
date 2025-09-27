// Decompiled with JetBrains decompiler
// Type: AudioLevels
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class AudioLevels : MonoBehaviour
{
  public static Dictionary<int, float> PlayerAudioLevels = new Dictionary<int, float>();
  public List<AudioLevelSlider> sliders;
  [SerializeField]
  private PauseMenuMainPage mainPage;

  public static void ResetSliders()
  {
    AudioLevels.PlayerAudioLevels.Clear();
    GlobalEvents.TriggerCharacterAudioLevelsUpdated();
  }

  public static float GetPlayerLevel(int playerID)
  {
    return !AudioLevels.PlayerAudioLevels.ContainsKey(playerID) ? 0.5f : AudioLevels.PlayerAudioLevels[playerID];
  }

  public static void SetPlayerLevel(int playerID, float f)
  {
    if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
      AudioLevels.PlayerAudioLevels.Add(playerID, 1f);
    AudioLevels.PlayerAudioLevels[playerID] = f;
    GlobalEvents.TriggerCharacterAudioLevelsUpdated();
  }

  public void OnEnable() => this.UpdateSliders();

  public void OnDisable()
  {
  }

  public void OnPlayerListChanged(Photon.Realtime.Player newPlayer) => this.UpdateSliders();

  public void UpdateSliders()
  {
    Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
    int index1 = 0;
    Debug.Log((object) $"There are {playerList.Length.ToString()} Players.");
    for (int index2 = 0; index2 < playerList.Length; ++index2)
    {
      if (this.sliders.Count > index2)
        this.sliders[index2].Init(playerList[index2]);
      index1 = index2 + 1;
    }
    for (; index1 < this.sliders.Count; ++index1)
      this.sliders[index1].Init((Photon.Realtime.Player) null);
    this.InitNavigation();
  }

  private void InitNavigation()
  {
    if (!(bool) (Object) this.mainPage)
      return;
    for (int index = 0; index < this.sliders.Count; ++index)
    {
      Slider slider1 = this.sliders[index].slider;
      bool flag = index == 0 || !this.sliders[index - 1].gameObject.activeInHierarchy;
      int num = index == this.sliders.Count - 1 ? 1 : (!this.sliders[index + 1].gameObject.activeInHierarchy ? 1 : 0);
      Selectable slider2 = flag ? (Selectable) null : (Selectable) this.sliders[index - 1].slider;
      Selectable next = num != 0 ? (Selectable) this.mainPage.resumeButton : (Selectable) this.sliders[index + 1].slider;
      this.SetSliderSelection((Selectable) slider1, slider2, next);
    }
  }

  private void SetSliderSelection(Selectable obj, Selectable prev, Selectable next)
  {
    obj.navigation = new Navigation()
    {
      mode = Navigation.Mode.Explicit,
      selectOnUp = prev,
      selectOnDown = next,
      selectOnLeft = (Selectable) null,
      selectOnRight = (Selectable) null
    };
  }
}
