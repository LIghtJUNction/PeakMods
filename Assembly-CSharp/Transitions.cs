// Decompiled with JetBrains decompiler
// Type: Transitions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
public class Transitions : MonoBehaviour
{
  private Transition[] transitions;
  public static Transitions instance;

  private void Awake()
  {
    Transitions.instance = this;
    this.transitions = this.GetComponentsInChildren<Transition>(true);
  }

  public void PlayTransition(
    TransitionType transitionType,
    Action action,
    float transitionInSpeed = 1f,
    float transitionOutSpeed = 1f)
  {
    Transition transition = this.GetTransition(transitionType);
    this.StartCoroutine(IPlayTransition());

    IEnumerator IPlayTransition()
    {
      transition.gameObject.SetActive(true);
      yield return (object) transition.TransitionIn(transitionInSpeed);
      Action action = action;
      if (action != null)
        action();
      yield return (object) transition.TransitionOut(transitionOutSpeed);
      transition.gameObject.SetActive(false);
    }
  }

  private Transition GetTransition(TransitionType transitionType)
  {
    for (int index = 0; index < this.transitions.Length; ++index)
    {
      if (this.transitions[index].transitionType == transitionType)
        return this.transitions[index];
    }
    return (Transition) null;
  }
}
