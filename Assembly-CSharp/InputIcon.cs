// Decompiled with JetBrains decompiler
// Type: InputIcon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
public class InputIcon : MonoBehaviour
{
  private TMP_Text text;
  public GameObject hold;
  public InputSpriteData.InputAction action;
  public TMP_SpriteAsset keyboardSprites;
  public TMP_SpriteAsset xboxSprites;
  public TMP_SpriteAsset switchSprites;
  public TMP_SpriteAsset ps5Sprites;
  public TMP_SpriteAsset ps4Sprites;
  public bool disableIfController;
  public bool disableIfKeyboard;
  private ControllerIconSetting setting;

  private void Awake() => this.text = this.GetComponent<TMP_Text>();

  private void Start()
  {
    this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
  }

  private void OnEnable()
  {
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged += new Action<InputScheme>(this.OnDeviceChange);
    this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
  }

  private void OnDisable()
  {
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged -= new Action<InputScheme>(this.OnDeviceChange);
  }

  private void OnDeviceChange(InputScheme scheme)
  {
    if (this.setting == null)
    {
      if ((UnityEngine.Object) GameHandler.Instance == (UnityEngine.Object) null)
        return;
      this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
    }
    if (scheme == InputScheme.KeyboardMouse || this.setting.Value == ControllerIconSetting.IconMode.KBM)
      this.text.spriteAsset = this.keyboardSprites;
    else if (scheme == InputScheme.Gamepad)
    {
      if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
      {
        switch (InputHandler.GetGamepadType())
        {
          case GamepadType.Unkown:
            this.text.spriteAsset = this.xboxSprites;
            break;
          case GamepadType.Xbox:
            this.text.spriteAsset = this.xboxSprites;
            break;
          case GamepadType.Dualshock:
            this.text.spriteAsset = this.ps5Sprites;
            break;
          case GamepadType.Dualsense:
            this.text.spriteAsset = this.ps5Sprites;
            break;
          case GamepadType.SteamDeck:
            this.text.spriteAsset = this.xboxSprites;
            break;
        }
      }
      else if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
        this.text.spriteAsset = this.xboxSprites;
      else if (this.setting.Value == ControllerIconSetting.IconMode.Style2)
        this.text.spriteAsset = this.ps5Sprites;
    }
    this.SetText(scheme);
  }

  private void SetText(InputScheme scheme)
  {
    switch (scheme)
    {
      case InputScheme.KeyboardMouse:
        this.text.enabled = !this.disableIfKeyboard;
        break;
      case InputScheme.Gamepad:
        this.text.enabled = !this.disableIfController;
        break;
    }
    string str = scheme != InputScheme.Gamepad || this.action != InputSpriteData.InputAction.Scroll ? SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(this.action, scheme) : SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollForward, scheme) + SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollBackward, scheme);
    if (!string.IsNullOrEmpty(str))
      this.text.text = str;
    if (scheme != InputScheme.Gamepad || !((UnityEngine.Object) this.hold != (UnityEngine.Object) null))
      return;
    this.hold.SetActive(this.action == InputSpriteData.InputAction.Throw || this.action == InputSpriteData.InputAction.HoldInteract);
  }
}
