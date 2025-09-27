// Decompiled with JetBrains decompiler
// Type: Pretitle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#nullable disable
public class Pretitle : MonoBehaviour
{
  public InputActionReference[] skipKeys;
  public float loadWait = 11f;
  private bool allowedToSwitch;

  private void Start()
  {
    this.StartCoroutine(this.PreloadScene());
    this.StartCoroutine(this.LoadTitle());
  }

  private IEnumerator PreloadScene()
  {
    AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
    loadSceneAsync.allowSceneActivation = false;
    while (!this.allowedToSwitch)
      yield return (object) null;
    loadSceneAsync.allowSceneActivation = true;
  }

  private IEnumerator LoadTitle()
  {
    yield return (object) new WaitForSecondsRealtime(this.loadWait);
    this.allowedToSwitch = true;
  }

  private void Update()
  {
    bool flag = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape);
    if (!flag)
    {
      foreach (InputActionReference skipKey in this.skipKeys)
      {
        if (skipKey.action.WasPressedThisFrame())
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
      return;
    this.allowedToSwitch = true;
  }
}
