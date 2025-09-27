// Decompiled with JetBrains decompiler
// Type: SerializableRunBasedValues
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

#nullable disable
public class SerializableRunBasedValues : IBinarySerializable
{
  internal Dictionary<RUNBASEDVALUETYPE, int> runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();
  internal Dictionary<RUNBASEDVALUETYPE, float> runBasedFloats = new Dictionary<RUNBASEDVALUETYPE, float>();
  internal List<ushort> runBasedFruitsEaten = new List<ushort>();
  internal List<ushort> nonToxicMushroomsEaten = new List<ushort>();
  internal List<ushort> gourmandRequirementsEaten = new List<ushort>();
  internal List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = new List<ACHIEVEMENTTYPE>();
  internal List<int> completedAscentsThisRun = new List<int>();
  internal List<ACHIEVEMENTTYPE> steamAchievementsPreviouslyUnlocked = new List<ACHIEVEMENTTYPE>();

  internal void PrimeExistingAchievements()
  {
    this.steamAchievementsPreviouslyUnlocked.Clear();
    foreach (ACHIEVEMENTTYPE achievementtype in Enum.GetValues(typeof (ACHIEVEMENTTYPE)))
    {
      bool pbAchieved;
      if (SteamUserStats.GetAchievement(achievementtype.ToString(), out pbAchieved) && pbAchieved)
        this.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
    }
  }

  public void Serialize(BinarySerializer serializer)
  {
    this.SerializeRunBasedValues(serializer);
    this.SerializeUshortList(this.runBasedFruitsEaten, serializer);
    this.SerializeUshortList(this.gourmandRequirementsEaten, serializer);
    this.SerializeAchievementList(this.achievementsEarnedThisRun, serializer);
    this.SerializeAchievementList(this.steamAchievementsPreviouslyUnlocked, serializer);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.DeserializeRunBasedValues(deserializer);
    this.runBasedFruitsEaten = this.DeserializeUshortList(deserializer);
    this.gourmandRequirementsEaten = this.DeserializeUshortList(deserializer);
    this.achievementsEarnedThisRun = this.DeserializeAchievementList(deserializer);
    this.steamAchievementsPreviouslyUnlocked = this.DeserializeAchievementList(deserializer);
  }

  public void SerializeRunBasedValues(BinarySerializer serializer)
  {
    int count1 = this.runBasedInts.Count;
    serializer.WriteInt(count1);
    foreach (KeyValuePair<RUNBASEDVALUETYPE, int> runBasedInt in this.runBasedInts)
    {
      serializer.WriteInt((int) runBasedInt.Key);
      serializer.WriteInt(runBasedInt.Value);
    }
    int count2 = this.runBasedFloats.Count;
    serializer.WriteInt(count2);
    foreach (KeyValuePair<RUNBASEDVALUETYPE, float> runBasedFloat in this.runBasedFloats)
    {
      serializer.WriteInt((int) runBasedFloat.Key);
      serializer.WriteFloat(runBasedFloat.Value);
    }
  }

  public void DeserializeRunBasedValues(BinaryDeserializer deserializer)
  {
    this.runBasedInts.Clear();
    int num1 = deserializer.ReadInt();
    for (int index = 0; index < num1; ++index)
      this.runBasedInts.TryAdd((RUNBASEDVALUETYPE) deserializer.ReadInt(), deserializer.ReadInt());
    this.runBasedFloats.Clear();
    int num2 = deserializer.ReadInt();
    for (int index = 0; index < num2; ++index)
      this.runBasedFloats.TryAdd((RUNBASEDVALUETYPE) deserializer.ReadInt(), deserializer.ReadFloat());
  }

  public void SerializeUshortList(List<ushort> list, BinarySerializer serializer)
  {
    if (list == null)
      list = new List<ushort>();
    serializer.WriteInt(list.Count);
    for (int index = 0; index < list.Count; ++index)
      serializer.WriteUshort(list[index]);
  }

  public List<ushort> DeserializeUshortList(BinaryDeserializer deserializer)
  {
    List<ushort> ushortList = new List<ushort>();
    int num = deserializer.ReadInt();
    for (int index = 0; index < num; ++index)
      ushortList.Add(deserializer.ReadUShort());
    return ushortList;
  }

  public void SerializeAchievementList(List<ACHIEVEMENTTYPE> list, BinarySerializer serializer)
  {
    if (list == null)
      list = new List<ACHIEVEMENTTYPE>();
    serializer.WriteInt(list.Count);
    for (int index = 0; index < list.Count; ++index)
      serializer.WriteInt((int) list[index]);
  }

  public List<ACHIEVEMENTTYPE> DeserializeAchievementList(BinaryDeserializer deserializer)
  {
    List<ACHIEVEMENTTYPE> achievementtypeList = new List<ACHIEVEMENTTYPE>();
    int num = deserializer.ReadInt();
    for (int index = 0; index < num; ++index)
      achievementtypeList.Add((ACHIEVEMENTTYPE) deserializer.ReadInt());
    return achievementtypeList;
  }
}
