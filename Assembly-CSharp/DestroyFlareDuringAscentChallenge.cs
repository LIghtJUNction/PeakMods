// Decompiled with JetBrains decompiler
// Type: DestroyFlareDuringAscentChallenge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;

#nullable disable
public class DestroyFlareDuringAscentChallenge : MonoBehaviourPun
{
  private IEnumerator Start()
  {
    DestroyFlareDuringAscentChallenge duringAscentChallenge = this;
    while (!PhotonNetwork.InRoom)
      yield return (object) null;
    if (!Ascents.shouldSpawnFlare)
      PhotonNetwork.Destroy(duringAscentChallenge.gameObject);
  }
}
