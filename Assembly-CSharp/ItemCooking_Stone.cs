// Decompiled with JetBrains decompiler
// Type: ItemCooking_Stone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ItemCooking_Stone : ItemCooking
{
  public Color heatColor;

  protected override void CookVisually(int cookedAmount)
  {
    for (int index = 0; index < cookedAmount; ++index)
    {
      foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
        componentsInChild.material.SetColor("_Tint", (Color) Vector4.MoveTowards((Vector4) componentsInChild.material.GetColor("_Tint"), (Vector4) this.heatColor, 0.15f));
    }
  }
}
