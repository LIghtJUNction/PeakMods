// Decompiled with JetBrains decompiler
// Type: Photon.Chat.UtilityScripts.EventSystemSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Photon.Chat.UtilityScripts;

public class EventSystemSpawner : MonoBehaviour
{
  private void OnEnable()
  {
    if (!((Object) Object.FindFirstObjectByType<EventSystem>() == (Object) null))
      return;
    GameObject gameObject = new GameObject("EventSystem");
    gameObject.AddComponent<EventSystem>();
    gameObject.AddComponent<StandaloneInputModule>();
  }
}
