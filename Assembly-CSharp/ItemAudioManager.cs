// Decompiled with JetBrains decompiler
// Type: ItemAudioManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using WebSocketSharp;

#nullable disable
public class ItemAudioManager : MonoBehaviour
{
  private string prevUse;
  private Item prevItem;
  public AudioSource throwCharge;
  private Character character;
  [HideInInspector]
  public float finishTimer;
  private float increase;
  public SFX_Instance[] switchGeneric;

  private void Start() => this.character = this.transform.root.GetComponent<Character>();

  private void Update()
  {
    this.character.refs.animator.SetBool("Eat", false);
    this.character.refs.animator.SetBool("Heal", false);
    this.character.refs.animator.SetBool("Drink", false);
    this.character.refs.animator.SetBool("Antidote", false);
    this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0.0f, Time.deltaTime * 5f);
    this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 1f, Time.deltaTime * 5f);
    if (!string.IsNullOrEmpty(this.prevUse) && !this.prevUse.IsNullOrEmpty())
      this.character.refs.animator.SetBool(this.prevUse, false);
    if (!(bool) (Object) this.character.data.currentItem && !string.IsNullOrEmpty(this.prevUse))
      this.character.refs.animator.SetBool(this.prevUse, false);
    if (this.character.refs.animator.GetBool("Consumed Item"))
      this.finishTimer -= Time.deltaTime;
    else
      this.finishTimer = 0.25f;
    if ((double) this.finishTimer <= 0.0)
      this.character.refs.animator.SetBool("Consumed Item", false);
    if ((bool) (Object) this.character.data.currentItem)
    {
      if ((double) this.character.refs.items.throwChargeLevel > 0.0)
      {
        this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0.3f, Time.deltaTime * 10f);
        this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, (float) (2.0 + (double) this.character.refs.items.throwChargeLevel * 3.0), Time.deltaTime * 10f);
      }
      if ((Object) this.prevItem != (Object) this.character.data.currentItem)
      {
        for (int index = 0; index < this.switchGeneric.Length; ++index)
          this.switchGeneric[index].Play(this.transform.position);
      }
      if ((bool) (Object) this.character.data.currentItem.GetComponent<ItemUseFeedback>() && (bool) (Object) this.character.data.currentItem)
      {
        if ((Object) this.prevItem != (Object) this.character.data.currentItem)
        {
          for (int index = 0; index < this.character.data.currentItem.GetComponent<ItemUseFeedback>().equip.Length; ++index)
            this.character.data.currentItem.GetComponent<ItemUseFeedback>().equip[index].Play(this.transform.position);
        }
        string useAnimation = this.character.data.currentItem.GetComponent<ItemUseFeedback>().useAnimation;
        if (!string.IsNullOrEmpty(useAnimation))
        {
          if (this.character.data.currentItem.isUsingPrimary && (double) this.character.data.currentItem.castProgress < 1.0)
            this.character.refs.animator.SetBool(useAnimation, true);
          else
            this.character.refs.animator.SetBool(useAnimation, false);
        }
        this.prevUse = useAnimation;
      }
    }
    if ((bool) (Object) this.prevItem && !(bool) (Object) this.character.data.currentItem)
    {
      for (int index = 0; index < this.switchGeneric.Length; ++index)
        this.switchGeneric[index].Play(this.transform.position);
    }
    this.prevItem = this.character.data.currentItem;
  }
}
