// Decompiled with JetBrains decompiler
// Type: CompassPointer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CompassPointer : MonoBehaviour
{
  public CompassPointer.CompassType compassType;
  public Transform needle;
  public float warpSpeed = 2f;
  public float speedMultiplier = 1f;
  private Item item;
  protected Vector3 heading;
  private Vector3 currentLuggageVector = Vector3.zero;

  private void Awake() => this.item = this.GetComponentInParent<Item>();

  private void Update() => this.UpdateHeading();

  protected void UpdateHeading()
  {
    bool flag = true;
    switch (this.compassType)
    {
      case CompassPointer.CompassType.Normal:
        this.heading = Vector3.forward;
        break;
      case CompassPointer.CompassType.Warp:
        flag = false;
        this.needle.RotateAround(this.needle.transform.position, this.needle.right, this.warpSpeed * Time.deltaTime * this.speedMultiplier);
        break;
      case CompassPointer.CompassType.Pirate:
        this.UpdateHeadingPirate();
        break;
    }
    if (!flag)
      return;
    this.heading = Vector3.ProjectOnPlane(this.heading, this.transform.forward);
    this.needle.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(this.needle.transform.forward, this.heading, 10f * Time.deltaTime), this.transform.up);
  }

  protected void UpdateHeadingPirate()
  {
    if (Luggage.ALL_LUGGAGE.Count == 0)
      this.heading = Quaternion.Euler(0.0f, Time.time * this.warpSpeed, 0.0f) * Vector3.forward;
    if (!this.item.inActiveList)
      return;
    float num = float.MaxValue;
    foreach (Luggage luggage in Luggage.ALL_LUGGAGE)
    {
      if ((double) Vector3.Distance(luggage.Center(), this.transform.position) < (double) num)
      {
        num = Vector3.Distance(luggage.Center(), this.transform.position);
        this.currentLuggageVector = luggage.Center() - this.transform.position;
      }
    }
    this.heading = this.currentLuggageVector;
  }

  public enum CompassType
  {
    Normal,
    Warp,
    Pirate,
  }
}
