// Decompiled with JetBrains decompiler
// Type: ItemVFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ItemVFX : MonoBehaviour
{
  protected Item item;
  public bool shake;
  public float shakeAmount = 1f;
  public float castProgress;
  public AudioSource shakeSFX;
  public SFX_Instance[] doneSFX;

  protected virtual void Start()
  {
    this.item = this.GetComponent<Item>();
    if (!((Object) this.item.holderCharacter == (Object) null))
      return;
    this.enabled = false;
  }

  protected virtual void Update()
  {
    this.Shake();
    this.shakeSFX.volume = this.item.castProgress / 2f;
    this.shakeSFX.pitch = 1f + this.item.castProgress;
  }

  protected virtual void Shake()
  {
    if (!this.item.finishedCast)
      GamefeelHandler.instance.AddPerlinShake((float) ((double) this.item.castProgress * (double) this.shakeAmount * (double) Time.deltaTime * 60.0));
    if (this.item.finishedCast)
    {
      for (int index = 0; index < this.doneSFX.Length; ++index)
        this.doneSFX[index].Play(this.transform.position);
    }
    this.castProgress = this.item.castProgress;
  }
}
