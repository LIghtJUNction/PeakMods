// Decompiled with JetBrains decompiler
// Type: MirrorReflectionScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MirrorReflectionScript : MonoBehaviour
{
  private MirrorCameraScript childScript;

  private void Start()
  {
    this.childScript = this.gameObject.transform.parent.gameObject.GetComponentInChildren<MirrorCameraScript>();
    if (!((Object) this.childScript == (Object) null))
      return;
    Debug.LogError((object) "Child script (MirrorCameraScript) should be in sibling object");
  }

  private void OnWillRenderObject() => this.childScript.RenderMirror();
}
