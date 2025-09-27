// Decompiled with JetBrains decompiler
// Type: CharacterHeatEmission
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CharacterHeatEmission : MonoBehaviour
{
  public float radius = 1f;
  public float heatAmount = 0.05f;
  public float rate = 0.5f;
  private float counter;
  private Character character;

  private void Awake() => this.character = this.GetComponentInParent<Character>();

  public void Update()
  {
    this.transform.position = this.character.refs.hip.transform.position;
    if ((double) this.character.data.sinceAddedCold < 3.0)
      return;
    this.counter += Time.deltaTime;
    if ((double) this.counter < (double) this.rate)
      return;
    this.counter = 0.0f;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((double) Vector3.Distance(this.transform.position, allCharacter.Center) < (double) this.radius)
        allCharacter.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, this.heatAmount);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.radius);
  }
}
