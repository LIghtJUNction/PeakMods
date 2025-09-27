// Decompiled with JetBrains decompiler
// Type: PassportManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public class PassportManager : MenuWindow
{
  public static PassportManager instance;
  public Animator anim;
  public GameObject uiObject;
  public PassportTab[] tabs;
  public Customization.Type activeType;
  public PassportButton[] buttons;
  public PlayerCustomizationDummy dummy;
  public Camera dummyCamera;
  public TextMeshProUGUI nameText;
  public TextMeshProUGUI passportNumberText;
  private static string passportNumberString;
  public Color inactiveBorderColor;
  public Color activeBorderColor;
  public bool testUnlockAll;
  public SFX_Instance camIn;
  public SFX_Instance camOut;
  private bool closing;

  public override bool openOnStart => false;

  public override bool selectOnOpen => true;

  public override Selectable objectToSelectOnOpen => (Selectable) this.buttons[0].button;

  public override bool closeOnPause => true;

  public override bool closeOnUICancel => true;

  public override bool autoHideOnClose => false;

  public void Awake()
  {
    PassportManager.instance = this;
    this.uiObject.SetActive(false);
  }

  [ConsoleCommand]
  public static void TestAllCosmetics()
  {
    if (!((Object) PassportManager.instance != (Object) null))
      return;
    PassportManager.instance.testUnlockAll = true;
  }

  public static string GeneratePassportNumber(string name)
  {
    return $"{PassportManager.GenerateCountryCode(name)}{PassportManager.GenerateNumericCode(name, 9):D7}";
  }

  private static string GenerateCountryCode(string name)
  {
    name = name.ToUpper().Replace(" ", "");
    if (name.Length < 2)
      name += "XX";
    return $"{name[0]}";
  }

  private static int GenerateNumericCode(string input, int length)
  {
    return Mathf.Abs(input.GetHashCode()) % (int) Mathf.Pow(10f, (float) length);
  }

  public void ToggleOpen()
  {
    if (this.closing)
      return;
    if (!this.isOpen)
    {
      this.Open();
      this.uiObject.SetActive(true);
      this.OpenTab(this.activeType);
    }
    else
      this.Close();
  }

  protected override void Initialize()
  {
    string characterName = Character.localCharacter.characterName;
    PassportManager.passportNumberString = PassportManager.GeneratePassportNumber(characterName);
    this.nameText.text = characterName;
    this.passportNumberText.text = PassportManager.passportNumberString;
  }

  protected override void OnClose() => this.StartCoroutine(this.CloseRoutine());

  private IEnumerator CloseRoutine()
  {
    this.closing = true;
    this.anim.Play("Close");
    this.CameraIn();
    yield return (object) new WaitForSeconds(0.5f);
    this.uiObject.SetActive(false);
    this.closing = false;
  }

  public void OpenTab(Customization.Type type)
  {
    this.activeType = type;
    int index1 = 0;
    for (int index2 = 0; index2 < this.tabs.Length; ++index2)
    {
      if (this.tabs[index2].type == type)
        index1 = index2;
      else
        this.tabs[index2].Close();
    }
    this.tabs[index1].Open();
    if (index1 == 4 || index1 == 6)
      this.CameraOut();
    else
      this.CameraIn();
    this.SetButtons();
    this.dummy.UpdateDummy();
  }

  private void CameraIn()
  {
    this.dummyCamera.DOOrthoSize(0.6f, 0.2f);
    this.dummyCamera.transform.DOLocalMove(new Vector3(0.0f, 1.65f, 1f), 0.2f);
    if (!(bool) (Object) this.camIn)
      return;
    this.camIn.Play();
  }

  private void CameraOut()
  {
    this.dummyCamera.DOOrthoSize(1.3f, 0.2f);
    this.dummyCamera.transform.DOLocalMove(new Vector3(0.0f, 1.05f, 1f), 0.2f);
    if (!(bool) (Object) this.camOut)
      return;
    this.camOut.Play();
  }

  public void SetButtons()
  {
    CustomizationOption[] list = Singleton<Customization>.Instance.GetList(this.activeType);
    for (int index = 0; index < this.buttons.Length; ++index)
    {
      if (index < list.Length)
        this.buttons[index].SetButton(list[index], index);
      else
        this.buttons[index].SetButton((CustomizationOption) null, -1);
    }
    this.SetActiveButton();
  }

  private void SetActiveButton()
  {
    PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
    int num = playerData.customizationData.currentSkin;
    if (this.activeType == Customization.Type.Accessory)
      num = playerData.customizationData.currentAccessory;
    else if (this.activeType == Customization.Type.Eyes)
      num = playerData.customizationData.currentEyes;
    else if (this.activeType == Customization.Type.Mouth)
      num = playerData.customizationData.currentMouth;
    else if (this.activeType == Customization.Type.Fit)
      num = playerData.customizationData.currentOutfit;
    else if (this.activeType == Customization.Type.Hat)
      num = playerData.customizationData.currentHat;
    else if (this.activeType == Customization.Type.Sash)
      num = playerData.customizationData.currentSash;
    for (int index = 0; index < this.buttons.Length; ++index)
      this.buttons[index].border.color = num == index ? this.activeBorderColor : this.inactiveBorderColor;
  }

  public void SetOption(CustomizationOption option, int index)
  {
    if (option.type == Customization.Type.Skin)
      CharacterCustomization.SetCharacterSkinColor(index);
    else if (option.type == Customization.Type.Eyes)
      CharacterCustomization.SetCharacterEyes(index);
    else if (option.type == Customization.Type.Mouth)
      CharacterCustomization.SetCharacterMouth(index);
    else if (option.type == Customization.Type.Accessory)
      CharacterCustomization.SetCharacterAccessory(index);
    else if (option.type == Customization.Type.Fit)
      CharacterCustomization.SetCharacterOutfit(index);
    else if (option.type == Customization.Type.Hat)
      CharacterCustomization.SetCharacterHat(index);
    else if (option.type == Customization.Type.Sash)
      CharacterCustomization.SetCharacterSash(index);
    this.SetActiveButton();
    this.dummy.UpdateDummy();
  }
}
