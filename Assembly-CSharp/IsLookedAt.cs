// Decompiled with JetBrains decompiler
// Type: IsLookedAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class IsLookedAt : MonoBehaviour
{
  public AnimatedMouth mouth;
  public CharacterInteractible characterInteractible;
  public float visibleDistance = 8f;
  public float visibleAngle = 45f;
  public float angleDistRatio = 45f;
  public Transform playerNamePos;
  private int index;

  private void Start()
  {
    if ((Object) this.characterInteractible.character == (Object) Character.localCharacter)
      this.gameObject.SetActive(false);
    else
      this.index = GUIManager.instance.playerNames.Init(this.characterInteractible);
  }

  private void Update()
  {
    bool visible = false;
    float num1 = Vector3.Distance(MainCamera.instance.transform.position, this.transform.position);
    float num2 = Vector3.Angle(MainCamera.instance.transform.forward, this.transform.position - MainCamera.instance.transform.position);
    if ((double) num1 < (double) this.visibleDistance && (double) num2 < (double) this.visibleAngle + ((double) this.visibleDistance - (double) num1) / (double) this.visibleDistance * (double) this.angleDistRatio)
      visible = true;
    if (this.mouth.character.data.isBlind)
      visible = false;
    GUIManager.instance.playerNames.UpdateName(this.index, this.playerNamePos.position, visible, this.mouth.amplitudeIndex);
  }

  private void OnDisable() => GUIManager.instance.playerNames.DisableName(this.index);
}
