// Decompiled with JetBrains decompiler
// Type: AscentUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class AscentUI : MonoBehaviour
{
  public TextMeshProUGUI text;

  private void Update()
  {
    int currentAscent = Ascents._currentAscent;
    this.text.text = SingletonAsset<AscentData>.Instance.ascents[currentAscent + 1].localizedTitle;
    if (currentAscent != 0)
      return;
    this.text.text = "";
  }
}
