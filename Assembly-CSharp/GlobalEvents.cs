// Decompiled with JetBrains decompiler
// Type: GlobalEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public static class GlobalEvents
{
  public static Action<Item, Character> OnItemRequested;
  public static Action<Item, Character> OnItemConsumed;
  public static Action<RespawnChest, Character> OnRespawnChestOpened;
  public static Action<Luggage, Character> OnLuggageOpened;
  public static Action OnLocalCharacterWonRun;
  public static Action OnSomeoneWonRun;
  public static Action<Character> OnCharacterPassedOut;
  public static Action OnRunEnded;
  public static Action<Item> OnBugleTooted;
  public static Action<Character> OnCharacterSpawned;
  public static Action<Character> OnCharacterOwnerDisconnected;
  public static Action OnCharacterAudioLevelsUpdated;
  public static Action<Photon.Realtime.Player> OnPlayerConnected;
  public static Action<Photon.Realtime.Player> OnPlayerDisconnected;
  public static Action<Item> OnItemThrown;
  public static Action<ACHIEVEMENTTYPE> OnAchievementThrown;

  public static void TriggerItemRequested(Item interactor, Character character)
  {
    try
    {
      if (GlobalEvents.OnItemRequested == null)
        return;
      GlobalEvents.OnItemRequested(interactor, character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerItemConsumed(Item item, Character character)
  {
    try
    {
      if ((UnityEngine.Object) item != (UnityEngine.Object) null && (UnityEngine.Object) character != (UnityEngine.Object) null)
        Debug.Log((object) $"{item.UIData.itemName} consumed by {character.gameObject.name}");
      if (GlobalEvents.OnItemConsumed == null)
        return;
      GlobalEvents.OnItemConsumed(item, character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerRespawnChestOpened(RespawnChest chest, Character character)
  {
    try
    {
      if (GlobalEvents.OnRespawnChestOpened == null)
        return;
      GlobalEvents.OnRespawnChestOpened(chest, character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerLuggageOpened(Luggage luggage, Character character)
  {
    try
    {
      if (GlobalEvents.OnLuggageOpened == null)
        return;
      GlobalEvents.OnLuggageOpened(luggage, character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerLocalCharacterWonRun()
  {
    try
    {
      if (GlobalEvents.OnLocalCharacterWonRun == null)
        return;
      GlobalEvents.OnLocalCharacterWonRun();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerSomeoneWonRun()
  {
    try
    {
      if (GlobalEvents.OnSomeoneWonRun == null)
        return;
      GlobalEvents.OnSomeoneWonRun();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerCharacterPassedOut(Character character)
  {
    try
    {
      if (GlobalEvents.OnCharacterPassedOut == null)
        return;
      GlobalEvents.OnCharacterPassedOut(character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerRunEnded()
  {
    try
    {
      if (GlobalEvents.OnRunEnded == null)
        return;
      GlobalEvents.OnRunEnded();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerBugleTooted(Item bugle)
  {
    try
    {
      if (GlobalEvents.OnBugleTooted == null)
        return;
      GlobalEvents.OnBugleTooted(bugle);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerCharacterSpawned(Character character)
  {
    try
    {
      if (GlobalEvents.OnCharacterSpawned == null)
        return;
      GlobalEvents.OnCharacterSpawned(character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerCharacterDestroyed(Character character)
  {
    try
    {
      if (GlobalEvents.OnCharacterOwnerDisconnected == null)
        return;
      GlobalEvents.OnCharacterOwnerDisconnected(character);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerCharacterAudioLevelsUpdated()
  {
    try
    {
      if (GlobalEvents.OnCharacterAudioLevelsUpdated == null)
        return;
      GlobalEvents.OnCharacterAudioLevelsUpdated();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerPlayerConnected(Photon.Realtime.Player player)
  {
    try
    {
      if (GlobalEvents.OnPlayerConnected == null)
        return;
      GlobalEvents.OnPlayerConnected(player);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerPlayerDisconnected(Photon.Realtime.Player player)
  {
    try
    {
      if (GlobalEvents.OnPlayerDisconnected == null)
        return;
      GlobalEvents.OnPlayerDisconnected(player);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerItemThrown(Item item)
  {
    try
    {
      if (GlobalEvents.OnItemThrown == null)
        return;
      GlobalEvents.OnItemThrown(item);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  public static void TriggerAchievementThrown(ACHIEVEMENTTYPE cheevo)
  {
    try
    {
      if (GlobalEvents.OnAchievementThrown == null)
        return;
      GlobalEvents.OnAchievementThrown(cheevo);
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }
}
