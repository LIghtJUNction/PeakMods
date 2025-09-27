// Decompiled with JetBrains decompiler
// Type: FeedData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class FeedData
{
  public int giverID;
  public int receiverID;
  public ushort itemID;
  public float totalItemTime;

  public void PrintDescription() => Debug.Log((object) this.GetDescription());

  public string GetDescription()
  {
    Character characterResult;
    int num = Character.GetCharacterWithPhotonID(this.giverID, out characterResult) ? 1 : 0;
    Character.GetCharacterWithPhotonID(this.receiverID, out Character _);
    Item obj;
    bool flag = ItemDatabase.TryGetItem(this.itemID, out obj);
    return $"{(num != 0 ? characterResult.characterName : "An unknown scout")} is feeding {(num != 0 ? characterResult.characterName : "an unknown scout")} a {(flag ? obj.GetItemName() : "an unknown item")}...";
  }
}
