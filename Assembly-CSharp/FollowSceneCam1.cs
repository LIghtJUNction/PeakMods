// Decompiled with JetBrains decompiler
// Type: FollowSceneCam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class FollowSceneCam1 : MonoBehaviour
{
  private void OnDrawGizmosSelected()
  {
    if (!((Object) Camera.current != (Object) null))
      return;
    this.transform.position = Camera.current.transform.position;
  }
}
