// Decompiled with JetBrains decompiler
// Type: AvaragePosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AvaragePosition : MonoBehaviour
{
  public Transform p1;
  public Transform p2;

  private void Update() => this.transform.position = (this.p1.position + this.p2.position) / 2f;
}
