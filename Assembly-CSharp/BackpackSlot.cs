// Decompiled with JetBrains decompiler
// Type: BackpackSlot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class BackpackSlot(byte slotID) : ItemSlot(slotID)
{
  public bool hasBackpack;

  public override void EmptyOut()
  {
    this.hasBackpack = false;
    base.EmptyOut();
  }

  public override bool IsEmpty() => !this.hasBackpack;

  public override string GetPrefabName() => "Backpack";
}
