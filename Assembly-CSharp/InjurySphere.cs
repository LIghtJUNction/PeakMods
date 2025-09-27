// Decompiled with JetBrains decompiler
// Type: InjurySphere
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class InjurySphere : MonoBehaviour
{
  public bool isHealing;

  private void Start()
  {
  }

  private void Update()
  {
    if ((double) Vector3.Distance(Character.localCharacter.data.groundPos, this.transform.position) >= (double) this.transform.localScale.x / 2.0)
      return;
    if (this.isHealing)
      Character.localCharacter.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f);
    else
      Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f);
  }
}
