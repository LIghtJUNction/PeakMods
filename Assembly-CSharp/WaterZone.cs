// Decompiled with JetBrains decompiler
// Type: WaterZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaterZone : MonoBehaviour
{
  public Bounds zoneBounds;
  public Vector3 forceDirection;
  [SerializeField]
  private float Force;
  public bool characterInsideBounds;

  private void Awake() => this.zoneBounds.center = this.transform.position;

  private void Start()
  {
  }

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    this.characterInsideBounds = this.zoneBounds.Contains(Character.observedCharacter.Center);
  }

  private void FixedUpdate()
  {
    if ((Object) Character.localCharacter == (Object) null || !this.characterInsideBounds || !((Object) Character.observedCharacter == (Object) Character.localCharacter))
      return;
    this.AddForceToCharacter();
  }

  private void AddForceToCharacter()
  {
    Character.localCharacter.AddForce(-Character.localCharacter.data.worldMovementInput * 0.5f);
  }

  private void OnDrawGizmosSelected()
  {
    this.zoneBounds.center = this.transform.position;
    Gizmos.color = new Color(0.0f, 0.0f, 1f, 0.5f);
    Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
    Gizmos.color = Color.white;
    Gizmos.DrawLine(this.transform.position, this.transform.position + this.forceDirection * this.Force);
  }
}
