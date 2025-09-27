// Decompiled with JetBrains decompiler
// Type: CharacterCustomization
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[ConsoleClassCustomizer("Customization")]
public class CharacterCustomization : MonoBehaviour
{
  private Character _character;
  public CustomizationRefs refs;
  private static readonly int MainTex = Shader.PropertyToID("_MainTex");
  private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");
  private static readonly int Idle = Animator.StringToHash(nameof (Idle));
  private static readonly int Spin = Shader.PropertyToID("_Spin");
  private static readonly int VertexGhost = Shader.PropertyToID("_VertexGhost");
  private static readonly int StatusColor = Shader.PropertyToID("_StatusColor");
  private static readonly int StatusGlow = Shader.PropertyToID("_StatusGlow");
  private static readonly int Opacity = Shader.PropertyToID("_Opacity");
  public bool useDebugColor;
  public int debugColorIndex;
  public int maxIdles;
  public Animator PlayerAnimator;
  private PhotonView view;
  public ParticleSystem chickenParticle;
  public Texture passedOutEyes;
  private Texture CurrentEyeTexture;
  [FormerlySerializedAs("diedTexture")]
  public Texture deadEyes;
  [FormerlySerializedAs("isDead")]
  public bool isPassedOut;
  public bool isDead;
  public bool isCannibalizable;
  private Tweener chickenTweener;

  public Color PlayerColor
  {
    get
    {
      return Singleton<Customization>.Instance.skins[CharacterCustomization.GetCustomizationData(this._character.photonView.Owner).currentSkin].color;
    }
  }

  private void Awake()
  {
    this.view = this.GetComponent<PhotonView>();
    this._character = this.GetComponent<Character>();
  }

  public void Start()
  {
    if (this.view.IsMine && !this._character.isBot)
    {
      this.SetRandomIdle();
      this.HideChicken();
      if (GameHandler.GetService<ConnectionService>().StateMachine.CurrentState is InRoomState currentState && !currentState.hasLoadedCustomization)
      {
        currentState.hasLoadedCustomization = true;
        this.StartCoroutine(this.GetCosmeticsFromSteamRoutine());
        if (this._character.IsLocal)
          this.refs.mainRenderer.updateWhenOffscreen = true;
      }
    }
    this._character.reviveAction += new Action(this.OnRevive);
    this._character.UnPassOutAction += new Action(this.OnRevive);
    PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
    service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
    this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
  }

  private IEnumerator GetCosmeticsFromSteamRoutine()
  {
    while (!Singleton<AchievementManager>.Instance.gotStats)
      yield return (object) null;
    this.TryGetCosmeticsFromSteam();
  }

