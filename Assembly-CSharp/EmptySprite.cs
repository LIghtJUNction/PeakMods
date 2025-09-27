// Decompiled with JetBrains decompiler
// Type: EmptySprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class EmptySprite
{
  private static Sprite instance;

  public static Sprite Get()
  {
    if ((Object) EmptySprite.instance == (Object) null)
      EmptySprite.instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
    return EmptySprite.instance;
  }

  public static bool IsEmptySprite(Sprite s) => (Object) EmptySprite.Get() == (Object) s;
}
