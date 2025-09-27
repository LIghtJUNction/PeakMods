// Decompiled with JetBrains decompiler
// Type: Lava
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Lava : MonoBehaviour
{
  private List<Character> hitPlayers = new List<Character>();
  public float heatRate = 0.5f;
  public float heat = 0.02f;
  public float height = 10f;
  public bool isKiln;
  private Bounds bounds;
  private float counter;
  public Dictionary<Item, float> itemToCookTime = new Dictionary<Item, float>();
  private List<Item> itemToRemoveList = new List<Item>();
  private List<Item> itemToCookList = new List<Item>();

  private void Start() => this.bounds = this.GetComponentInChildren<MeshRenderer>().bounds;

  private void FixedUpdate()
  {
    if ((UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null)
      return;
    this.Movement();
    if ((bool) (UnityEngine.Object) Character.localCharacter)
    {
      this.DoEffects();
      this.Heat();
    }
    this.TryCookItems();
  }

  private void Heat()
  {
    Character localCharacter = Character.localCharacter;
    if ((UnityEngine.Object) localCharacter == (UnityEngine.Object) null)
      return;
    this.counter += Time.deltaTime;
    if (this.OutsideBounds(localCharacter.Center))
      return;
    float num = 1f - Mathf.Clamp01((localCharacter.Center.y - this.transform.position.y) / this.height);
    if ((double) num < 0.0099999997764825821 || (double) this.counter < (double) this.heatRate)
      return;
    this.counter = 0.0f;
    localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, (float) ((double) num * (double) this.heat * 1.5));
  }

  private bool OutsideBounds(Vector3 pos)
  {
    return (double) pos.x > (double) this.bounds.max.x || (double) pos.x < (double) this.bounds.min.x || (double) pos.z > (double) this.bounds.max.z || (double) pos.z < (double) this.bounds.min.z;
  }

  private void DoEffects()
  {
    Character localCharacter = Character.localCharacter;
    if (this.OutsideBounds(localCharacter.Center) || (double) localCharacter.Center.y > (double) this.transform.position.y)
      return;
    localCharacter.AddForce(Vector3.up * 80f, 0.5f);
    localCharacter.data.sinceGrounded = 0.0f;
    localCharacter.refs.movement.ApplyExtraDrag(0.8f, true);
    if (this.hitPlayers.Contains(localCharacter) || localCharacter.data.dead || (double) localCharacter.refs.afflictions.statusSum > 1.8999999761581421)
      return;
    this.HitPlayer(localCharacter);
    this.StartCoroutine(this.IHoldPlayer(localCharacter));
  }

  private void HitPlayer(Character item)
  {
    item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f);
    item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.25f);
    item.data.sinceGrounded = 0.0f;
  }

  private IEnumerator IHoldPlayer(Character item)
  {
    this.hitPlayers.Add(item);
    yield return (object) new WaitForSeconds(1f);
    this.hitPlayers.Remove(item);
  }

  private void Movement()
  {
  }

  private void TryCookItems()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    for (int index = 0; index < Item.ALL_ACTIVE_ITEMS.Count; ++index)
    {
      Item unityObject = Item.ALL_ACTIVE_ITEMS[index];
      if (unityObject.UnityObjectExists<Item>() && !this.OutsideBounds(unityObject.Center()))
      {
        if (this.TestSacrificeIdol(unityObject))
          return;
        if (unityObject.cooking.canBeCooked && (double) this.GetItemCookAmount(unityObject) > 0.0 && this.itemToCookTime.TryAdd(unityObject, 0.0f))
        {
          Debug.Log((object) ("Lava started cooking: " + unityObject.GetItemName()));
          unityObject.GetComponent<ItemCooking>().StartCookingVisuals();
        }
      }
    }
    this.itemToRemoveList.Clear();
    this.itemToCookList.Clear();
    foreach (Item key in this.itemToCookTime.Keys)
    {
      if ((UnityEngine.Object) key == (UnityEngine.Object) null)
        this.itemToRemoveList.Add(key);
      else if (this.OutsideBounds(key.Center()))
      {
        this.itemToRemoveList.Add(key);
        key.GetComponent<ItemCooking>().CancelCookingVisuals();
      }
      else
        this.itemToCookList.Add(key);
    }
    foreach (Item itemToCook in this.itemToCookList)
    {
      float num = this.GetItemCookAmount(itemToCook) * Time.deltaTime;
      this.itemToCookTime[itemToCook] += num;
      if ((double) this.itemToCookTime[itemToCook] >= 1.0)
      {
        Debug.Log((object) ("Lava finished cooking: " + itemToCook.GetItemName()));
        itemToCook.GetComponent<ItemCooking>().FinishCooking();
        this.itemToCookTime[itemToCook] = 0.0f;
      }
    }
    foreach (Item itemToRemove in this.itemToRemoveList)
      this.itemToCookTime.Remove(itemToRemove);
  }

  private float GetItemCookAmount(Item item)
  {
    float num = 1f - Mathf.Clamp01((item.Center().y - this.transform.position.y) / this.height);
    return (double) num < 0.0099999997764825821 ? 0.0f : num * 0.7f;
  }

  private bool TestSacrificeIdol(Item item)
  {
    if (!this.isKiln || (double) item.Center().y > (double) this.transform.position.y || !item.photonView.IsMine || !item.itemTags.HasFlag((Enum) Item.ItemTags.GoldenIdol))
      return false;
    if ((UnityEngine.Object) Character.localCharacter.data.currentItem == (UnityEngine.Object) item)
    {
      Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
      Character.localCharacter.refs.afflictions.UpdateWeight();
    }
    PhotonNetwork.Destroy(item.gameObject);
    GameUtils.instance.ThrowSacrificeAchievement();
    return true;
  }
}