  private void TryGetCosmeticsFromSteam()
  {
    int num;
    if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.LoadedCosmeticsPreviously, out num))
    {
      if (num > 0)
      {
        int index1;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Skin, out index1) && index1 != -1)
          CharacterCustomization.SetCharacterSkinColor(index1);
        else
          this.SetRandomSkinColor();
        int index2;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Eyes, out index2) && index2 != -1)
          CharacterCustomization.SetCharacterEyes(index2);
        else
          this.SetRandomEyes();
        int index3;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Mouth, out index3) && index3 != -1)
          CharacterCustomization.SetCharacterMouth(index3);
        else
          this.SetRandomMouth();
        int index4;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Accessory, out index4) && index3 != -1)
          CharacterCustomization.SetCharacterAccessory(index4);
        else
          this.SetRandomAccessory();
        int index5;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Outfit, out index5) && index5 != -1)
          CharacterCustomization.SetCharacterOutfit(index5);
        else
          this.SetRandomOutfit();
        int index6;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Hat, out index6) && index6 != -1)
          CharacterCustomization.SetCharacterHat(index6);
        else
          this.SetRandomHat();
        int index7;
        if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Sash, out index7) && index7 != -1)
          CharacterCustomization.SetCharacterSash(index7);
        else
          CharacterCustomization.SetCharacterSash(0);
      }
      else
        this.RandomizeCosmetics();
      Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.LoadedCosmeticsPreviously, 1);
    }
    else
      this.SetRandomSkinColor();
  }

  [ConsoleCommand]
  public static void Randomize()
  {
    Character.localCharacter.refs.customization.RandomizeCosmetics();
  }

  public void RandomizeCosmetics()
  {
    this.SetRandomSkinColor();
    this.SetRandomEyes();
    this.SetRandomMouth();
    this.SetRandomAccessory();
    this.SetRandomOutfit();
    this.SetRandomHat();
  }

  private void OnDestroy()
  {
    GameHandler.GetService<PersistentPlayerDataService>()?.UnsubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
  }

  public void SetCustomizationForRef(CustomizationRefs refs)
  {
    CustomizationRefs refs1 = this.refs;
    this.refs = refs;
    PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
    service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
    this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
    this.refs = refs1;
  }

  private void OnPlayerDataChange(PersistentPlayerData playerData)
  {
    if ((UnityEngine.Object) this.refs.PlayerRenderers[0] == (UnityEngine.Object) null || this._character.isBot)
      return;
    Debug.Log((object) "On Player Data Change");
    int index1 = CharacterCustomization.GetSkinIndex(playerData);
    if (this.useDebugColor)
      index1 = this.debugColorIndex;
    foreach (Renderer playerRenderer in this.refs.PlayerRenderers)
      playerRenderer.material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[index1].color);
    foreach (Renderer eyeRenderer in this.refs.EyeRenderers)
      eyeRenderer.material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[index1].color);
    int fitIndex = CharacterCustomization.GetFitIndex(playerData);
    this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
    this.refs.mainRendererShadow.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
    this.refs.mainRenderer.SetSharedMaterials(new List<Material>()
    {
      this.refs.mainRenderer.materials[0],
      Singleton<Customization>.Instance.fits[fitIndex].fitMaterial,
      Singleton<Customization>.Instance.fits[fitIndex].fitMaterialShoes
    });
    if (Singleton<Customization>.Instance.fits[fitIndex].noPants)
    {
      this.refs.skirt.gameObject.SetActive(false);
      this.refs.shortsShadow.gameObject.SetActive(false);
      this.refs.skirtShadow.gameObject.SetActive(false);
      this.refs.shorts.gameObject.SetActive(false);
    }
    else if (Singleton<Customization>.Instance.fits[fitIndex].isSkirt)
    {
      this.refs.skirt.gameObject.SetActive(true);
      this.refs.shortsShadow.gameObject.SetActive(false);
      this.refs.skirtShadow.gameObject.SetActive(true);
      this.refs.shorts.gameObject.SetActive(false);
      this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
    }
    else
    {
      this.refs.skirt.gameObject.SetActive(false);
      this.refs.shortsShadow.gameObject.SetActive(true);
      this.refs.skirtShadow.gameObject.SetActive(false);
      this.refs.shorts.gameObject.SetActive(true);
      this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
    }
    this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
    this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
    this.CurrentEyeTexture = Singleton<Customization>.Instance.eyes[CharacterCustomization.GetEyesIndex(playerData)].texture;
    foreach (Renderer eyeRenderer in this.refs.EyeRenderers)
      eyeRenderer.material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
    int mouthIndex = CharacterCustomization.GetMouthIndex(playerData);
    this.refs.mouthRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
    int accessoryIndex = CharacterCustomization.GetAccessoryIndex(playerData);
    this.refs.accessoryRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
    this.refs.accessoryRenderer.material.renderQueue = Singleton<Customization>.Instance.accessories[accessoryIndex].drawUnderEye ? 3007 : 3009;
    int num = playerData.customizationData.currentHat;
    if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
      num = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
    MeshFilter meshFilter = (MeshFilter) null;
    for (int index2 = 0; index2 < this.refs.playerHats.Length; ++index2)
    {
      this.refs.playerHats[index2].gameObject.SetActive(num == index2);
      if (num == index2)
        meshFilter = this.refs.playerHats[index2].GetComponent<MeshFilter>();
    }
    if (!(bool) (UnityEngine.Object) meshFilter)
      meshFilter = this.refs.playerHats[0].GetComponent<MeshFilter>();
    this.refs.hatShadowMeshFilter.sharedMesh = meshFilter.sharedMesh;
    this.refs.hatShadowMeshFilter.transform.SetPositionAndRotation(meshFilter.transform.position, meshFilter.transform.rotation);
    this.refs.hatShadowMeshFilter.transform.localScale = meshFilter.transform.localScale;
    List<Material> materials = new List<Material>();
    materials.Add(this.refs.sashRenderer.materials[0]);
    int sashIndex = CharacterCustomization.GetSashIndex(playerData);
    materials.Add(this.refs.sashAscentMaterials[sashIndex]);
    this.refs.sashRenderer.SetMaterials(materials);
    if (!(bool) (UnityEngine.Object) this._character)
      return;
    this._character.refs.hideTheBody.Refresh();
  }

  public static int GetEyesIndex(PersistentPlayerData playerData)
  {
    int eyesIndex = playerData.customizationData.currentEyes;
    if (eyesIndex >= Singleton<Customization>.Instance.eyes.Length)
      eyesIndex = 0;
    return eyesIndex;
  }

  public static int GetSkinIndex(PersistentPlayerData playerData)
  {
    int skinIndex = playerData.customizationData.currentSkin;
    if (skinIndex >= Singleton<Customization>.Instance.skins.Length)
      skinIndex = 0;
    return skinIndex;
  }

  public static int GetFitIndex(PersistentPlayerData playerData)
  {
    int fitIndex = playerData.customizationData.currentOutfit;
    if (fitIndex >= Singleton<Customization>.Instance.fits.Length)
      fitIndex = 0;
    return fitIndex;
  }

  public static int GetMouthIndex(PersistentPlayerData playerData)
  {
    int mouthIndex = playerData.customizationData.currentMouth;
    if (mouthIndex >= Singleton<Customization>.Instance.mouths.Length)
      mouthIndex = 0;
    return mouthIndex;
  }

  public static int GetAccessoryIndex(PersistentPlayerData playerData)
  {
    int accessoryIndex = playerData.customizationData.currentAccessory;
    if (accessoryIndex >= Singleton<Customization>.Instance.accessories.Length)
      accessoryIndex = 0;
    return accessoryIndex;
  }

  public static int GetSashIndex(PersistentPlayerData playerData)
  {
    int sashIndex = playerData.customizationData.currentSash;
    if (sashIndex >= Singleton<Customization>.Instance.sashes.Length)
      sashIndex = Singleton<Customization>.Instance.sashes.Length - 1;
    return sashIndex;
  }

  public static void SetCharacterSkinColor(int index)
  {
    if (index >= Singleton<Customization>.Instance.skins.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentSkin = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Skin, index);
    Debug.Log((object) $"Set character color: {index}");
  }

  public static void SetCharacterEyes(int index)
  {
    if (index >= Singleton<Customization>.Instance.eyes.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentEyes = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Eyes, index);
    Debug.Log((object) $"Set character eyes: {index}");
  }

  public static void SetCharacterMouth(int index)
  {
    if (index >= Singleton<Customization>.Instance.mouths.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentMouth = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Mouth, index);
    Debug.Log((object) $"Setting Character Mouth: {index}");
  }

  public static void SetCharacterAccessory(int index)
  {
    if (index >= Singleton<Customization>.Instance.accessories.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentAccessory = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Accessory, index);
    Debug.Log((object) $"Setting Character Accessory: {index}");
  }

  public static void SetCharacterOutfit(int index)
  {
    if (index >= Singleton<Customization>.Instance.fits.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentOutfit = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Outfit, index);
    Debug.Log((object) $"Setting Character outfit: {index}");
  }

  public static void SetCharacterHat(int index)
  {
    if (index >= Singleton<Customization>.Instance.hats.Length)
      index = 0;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentHat = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Hat, index);
    Debug.Log((object) $"Setting Character Hat: {index}");
  }

  public static void SetCharacterSash(int index)
  {
    if (index >= Singleton<Customization>.Instance.sashes.Length)
      index = Singleton<Customization>.Instance.sashes.Length - 1;
    CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
    customizationData.currentSash = index;
    CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
    Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Sash, index);
    Debug.Log((object) $"Setting Character Sash: {index}");
  }

  public void SetRandomSkinColor()
  {
    CharacterCustomization.SetCharacterSkinColor(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Skin));
  }

  public void SetRandomEyes()
  {
    CharacterCustomization.SetCharacterEyes(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Eyes));
  }

  public void SetRandomMouth()
  {
    CharacterCustomization.SetCharacterMouth(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Mouth));
  }

  public void SetRandomAccessory()
  {
    CharacterCustomization.SetCharacterAccessory(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Accessory));
  }

  public void SetRandomOutfit()
  {
    CharacterCustomization.SetCharacterOutfit(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Fit));
  }

  public void SetRandomHat()
  {
    CharacterCustomization.SetCharacterHat(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Hat));
  }

  public void SetRandomIdle()
  {
    if (!this.view.IsMine)
      return;
    this.view.RPC("SetCharacterIdle_RPC", RpcTarget.AllBuffered, (object) UnityEngine.Random.Range(0, this.maxIdles));
  }

  [PunRPC]
  public void CharacterDied()
  {
    for (int index = 0; index < this.refs.EyeRenderers.Length; ++index)
    {
      this.refs.EyeRenderers[index].material.SetTexture(CharacterCustomization.MainTex, this.deadEyes);
      this.refs.EyeRenderers[index].material.SetInt(CharacterCustomization.Spin, 0);
    }
  }

  [PunRPC]
  public void CharacterPassedOut()
  {
    for (int index = 0; index < this.refs.EyeRenderers.Length; ++index)
    {
      this.refs.EyeRenderers[index].material.SetTexture(CharacterCustomization.MainTex, this.passedOutEyes);
      this.refs.EyeRenderers[index].material.SetInt(CharacterCustomization.Spin, 1);
    }
  }

  public void BecomeChicken()
  {
    if (this.isCannibalizable)
      return;
    this.isCannibalizable = true;
    if (this.chickenTweener != null)
      this.chickenTweener.Kill();
    this.ShowChicken();
    this.chickenTweener = (Tweener) this.refs.chickenRenderer.material.DOFloat(1f, CharacterCustomization.Opacity, 1f).OnComplete<TweenerCore<float, float, FloatOptions>>(new TweenCallback(this.HideHuman));
    for (int index1 = 0; index1 < this.refs.AllRenderers.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.refs.AllRenderers[index1].materials.Length; ++index2)
        this.refs.AllRenderers[index1].materials[index2].DOFloat(0.0f, CharacterCustomization.Opacity, 1f);
    }
    this.chickenParticle.Play();
    this.refs.hatTransform.DOLocalMoveY(-4.66f, 1f);
  }

  public void BecomeHuman()
  {
    if (!this.isCannibalizable)
      return;
    this.isCannibalizable = false;
    if (this.chickenTweener != null)
      this.chickenTweener.Kill();
    this.chickenTweener = (Tweener) this.refs.chickenRenderer.material.DOFloat(0.0f, CharacterCustomization.Opacity, 1f).OnComplete<TweenerCore<float, float, FloatOptions>>(new TweenCallback(this.HideChicken));
    this.ShowHuman();
    for (int index1 = 0; index1 < this.refs.AllRenderers.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.refs.AllRenderers[index1].materials.Length; ++index2)
        this.refs.AllRenderers[index1].materials[index2].DOFloat(1f, CharacterCustomization.Opacity, 1f);
    }
    this.chickenParticle.Stop();
    this.refs.hatTransform.DOLocalMoveY(-3.98f, 1f);
  }

  private void ShowChicken() => this.refs.chickenRenderer.gameObject.SetActive(true);

  private void HideChicken() => this.refs.chickenRenderer.gameObject.SetActive(false);

  private void ShowHuman()
  {
    this.refs.mainRenderer.gameObject.SetActive(true);
    this.refs.sashRenderer.gameObject.SetActive(true);
    this.refs.shorts.gameObject.SetActive(true);
    this.refs.skirt.gameObject.SetActive(true);
  }

  private void HideHuman()
  {
    this.refs.mainRenderer.gameObject.SetActive(false);
    this.refs.sashRenderer.gameObject.SetActive(false);
    this.refs.shorts.gameObject.SetActive(false);
    this.refs.skirt.gameObject.SetActive(false);
  }

  public void PulseStatus(Color c)
  {
    for (int index1 = 0; index1 < this.refs.PlayerRenderers.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.refs.PlayerRenderers[index1].materials.Length; ++index2)
      {
        this.refs.PlayerRenderers[index1].materials[index2].SetColor(CharacterCustomization.StatusColor, c);
        this.refs.PlayerRenderers[index1].materials[index2].SetFloat(CharacterCustomization.StatusGlow, 1f);
        this.refs.PlayerRenderers[index1].materials[index2].DOFloat(0.0f, CharacterCustomization.StatusGlow, 0.5f);
      }
    }
  }

  [PunRPC]
  public void SetCharacterIdle_RPC(int index)
  {
    this.PlayerAnimator.SetFloat(CharacterCustomization.Idle, (float) index);
    Debug.Log((object) $"Setting Character Idle: {index}");
  }

  private static CharacterCustomizationData GetCustomizationData(Photon.Realtime.Player player)
  {
    return GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player).customizationData;
  }

  private static void SetCustomizationData(
    CharacterCustomizationData customizationData,
    Photon.Realtime.Player player)
  {
    PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
    PersistentPlayerData playerData = service.GetPlayerData(player);
    playerData.customizationData = customizationData;
    service.SetPlayerData(player, playerData);
  }

  public void Update()
  {
    if (this._character.data.passedOut && !this.isPassedOut)
    {
      this.isPassedOut = true;
      if (this.view.IsMine)
        this.view.RPC("CharacterPassedOut", RpcTarget.AllBuffered);
    }
    if (!this._character.data.dead || this.isDead)
      return;
    this.isDead = true;
    if (!this.view.IsMine)
      return;
    this.view.RPC("CharacterDied", RpcTarget.AllBuffered);
  }

  public void OnRevive()
  {
    if (!this.view.IsMine)
      return;
    this.view.RPC("OnRevive_RPC", RpcTarget.AllBuffered);
  }

  [PunRPC]
  public void OnRevive_RPC()
  {
    Debug.Log((object) "test dead");
    this.isDead = false;
    this.isPassedOut = false;
    for (int index = 0; index < this.refs.EyeRenderers.Length; ++index)
    {
      this.refs.EyeRenderers[index].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
      this.refs.EyeRenderers[index].material.SetInt(CharacterCustomization.Spin, 0);
    }
  }
}
