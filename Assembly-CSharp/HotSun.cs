// Decompiled with JetBrains decompiler
// Type: HotSun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteAlways]
public class HotSun : MonoBehaviour
{
  public Bounds bounds;
  public float rate = 0.5f;
  public float amount = 0.05f;
  private float counter;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
  }

  private void Start()
  {
  }

  private void Update()
  {
    this.bounds.center = this.transform.position;
    this.bounds.size = this.transform.localScale;
    if (!Application.isPlaying || (Object) Character.localCharacter == (Object) null || !this.bounds.Contains(Character.localCharacter.Center) || (double) DayNightManager.instance.sun.intensity < 5.0)
      return;
    Transform transform = DayNightManager.instance.sun.transform;
    RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical);
    bool flag = false;
    if ((Object) raycastHit.transform == (Object) null || (Object) raycastHit.transform.root == (Object) Character.localCharacter.transform.root)
      flag = true;
    if (!flag)
      return;
    this.counter += Time.deltaTime;
    if ((double) this.counter <= (double) this.rate)
      return;
    this.counter = 0.0f;
    Character.localCharacter.refs.afflictions.AddSunHeat(this.amount);
  }
}
