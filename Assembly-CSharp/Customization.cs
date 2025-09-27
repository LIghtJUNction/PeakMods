// Decompiled with JetBrains decompiler
// Type: Customization
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Zorro.Core;

#nullable disable
public class Customization : Singleton<Customization>
{
  public CustomizationOption[] skins;
  public CustomizationOption[] accessories;
  public CustomizationOption[] eyes;
  public CustomizationOption[] mouths;
  public CustomizationOption[] fits;
  public CustomizationOption[] hats;
  public CustomizationOption[] sashes;
  public CustomizationOption goatHat;
  public CustomizationOption crownHat;

  public bool TryGetUnlockedCosmetic(BadgeData badge, out CustomizationOption cosmetic)
  {
    cosmetic = (CustomizationOption) null;
    foreach (Customization.Type type in Enum.GetValues(typeof (Customization.Type)))
    {
      foreach (CustomizationOption customizationOption in this.GetList(type))
      {
        if (!((UnityEngine.Object) customizationOption == (UnityEngine.Object) null) && customizationOption.requiredAchievement != ACHIEVEMENTTYPE.NONE && customizationOption.requiredAchievement == badge.linkedAchievement)
        {
          cosmetic = customizationOption;
          return true;
        }
      }
    }
    return false;
  }

  public CustomizationOption[] GetList(Customization.Type type)
  {
    switch (type)
    {
      case Customization.Type.Skin:
        return this.skins;
      case Customization.Type.Accessory:
        return this.accessories;
      case Customization.Type.Eyes:
        return this.eyes;
      case Customization.Type.Mouth:
        return this.mouths;
      case Customization.Type.Fit:
        return this.fits;
      case Customization.Type.Hat:
        return this.hats;
      case Customization.Type.Sash:
        return this.sashes;
      default:
        return this.skins;
    }
  }

  public int GetRandomUnlockedIndex(Customization.Type type)
  {
    CustomizationOption[] list = this.GetList(type);
    List<int> intList = new List<int>();
    for (int index = 0; index < list.Length; ++index)
    {
      if (!list[index].IsLocked)
        intList.Add(index);
    }
    return intList.Count <= 0 ? 0 : intList[UnityEngine.Random.Range(0, intList.Count)];
  }

  public enum Type
  {
    Skin = 0,
    Accessory = 10, // 0x0000000A
    Eyes = 20, // 0x00000014
    Mouth = 30, // 0x0000001E
    Fit = 40, // 0x00000028
    Hat = 50, // 0x00000032
    Sash = 60, // 0x0000003C
  }
}
