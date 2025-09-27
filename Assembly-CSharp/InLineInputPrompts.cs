// Decompiled with JetBrains decompiler
// Type: InLineInputPrompts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
[RequireComponent(typeof (TMP_Text))]
public class InLineInputPrompts : MonoBehaviour
{
  private TMP_Text text;
  private LocalizedText loc;
  private string originalText;
  private ControllerIconSetting setting;

  private void Awake()
  {
    this.text = this.GetComponent<TMP_Text>();
    this.loc = this.GetComponent<LocalizedText>();
    this.originalText = this.text.text;
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
    this.UpdateText(scheme);
    this.UpdateSprites(scheme);
  }

  private void UpdateText(InputScheme scheme)
  {
    string str = this.originalText;
    if ((bool) (UnityEngine.Object) this.loc)
    {
      str = this.loc.GetText();
      this.loc.enabled = false;
    }
    if (str.Contains("[") && str.Contains("]"))
    {
      foreach (object action in Enum.GetValues(typeof (InputSpriteData.InputAction)))
      {
        if (str.ToUpperInvariant().Contains(action.ToString().ToUpperInvariant()))
        {
          string spriteTag = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag((InputSpriteData.InputAction) action, scheme);
          if (!string.IsNullOrEmpty(spriteTag))
          {
            string upperInvariant = $"[{action}]".ToUpperInvariant();
            str = str.Replace(upperInvariant, spriteTag);
          }
        }
      }
    }
    this.text.text = str;
  }

  private void UpdateSprites(InputScheme scheme)
  {
    if (scheme == InputScheme.KeyboardMouse || this.setting.Value == ControllerIconSetting.IconMode.KBM)
    {
      this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.keyboardSprites;
    }
    else
    {
      if (scheme != InputScheme.Gamepad)
        return;
      if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
      {
        switch (InputHandler.GetGamepadType())
        {
          case GamepadType.Unkown:
            this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
            break;
          case GamepadType.Xbox:
            this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
            break;
          case GamepadType.Dualshock:
            this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
            break;
          case GamepadType.Dualsense:
            this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
            break;
          case GamepadType.SteamDeck:
            this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
            break;
        }
      }
      else if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
      {
        this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
      }
      else
      {
        if (this.setting.Value != ControllerIconSetting.IconMode.Style2)
          return;
        this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
      }
    }
  }
}
