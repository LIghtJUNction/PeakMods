// Decompiled with JetBrains decompiler
// Type: BingBongStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class BingBongStatus : MonoBehaviour
{
  private BingBongPowers bingBongPowers;
  private string descr = "Add status: [LMB]\n\nRemove status: [RMB]\n\nSelect all status: [F]\n\nPrev status: [V]\n\nNext status: [C]\n\n";
  private PhotonView view;
  private bool allStatusSelected;
  private CharacterAfflictions.STATUSTYPE currentStatusTarget;

  private void OnEnable()
  {
    this.bingBongPowers = this.GetComponent<BingBongPowers>();
    this.bingBongPowers.SetTexts("STATUS", this.descr);
  }

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      this.allStatusSelected = true;
      this.bingBongPowers.SetTip("Status: All", 2);
    }
    string[] names = Enum.GetNames(typeof (CharacterAfflictions.STATUSTYPE));
    int length = names.Length;
    int index = (int) this.currentStatusTarget;
    if (Input.GetKeyDown(KeyCode.V))
    {
      this.allStatusSelected = false;
      --index;
    }
    if (Input.GetKeyDown(KeyCode.C))
    {
      this.allStatusSelected = false;
      ++index;
    }
    if (index < 0)
      index = length - 1;
    if (index >= length)
      index = 0;
    if (this.currentStatusTarget != (CharacterAfflictions.STATUSTYPE) index)
    {
      this.currentStatusTarget = (CharacterAfflictions.STATUSTYPE) index;
      this.bingBongPowers.SetTip(names[index] ?? "", 2);
    }
    Character target = this.GetTarget();
    if (!(bool) (UnityEngine.Object) target)
      return;
    if (Input.GetKeyDown(KeyCode.Mouse0))
      this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, (object) target.photonView.ViewID, (object) (this.allStatusSelected ? -1 : (int) this.currentStatusTarget), (object) 1);
    if (!Input.GetKeyDown(KeyCode.Mouse1))
      return;
    this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, (object) target.photonView.ViewID, (object) (this.allStatusSelected ? -1 : (int) this.currentStatusTarget), (object) -1);
  }

  [PunRPC]
  public void RPCA_AddStatusBingBing(int target, int statusID, int mult)
  {
    Character component = PhotonView.Find(target).GetComponent<Character>();
    if (!component.IsLocal)
      return;
    if (mult > 0)
    {
      if (statusID == -1)
      {
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f);
        component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f);
      }
      else
        component.refs.afflictions.AddStatus(this.currentStatusTarget, 0.2f);
    }
    else if (statusID == -1)
    {
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f);
      component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f);
    }
    else
      component.refs.afflictions.SubtractStatus(this.currentStatusTarget, 0.2f);
  }

  private Character GetTarget()
  {
    Character target = (Character) null;
    float num1 = float.MaxValue;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num2 = Vector3.Angle(MainCamera.instance.transform.forward, allCharacter.Center - MainCamera.instance.transform.position);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        target = allCharacter;
      }
    }
    return target;
  }
}
