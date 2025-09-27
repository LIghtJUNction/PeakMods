// Decompiled with JetBrains decompiler
// Type: Transition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public abstract class Transition : MonoBehaviour
{
  public TransitionType transitionType;

  public abstract IEnumerator TransitionIn(float speed = 1f);

  public abstract IEnumerator TransitionOut(float speed = 1f);
}
