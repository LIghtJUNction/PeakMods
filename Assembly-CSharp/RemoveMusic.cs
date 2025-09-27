// Decompiled with JetBrains decompiler
// Type: RemoveMusic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RemoveMusic : MonoBehaviour
{
  public GameObject[] musics;

  private void Start() => this.musics = GameObject.FindGameObjectsWithTag("Music");

  private void Update()
  {
    for (int index = 0; index < this.musics.Length; ++index)
    {
      if ((Object) this.musics[index] != (Object) null)
        this.musics[index].GetComponent<AudioSource>().volume /= 1.01f;
    }
  }
}
