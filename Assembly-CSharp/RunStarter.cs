// Decompiled with JetBrains decompiler
// Type: RunStarter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;

#nullable disable
public class RunStarter : MonoBehaviour
{
  private IEnumerator Start()
  {
    while (!PhotonNetwork.InRoom || !(bool) (Object) Character.localCharacter || LoadingScreenHandler.loading)
      yield return (object) null;
    Debug.Log((object) "RUN STARTED");
    this.StartRun();
  }

  private void StartRun() => RunManager.Instance.StartRun();
}
