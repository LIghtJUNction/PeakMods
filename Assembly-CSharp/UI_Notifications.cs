// Decompiled with JetBrains decompiler
// Type: UI_Notifications
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class UI_Notifications : MonoBehaviour
{
  public GameObject prefab;

  public void AddNotification(string text)
  {
    Transform child = this.transform.GetChild(0);
    Object.Instantiate<GameObject>(this.prefab, child.position, child.rotation, child).GetComponentInChildren<TextMeshProUGUI>().text = text;
  }
}
