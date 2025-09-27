// Decompiled with JetBrains decompiler
// Type: PlayerCustomizationDummy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PlayerCustomizationDummy : MonoBehaviour
{
  private static readonly int MainTex = Shader.PropertyToID("_MainTex");
  private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");
  public CustomizationRefs refs;

  public void UpdateDummy()
  {
    PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
    this.SetPlayerColor(playerData.customizationData.currentSkin);
    int fitIndex = CharacterCustomization.GetFitIndex(playerData);
    this.SetPlayerCostume(fitIndex);
    int index1 = playerData.customizationData.currentHat;
    if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
      index1 = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
    this.SetPlayerHat(index1);
    int eyesIndex = CharacterCustomization.GetEyesIndex(playerData);
    for (int index2 = 0; index2 < this.refs.EyeRenderers.Length; ++index2)
      this.refs.EyeRenderers[index2].material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.eyes[eyesIndex].texture);
    int accessoryIndex = CharacterCustomization.GetAccessoryIndex(playerData);
    this.refs.accessoryRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
    this.refs.accessoryRenderer.material.renderQueue = Singleton<Customization>.Instance.accessories[accessoryIndex].drawUnderEye ? 3007 : 3009;
    this.refs.mouthRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.mouths[playerData.customizationData.currentMouth].texture);
    List<Material> materials = new List<Material>();
    materials.Add(this.refs.sashRenderer.materials[0]);
    int index3 = CharacterCustomization.GetSashIndex(playerData);
    if (index3 >= this.refs.sashAscentMaterials.Length)
      index3 = this.refs.sashAscentMaterials.Length - 1;
    materials.Add(this.refs.sashAscentMaterials[index3]);
    this.refs.sashRenderer.SetMaterials(materials);
  }

  public void SetPlayerCostume(int index)
  {
    int index1 = index;
    if (index1 >= Singleton<Customization>.Instance.fits.Length)
      index1 = 0;
    this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[index1].fitMesh;
    this.refs.mainRenderer.SetSharedMaterials(new List<Material>()
    {
      this.refs.mainRenderer.materials[0],
      Singleton<Customization>.Instance.fits[index1].fitMaterial,
      Singleton<Customization>.Instance.fits[index1].fitMaterialShoes
    });
    if (Singleton<Customization>.Instance.fits[index1].noPants)
    {
      this.refs.skirt.gameObject.SetActive(false);
      this.refs.shorts.gameObject.SetActive(false);
    }
    else if (Singleton<Customization>.Instance.fits[index1].isSkirt)
    {
      this.refs.skirt.gameObject.SetActive(true);
      this.refs.shorts.gameObject.SetActive(false);
      this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[index1].fitPantsMaterial;
    }
    else
    {
      this.refs.skirt.gameObject.SetActive(false);
      this.refs.shorts.gameObject.SetActive(true);
      this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[index1].fitPantsMaterial;
    }
    this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[index1].fitHatMaterial;
    this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[index1].fitHatMaterial;
  }

  public void SetPlayerHat(int index)
  {
    for (int index1 = 0; index1 < this.refs.playerHats.Length; ++index1)
      this.refs.playerHats[index1].gameObject.SetActive(index == index1);
  }

  public void SetPlayerColor(int index)
  {
    if (index >= Singleton<Customization>.Instance.skins.Length)
      return;
    for (int index1 = 0; index1 < this.refs.PlayerRenderers.Length; ++index1)
      this.refs.PlayerRenderers[index1].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
    for (int index2 = 0; index2 < this.refs.EyeRenderers.Length; ++index2)
      this.refs.EyeRenderers[index2].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
  }
}
