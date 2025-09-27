// Decompiled with JetBrains decompiler
// Type: DestroyAfterTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DestroyAfterTime : MonoBehaviour
{
  public float time = 3f;

  private void Start() => Object.Destroy((Object) this.gameObject, this.time);
}
