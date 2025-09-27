// Decompiled with JetBrains decompiler
// Type: DebugItemSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;

#nullable disable
public class DebugItemSpawner : MonoBehaviour
{
  private IEnumerator Start()
  {
    DebugItemSpawner debugItemSpawner = this;
    ISpawner spawner = debugItemSpawner.GetComponent<ISpawner>();
    if (spawner == null || !PhotonNetwork.IsMasterClient)
    {
      Object.Destroy((Object) debugItemSpawner);
    }
    else
    {
      while (!PhotonNetwork.InRoom || !(bool) (Object) Character.localCharacter)
        yield return (object) null;
      spawner.TrySpawnItems();
    }
  }
}
