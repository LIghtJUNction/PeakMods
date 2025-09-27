// Decompiled with JetBrains decompiler
// Type: CustomizationOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
[CreateAssetMenu(fileName = "CustomizationOption", menuName = "Scriptable Objects/CustomizationOption")]
public class CustomizationOption : ScriptableObject
{
  public Customization.Type type;
  public Texture texture;
  public ACHIEVEMENTTYPE requiredAchievement;
  public bool requiresAscent;
  public int requiredAscent;
  public CustomizationOption.CUSTOMREQUIREMENT customRequirement;
  public bool testLocked;
  public bool drawUnderEye;
  [ColorUsage(true, false)]
  public Color color;
  public Mesh fitMesh;
  public Material fitMaterial;
  public Material fitMaterialShoes;
  public Material fitMaterialOverridePants;
  public Material fitMaterialOverrideHat;
  public bool isSkirt;
  public bool noPants;
  public bool overrideHat;
  public int overrideHatIndex;

  private bool IsAccessory => this.type == Customization.Type.Accessory;

  private bool IsSkin => this.type == Customization.Type.Skin;

  private bool IsFit => this.type == Customization.Type.Fit;

  public Material fitPantsMaterial
  {
    get
    {
      return (Object) this.fitMaterialOverridePants != (Object) null ? this.fitMaterialOverridePants : this.fitMaterial;
    }
  }

  public Material fitHatMaterial
  {
    get
    {
      return (Object) this.fitMaterialOverrideHat != (Object) null ? this.fitMaterialOverrideHat : this.fitMaterial;
    }
  }

  public bool IsLocked
  {
    get
    {
      if (this.requiresAscent)
        return Singleton<AchievementManager>.Instance.GetMaxAscent() < this.requiredAscent;
      if (this.requiredAchievement == ACHIEVEMENTTYPE.NONE && this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.None)
        return false;
      if (this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Goat)
        return Singleton<AchievementManager>.Instance.GetMaxAscent() < 8;
      return this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Crown ? !Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked() : !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.requiredAchievement);
    }
  }

  public enum CUSTOMREQUIREMENT
  {
    None,
    Goat,
    Crown,
  }
}
