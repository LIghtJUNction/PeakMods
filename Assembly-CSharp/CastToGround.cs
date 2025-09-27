// Decompiled with JetBrains decompiler
// Type: CastToGround
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CastToGround : MonoBehaviour
{
  public bool castOnStart = true;
  public Vector3 offset;

  private void Start()
  {
    if (!this.castOnStart)
      return;
    this.castToGround();
  }

  public void castToGround()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.transform.position, Vector3.down, out hitInfo))
      return;
    this.transform.position = hitInfo.point + this.offset;
    this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
  }

  private void Update()
  {
  }
}
