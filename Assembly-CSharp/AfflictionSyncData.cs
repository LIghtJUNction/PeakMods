// Decompiled with JetBrains decompiler
// Type: AfflictionSyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public struct AfflictionSyncData : IBinarySerializable
{
  public List<Affliction> afflictions;

  public void Serialize(BinarySerializer serializer)
  {
    if (this.afflictions == null)
      this.afflictions = new List<Affliction>();
    serializer.WriteInt(this.afflictions.Count);
    for (int index = 0; index < this.afflictions.Count; ++index)
    {
      serializer.WriteInt((int) this.afflictions[index].GetAfflictionType());
      this.afflictions[index].Serialize(serializer);
    }
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.afflictions = new List<Affliction>();
    int num = deserializer.ReadInt();
    for (int index = 0; index < num; ++index)
    {
      Affliction.AfflictionType afflictionType = (Affliction.AfflictionType) deserializer.ReadInt();
      Affliction blankAffliction = Affliction.CreateBlankAffliction(afflictionType);
      if (blankAffliction == null)
      {
        Debug.LogError((object) $"FAILED TO CREATE AFFLICTION OF TYPE '{afflictionType.ToString()}'! Affliction.CreateBlankAffliction() is likely missing a case for this type.");
        break;
      }
      blankAffliction.Deserialize(deserializer);
      this.afflictions.Add(blankAffliction);
    }
  }
}
