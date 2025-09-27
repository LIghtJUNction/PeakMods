// Decompiled with JetBrains decompiler
// Type: RotateTransformOnPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Item))]
public class RotateTransformOnPickup : MonoBehaviour
{
  public Vector3 rotation;
  public Transform transformToRotate;
  public Item item;

  private void Start()
  {
    if (this.item.itemState != ItemState.Held)
      return;
    this.transformToRotate.localEulerAngles += this.rotation;
  }
}
