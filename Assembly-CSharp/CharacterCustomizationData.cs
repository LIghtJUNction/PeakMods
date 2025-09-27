// Decompiled with JetBrains decompiler
// Type: CharacterCustomizationData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
[Serializable]
public class CharacterCustomizationData : IBinarySerializable
{
  [SerializeField]
  public int currentSkin;
  [SerializeField]
  public int currentAccessory;
  [SerializeField]
  public int currentEyes;
  [SerializeField]
  public int currentMouth;
  [FormerlySerializedAs("currentFit")]
  [SerializeField]
  public int currentOutfit;
  [SerializeField]
  public int currentHat;
  [SerializeField]
  public int currentSash;

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteInt(this.currentSkin);
    serializer.WriteInt(this.currentAccessory);
    serializer.WriteInt(this.currentEyes);
    serializer.WriteInt(this.currentMouth);
    serializer.WriteInt(this.currentOutfit);
    serializer.WriteInt(this.currentHat);
    serializer.WriteInt(this.currentSash);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.currentSkin = deserializer.ReadInt();
    this.currentAccessory = deserializer.ReadInt();
    this.currentEyes = deserializer.ReadInt();
    this.currentMouth = deserializer.ReadInt();
    this.currentOutfit = deserializer.ReadInt();
    this.currentHat = deserializer.ReadInt();
    this.currentSash = deserializer.ReadInt();
    this.CorrectValues();
  }

  public void CorrectValues()
  {
    if (!(bool) (UnityEngine.Object) Singleton<Customization>.Instance)
      return;
    if (this.currentSkin >= Singleton<Customization>.Instance.skins.Length)
      this.currentSkin = 0;
    if (this.currentEyes >= Singleton<Customization>.Instance.eyes.Length)
      this.currentEyes = 0;
    if (this.currentMouth >= Singleton<Customization>.Instance.mouths.Length)
      this.currentMouth = 0;
    if (this.currentAccessory >= Singleton<Customization>.Instance.accessories.Length)
      this.currentAccessory = 0;
    if (this.currentOutfit >= Singleton<Customization>.Instance.fits.Length)
      this.currentOutfit = 0;
    if (this.currentHat >= Singleton<Customization>.Instance.hats.Length)
      this.currentHat = 0;
    if (this.currentSash < Singleton<Customization>.Instance.sashes.Length)
      return;
    this.currentSash = Singleton<Customization>.Instance.sashes.Length - 1;
  }
}
