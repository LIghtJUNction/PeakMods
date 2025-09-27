// Decompiled with JetBrains decompiler
// Type: RigPart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[Serializable]
public class RigPart
{
  [HideInInspector]
  public bool justCreated;
  public BodypartType partType;
  public float mass = 10f;
  public float spring = 10f;
  public Transform transform;
  public List<RigCreatorColliderData> colliders = new List<RigCreatorColliderData>();
  public RigCreatorRigidbody rigHandler;
  public Rigidbody rig;
  public ConfigurableJoint joint;
  public RigCreatorJoint jointHandler;
}
