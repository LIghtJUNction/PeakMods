// Decompiled with JetBrains decompiler
// Type: SpawnPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Flags]
public enum SpawnPool
{
  None = 0,
  Unused1 = 1,
  Unused2 = 2,
  MushroomCluster = 4,
  BerryBushBeach = 8,
  BerryBushJungle = 16, // 0x00000010
  SpikyVine = 32, // 0x00000020
  CoconutTree = 64, // 0x00000040
  WillowTreeJungle = 128, // 0x00000080
  JungleVine = 256, // 0x00000100
  WinterberryTree = 512, // 0x00000200
  LuggageBeach = 1024, // 0x00000400
  LuggageJungle = 2048, // 0x00000800
  LuggageTundra = 4096, // 0x00001000
  LuggageCaldera = 8192, // 0x00002000
  LuggageClimber = 16384, // 0x00004000
  LuggageAncient = 32768, // 0x00008000
  LuggageCursed = 65536, // 0x00010000
  RespawnCoffin = 131072, // 0x00020000
  Nest = 262144, // 0x00040000
  GuidebookPageBeach = 524288, // 0x00080000
  GuidebookPageTropics = 1048576, // 0x00100000
  GuidebookPageAlpine = 2097152, // 0x00200000
  Cactus = 4194304, // 0x00400000
  Redwood = 8388608, // 0x00800000
  LuggageMesa = 16777216, // 0x01000000
}
