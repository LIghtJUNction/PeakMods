// Decompiled with JetBrains decompiler
// Type: ReconnectData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public struct ReconnectData
{
  public bool isValid;
  public Vector3 position;
  public bool dead;
  public bool fullyPassedOut;
  public float deathTimer;
  public Segment mapSegment;
  public float[] currentStatuses;
  public InventorySyncData inventorySyncData;

  public void PrintData()
  {
    Debug.Log((object) $"Reconnect Data: Position: {this.position}, Dead: {this.dead}, FullyPassedOut: {this.fullyPassedOut}, DeathTimer: {this.deathTimer}");
  }

  public static ReconnectData CreateFromCharacter(Character character, Segment mapSegment)
  {
    return new ReconnectData()
    {
      isValid = true,
      position = character.Center,
      dead = character.data.dead,
      fullyPassedOut = character.data.fullyPassedOut,
      deathTimer = character.data.deathTimer,
      currentStatuses = character.refs.afflictions.currentStatuses,
      inventorySyncData = new InventorySyncData(Player.localPlayer.itemSlots, Player.localPlayer.backpackSlot, Player.localPlayer.tempFullSlot),
      mapSegment = mapSegment
    };
  }

  public static ReconnectData Invalid
  {
    get
    {
      return new ReconnectData()
      {
        isValid = false,
        inventorySyncData = new InventorySyncData()
      };
    }
  }

  public byte[] Serialize()
  {
    BinarySerializer serializer = new BinarySerializer(100, Allocator.Temp);
    serializer.WriteBool(this.isValid);
    if (this.isValid)
    {
      serializer.WriteFloat3((float3) this.position);
      serializer.WriteBool(this.dead);
      serializer.WriteBool(this.fullyPassedOut);
      serializer.WriteFloat(this.deathTimer);
      new StatusSyncData()
      {
        statusList = new List<float>((IEnumerable<float>) Character.localCharacter.refs.afflictions.currentStatuses)
      }.Serialize(serializer);
      this.inventorySyncData.Serialize(serializer);
      serializer.WriteByte((byte) this.mapSegment);
    }
    byte[] byteArray = serializer.buffer.ToByteArray();
    serializer.Dispose();
    return byteArray;
  }

  public static ReconnectData Deserialize(byte[] data)
  {
    ReconnectData reconnectData = new ReconnectData();
    BinaryDeserializer deserializer = new BinaryDeserializer(data, Allocator.Temp);
    reconnectData.isValid = deserializer.ReadBool();
    if (reconnectData.isValid)
    {
      reconnectData.position = (Vector3) deserializer.ReadFloat3();
      reconnectData.dead = deserializer.ReadBool();
      reconnectData.fullyPassedOut = deserializer.ReadBool();
      reconnectData.deathTimer = deserializer.ReadFloat();
      reconnectData.currentStatuses = IBinarySerializable.Deserialize<StatusSyncData>(deserializer).statusList.ToArray();
      reconnectData.inventorySyncData = IBinarySerializable.Deserialize<InventorySyncData>(deserializer);
      reconnectData.mapSegment = (Segment) deserializer.ReadByte();
    }
    deserializer.Dispose();
    return reconnectData;
  }

  public override string ToString()
  {
    string newLine = Environment.NewLine;
    return $"IsValid: {this.isValid}{newLine}Position: {this.position}{newLine}Dead: {this.dead}{newLine}FullyPassedOut: {this.fullyPassedOut}{newLine}DeathTimer: {this.deathTimer}";
  }
}
