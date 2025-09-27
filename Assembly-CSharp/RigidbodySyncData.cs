// Decompiled with JetBrains decompiler
// Type: RigidbodySyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public struct RigidbodySyncData
{
  public Vector3 position;
  public Quaternion rotation;

  public RigidbodySyncData(Rigidbody rig)
  {
    this.position = rig.position;
    this.rotation = rig.rotation;
  }
}
