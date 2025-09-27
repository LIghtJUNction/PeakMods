// Decompiled with JetBrains decompiler
// Type: DebugVoiceTester
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DebugVoiceTester : MonoBehaviour
{
  public AudioSource audioSource;

  private void Start()
  {
    this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
    this.audioSource.loop = true;
    do
      ;
    while (Microphone.GetPosition((string) null) <= 0);
    this.audioSource.Play();
  }

  private void Update()
  {
  }
}
