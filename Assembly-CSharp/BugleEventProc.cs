// Decompiled with JetBrains decompiler
// Type: BugleEventProc
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BugleEventProc : MonoBehaviour
{
  private Item item;

  private void Awake()
  {
    this.item = this.GetComponent<Item>();
    this.item.OnPrimaryStarted += new Action(this.ThrowBugleEvent);
  }

  private void OnDestroy() => this.item.OnPrimaryStarted -= new Action(this.ThrowBugleEvent);

  private void ThrowBugleEvent() => GlobalEvents.TriggerBugleTooted(this.item);
}
