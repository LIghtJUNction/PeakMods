// Decompiled with JetBrains decompiler
// Type: PropSpawnerMod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public abstract class PropSpawnerMod
{
  public bool mute;

  public abstract void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData);
}
