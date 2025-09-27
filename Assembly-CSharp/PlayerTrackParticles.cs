// Decompiled with JetBrains decompiler
// Type: PlayerTrackParticles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayerTrackParticles : MonoBehaviour
{
  public Bounds bounds;
  public GameObject fx;
  [Header("Track Axis")]
  public bool x;
  public bool y;
  public bool z;
  public float repositionDelta = 50f;
  private Vector3 lastPlayerPos = Vector3.positiveInfinity;
  public bool inBounds;
  public Vector3 positionOffset;

  private void Start()
  {
    if (!(this.bounds.center != this.transform.position))
      return;
    this.bounds.center = this.transform.position;
  }

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    this.inBounds = this.bounds.Contains(Character.observedCharacter.Center);
    if (!this.inBounds || (double) Vector3.Distance(this.lastPlayerPos, Character.observedCharacter.Center) <= (double) this.repositionDelta)
      return;
    Vector3 vector3 = this.fx.transform.position - this.positionOffset;
    if (this.x)
      vector3 = new Vector3(Character.observedCharacter.Center.x, vector3.y, vector3.z);
    if (this.y)
      vector3 = new Vector3(vector3.x, Character.observedCharacter.Center.y, vector3.z);
    if (this.z)
      vector3 = new Vector3(vector3.x, vector3.y, Character.observedCharacter.Center.z);
    this.fx.transform.position = vector3 + this.positionOffset;
    this.lastPlayerPos = Character.observedCharacter.Center;
  }

  private void OnDrawGizmosSelected()
  {
    if (this.bounds.center != this.transform.position)
      this.bounds.center = this.transform.position;
    Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
  }
}
