// Decompiled with JetBrains decompiler
// Type: TombTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class TombTrigger : MonoBehaviour
{
  private bool triggered;

  private void OnTriggerEnter(Collider other)
  {
    Debug.LogError((object) "Attempting tomb trigger");
    Character componentInParent = other.GetComponentInParent<Character>();
    if ((Object) componentInParent != (Object) null && (Object) componentInParent == (Object) Character.localCharacter && !this.triggered)
      this.TriggerTomb();
    if (!((Object) componentInParent != (Object) null) || !((Object) componentInParent == (Object) Character.localCharacter) || !(bool) (Object) Character.localCharacter.GetComponent<CharacterAnimations>() || !(bool) (Object) Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio)
      return;
    Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio.inTomb = true;
  }

  private void TriggerTomb()
  {
    this.triggered = true;
    GUIManager.instance.SetHeroTitle(Singleton<MountainProgressHandler>.Instance.tombProgressPoint.localizedTitle, Singleton<MountainProgressHandler>.Instance.tombProgressPoint.clip);
  }
}
