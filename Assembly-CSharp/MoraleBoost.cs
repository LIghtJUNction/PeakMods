// Decompiled with JetBrains decompiler
// Type: MoraleBoost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MoraleBoost
{
  public static bool SpawnMoraleBoost(
    Vector3 origin,
    float radius,
    float baselineStaminaBoost,
    float staminaBoostPerAdditionalScout,
    bool sendToAll = false,
    int minScouts = 1)
  {
    List<Character> characterList = new List<Character>();
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      Character allCharacter = Character.AllCharacters[index];
      if ((double) radius == -1.0 || (double) Vector3.Distance(allCharacter.Center, origin) <= (double) radius)
        characterList.Add(allCharacter);
    }
    if (characterList.Count < minScouts)
      return false;
    float staminaAdd = baselineStaminaBoost;
    Debug.Log((object) $"Creating morale boost. Characters in radius: {characterList.Count} total boost: {staminaAdd}");
    foreach (Character character in characterList)
    {
      if (sendToAll)
        character.photonView.RPC(nameof (MoraleBoost), RpcTarget.All, (object) staminaAdd, (object) characterList.Count);
      else
        character.MoraleBoost(staminaAdd, characterList.Count);
    }
    return true;
  }
}
