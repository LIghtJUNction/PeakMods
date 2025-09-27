// Decompiled with JetBrains decompiler
// Type: Rotate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Rotate : MonoBehaviour
{
  public Transform tf;
  public Vector3 rotation;

  private void Update() => this.tf.transform.Rotate(this.rotation * Time.deltaTime);
}
