// Decompiled with JetBrains decompiler
// Type: DevMessageUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class DevMessageUI : MonoBehaviour
{
  public GameObject parent;
  public GameObject shillParent;
  public TextMeshProUGUI[] texts;
  private NextLevelService service;
  public bool useDebugMessage;
  public string debugMessage;
  private string message;

  private void Start() => this.service = GameHandler.GetService<NextLevelService>();

  private void Update()
  {
    int num = !this.service.Data.IsSome ? 0 : (!string.IsNullOrEmpty(this.service.Data.Value.DevMessage) ? 1 : 0);
    if (num != 0)
    {
      this.message = this.useDebugMessage ? this.debugMessage : this.service.Data.Value.DevMessage;
      if (this.message.StartsWith("#"))
      {
        this.message = this.message.Remove(0, 1);
        this.parent.SetActive(false);
        this.shillParent.SetActive(true);
      }
      else
      {
        this.parent.SetActive(true);
        this.shillParent.SetActive(false);
      }
    }
    else
    {
      this.parent.SetActive(false);
      this.shillParent.SetActive(false);
    }
    if (num == 0)
      return;
    foreach (TMP_Text text in this.texts)
      text.text = this.message;
  }
}
