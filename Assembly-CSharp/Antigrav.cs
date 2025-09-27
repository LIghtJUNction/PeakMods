// Decompiled with JetBrains decompiler
// Type: Antigrav
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Item))]
public class Antigrav : MonoBehaviour
{
  private Item item;
  public float intensity = 1f;

  private void Start() => this.item = this.GetComponent<Item>();

  private void FixedUpdate()
  {
    if (this.item.itemState != ItemState.Ground)
      return;
    this.item.rig.AddForce(-Physics.gravity * this.intensity, ForceMode.Acceleration);
  }
}
