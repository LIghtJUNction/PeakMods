// Decompiled with JetBrains decompiler
// Type: LoadingScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

#nullable disable
public class LoadingScreen : MonoBehaviour
{
  public AnimationCurve FadeOutAudioCurve;
  public AnimationCurve FadeInAudioCurve;
  public AudioMixer Mixer;
  public CanvasGroup group;
  public Canvas canvas;
  private Animator anim;
  public float loadStartYieldTime = 1.5f;
  protected IEnumerator currentProcess;
  private bool runningProcess;

  private void Awake()
  {
    this.canvas.enabled = false;
    this.anim = this.GetComponent<Animator>();
    this.transform.SetParent((Transform) null, true);
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
  }

  public virtual IEnumerator LoadingRoutine(Action runAfter, IEnumerator[] processList)
  {
    LoadingScreen loadingScreen = this;
    PhotonNetwork.IsMessageQueueRunning = false;
    loadingScreen.canvas.enabled = true;
    loadingScreen.group.blocksRaycasts = true;
    float num = 0.0f;
    if (loadingScreen.FadeOutAudioCurve != null && loadingScreen.FadeOutAudioCurve.keys.Length != 0)
      num = loadingScreen.FadeOutAudioCurve.GetEndTime();
    float extraLoadTime = loadingScreen.loadStartYieldTime - num;
    if (loadingScreen.FadeOutAudioCurve != null && loadingScreen.FadeOutAudioCurve.keys.Length != 0)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) loadingScreen.FadeOutAudioCurve.YieldForCurve(new Action<float>(loadingScreen.\u003CLoadingRoutine\u003Eb__10_0));
    }
    if ((double) extraLoadTime > 0.0)
      yield return (object) new WaitForSecondsRealtime(extraLoadTime);
    for (int processIndex = 0; processIndex < processList.Length; ++processIndex)
    {
      loadingScreen.currentProcess = processList[processIndex];
      loadingScreen.StartCoroutine(loadingScreen.RunProcess(loadingScreen.currentProcess));
      while (loadingScreen.runningProcess)
        yield return (object) null;
    }
    if (!PhotonNetwork.IsMessageQueueRunning)
    {
      PhotonNetwork.IsMessageQueueRunning = true;
      Debug.Log((object) "Restarting message queue");
    }
    Action action = runAfter;
    if (action != null)
      action();
    loadingScreen.anim.SetTrigger("Finish");
    loadingScreen.group.blocksRaycasts = false;
    Debug.Log((object) "Loading finished.");
    if (loadingScreen.FadeInAudioCurve != null && loadingScreen.FadeInAudioCurve.keys.Length != 0)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) loadingScreen.FadeInAudioCurve.YieldForCurve(new Action<float>(loadingScreen.\u003CLoadingRoutine\u003Eb__10_1));
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) loadingScreen.gameObject, 6f);
  }

  private IEnumerator RunProcess(IEnumerator process)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LoadingScreen loadingScreen = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      loadingScreen.runningProcess = false;
      Debug.Log((object) "Process Finished: process");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Debug.Log((object) "Process Started: process");
    loadingScreen.runningProcess = true;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) loadingScreen.StartCoroutine(process);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public enum LoadingScreenType
  {
    Basic,
    Plane,
  }
}
