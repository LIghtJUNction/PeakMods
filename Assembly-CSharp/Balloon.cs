// Decompiled with JetBrains decompiler
// Type: Balloon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;

#nullable disable
public class Balloon : ItemComponent
{
  public new Item item;
  public Renderer r;
  public Texture2D[] icons;
  public int colorIndex;
  public bool isBunch;

  public void Start() => this.StartCoroutine(this.InitColorYieldRoutine());

  private void InitColor()
  {
    if (this.HasData(DataEntryKey.Color))
    {
      this.colorIndex = this.GetData<IntItemData>(DataEntryKey.Color).Value;
      this.SetColor(this.colorIndex);
    }
    else
    {
      if (!this.photonView.IsMine)
        return;
      this.RandomizeColor();
      this.photonView.RPC("RPC_SyncColor", RpcTarget.All, (object) this.colorIndex);
    }
  }

  private void SetColor(int index)
  {
    this.r.sharedMaterial = Character.localCharacter.refs.balloons.balloonColors[this.colorIndex];
    this.item.UIData.icon = this.icons[this.colorIndex];
  }

  private void RandomizeColor()
  {
    this.colorIndex = Random.Range(0, Character.localCharacter.refs.balloons.balloonColors.Length);
    this.SetColor(this.colorIndex);
    this.GetData<IntItemData>(DataEntryKey.Color).Value = this.colorIndex;
  }

  [PunRPC]
  public void RPC_SyncColor(int colorIndex)
  {
    this.colorIndex = colorIndex;
    this.SetColor(this.colorIndex);
    this.GetData<IntItemData>(DataEntryKey.Color).Value = colorIndex;
  }

  public override void OnInstanceDataSet() => this.StartCoroutine(this.InitColorYieldRoutine());

  private IEnumerator InitColorYieldRoutine()
  {
    while (!(bool) (Object) Character.localCharacter)
      yield return (object) null;
    if (!this.isBunch)
      this.InitColor();
  }
}
