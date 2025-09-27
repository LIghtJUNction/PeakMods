// Decompiled with JetBrains decompiler
// Type: setright
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class setright : MonoBehaviour
{
  public Vector3 right;
  public Vector3 up;

  private void Start()
  {
  }

  private void Update()
  {
  }

  public void go()
  {
    this.transform.right = this.right;
    this.transform.up = this.up;
  }
}
