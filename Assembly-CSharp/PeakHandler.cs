// Decompiled with JetBrains decompiler
// Type: PeakHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PeakHandler : Singleton<PeakHandler>
{
  public bool summonedHelicopter;
  public GameObject peakSequence;
  public GameObject endCutscene;
  public Animator endCutsceneAnimator;
  public float secondsUntilEndscreen = 13f;
  public CustomizationRefs firstCutsceneScout;
  public CustomizationRefs[] cutsceneScoutRefs;
  public EndCutsceneScoutHelper[] cutsceneScoutAnims;
  private List<AnimatedMouth> localMouths = new List<AnimatedMouth>();
  public bool isPlayingCinematic;
  private bool endScreenComplete;

  public void SummonHelicopter()
  {
    this.peakSequence.SetActive(true);
    this.summonedHelicopter = true;
  }

  public void EndCutscene()
  {
    this.isPlayingCinematic = true;
    List<Character> allCharacters = Character.AllCharacters;
    foreach (Character character in allCharacters)
      character.refs.animator.gameObject.SetActive(false);
    MainCamera.instance.gameObject.SetActive(false);
    MenuWindow.CloseAllWindows();
    this.peakSequence.SetActive(false);
    GUIManager.instance.letterboxCanvas.gameObject.SetActive(true);
    GUIManager.instance.hudCanvas.enabled = false;
    this.endCutscene.SetActive(true);
    this.SetCosmetics(allCharacters);
    this.StartCoroutine(OpenEndscreen());

    IEnumerator OpenEndscreen()
    {
      yield return (object) new WaitForSeconds(this.secondsUntilEndscreen);
      GUIManager.instance.endScreen.Open();
      while (!this.endScreenComplete)
        yield return (object) null;
      this.endCutsceneAnimator.SetBool("Next", true);
      GUIManager.instance.endScreen.Close();
    }
  }

  private void SetCosmetics(List<Character> characters)
  {
    Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetLocalMic));
    characters = characters.Where<Character>((Func<Character, bool>) (character => character.refs.stats.won)).ToList<Character>();
    characters.Sort((Comparison<Character>) ((c1, c2) => c1.photonView.ViewID.CompareTo(c2.photonView.ViewID)));
    characters[0].refs.customization.SetCustomizationForRef(this.firstCutsceneScout);
    this.firstCutsceneScout.GetComponent<AnimatedMouth>().audioSource = characters[0].GetComponent<AnimatedMouth>().audioSource;
    this.localMouths.Add(this.firstCutsceneScout.GetComponent<AnimatedMouth>());
    int index1 = 0;
    for (int index2 = 0; index2 < 4; ++index2)
    {
      if (index2 >= characters.Count)
      {
        this.cutsceneScoutRefs[index2].gameObject.SetActive(false);
      }
      else
      {
        characters[index2].refs.customization.SetCustomizationForRef(this.cutsceneScoutRefs[index1]);
        BadgeUnlocker.SetBadges(characters[index2], this.cutsceneScoutRefs[index1].sashRenderer);
        this.cutsceneScoutRefs[index1].GetComponent<AnimatedMouth>().audioSource = characters[index2].GetComponent<AnimatedMouth>().audioSource;
        if (characters[index2].IsLocal)
          this.localMouths.Add(this.cutsceneScoutRefs[index1].GetComponent<AnimatedMouth>());
        ++index1;
      }
    }
    if (characters.Count <= 1)
      this.cutsceneScoutAnims[0].alone = true;
    if (characters.Count > 2)
      return;
    this.cutsceneScoutAnims[1].alone = true;
  }

  private void OnGetLocalMic(float[] buffer)
  {
    foreach (AnimatedMouth localMouth in this.localMouths)
      localMouth.OnGetMic(buffer);
  }

  public void EndScreenComplete()
  {
    Singleton<GameOverHandler>.Instance.ForceEveryPlayerDoneWithEndScreen();
    this.endScreenComplete = true;
    this.StartCoroutine(CreditsLogic());

    static IEnumerator CreditsLogic()
    {
      yield return (object) new WaitForSecondsRealtime(20f);
      UnityEngine.InputSystem.InputAction anyKeyAction = UnityEngine.InputSystem.InputSystem.actions.FindAction("AnyKey", false);
      bool skipped = false;
      float creditsLength = 60f;
      float t = 0.0f;
      while ((double) t < (double) creditsLength && !skipped)
      {
        Debug.Log((object) "Waiting for input....");
        if (anyKeyAction != null && anyKeyAction.WasPerformedThisFrame())
          skipped = true;
        t += Time.unscaledDeltaTime;
        yield return (object) null;
      }
      Debug.Log((object) "Local player is done with credits!");
      Singleton<GameOverHandler>.Instance.LoadAirport();
    }
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    if (!this.isPlayingCinematic || !(bool) (UnityEngine.Object) Singleton<MicrophoneRelay>.Instance)
      return;
    Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetLocalMic));
  }
}
