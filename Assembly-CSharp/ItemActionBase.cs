// Decompiled with JetBrains decompiler
// Type: ItemActionBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class ItemActionBase : MonoBehaviourPun
{
  protected Item item;

  [SerializeField]
  protected Character character => this.item.holderCharacter;

  public virtual void RunAction()
  {
  }

  protected virtual void OnEnable()
  {
    this.Init();
    this.Subscribe();
  }

  protected virtual void Start()
  {
    this.Unsubscribe();
    this.Subscribe();
  }

  public void OnDisable() => this.Unsubscribe();

  protected virtual void Subscribe()
  {
  }

  protected virtual void Unsubscribe()
  {
  }

  private void Init() => this.item = this.GetComponent<Item>();
}
