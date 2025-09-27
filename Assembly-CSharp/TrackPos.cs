// Decompiled with JetBrains decompiler
// Type: TrackPos
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TrackPos : MonoBehaviour
{
  public Transform trackTransform;
  private Vector3 startPos;
  private Quaternion startRot;
  public bool trackPos;
  public bool trackRot;

  private void Start()
  {
    this.startPos = this.transform.position;
    this.startRot = this.transform.rotation;
  }

  private void Update()
  {
    if (this.trackPos)
      this.transform.position = this.trackTransform.position + this.startPos;
    if (!this.trackRot)
      return;
    this.transform.rotation = this.trackTransform.rotation * this.startRot;
  }
}
