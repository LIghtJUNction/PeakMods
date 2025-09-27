// Decompiled with JetBrains decompiler
// Type: FollowBodypart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FollowBodypart : MonoBehaviour
{
  public BodypartType followPart;
  private Transform target;

  private void Start()
  {
    this.target = this.GetComponentInParent<Character>().GetBodypart(this.followPart).transform;
  }

  private void LateUpdate() => this.transform.position = this.target.position;
}
