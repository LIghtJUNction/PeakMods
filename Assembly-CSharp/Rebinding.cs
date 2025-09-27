// Decompiled with JetBrains decompiler
// Type: Rebinding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.InputSystem;

#nullable disable
public static class Rebinding
{
  public static void LoadRebindingsFromFile(InputActionAsset actions = null)
  {
    if ((Object) actions == (Object) null)
      actions = UnityEngine.InputSystem.InputSystem.actions;
    string json = PlayerPrefs.GetString("rebinds");
    if (string.IsNullOrEmpty(json))
      return;
    actions.LoadBindingOverridesFromJson(json);
  }

  public static void SaveRebindingsToFile(InputActionAsset actions = null)
  {
    if ((Object) actions == (Object) null)
      actions = UnityEngine.InputSystem.InputSystem.actions;
    PlayerPrefs.SetString("rebinds", actions.SaveBindingOverridesAsJson());
  }
}
