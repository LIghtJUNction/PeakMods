// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_Glowing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_Glowing : Affliction
{
  public GameObject pointLightPref;
  private GameObject pointLightInstance;

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.Glowing;
  }

  public override void OnApplied()
  {
    base.OnApplied();
    Material material = this.character.refs.mainRenderer.materials[0];
    float num = material.GetFloat("_Glow");
    Debug.Log((object) $"Appling Glow to character {this.character.gameObject.name}, amount {num}");
    material.SetFloat("_Glow", num + 1f);
    this.pointLightInstance = Object.Instantiate<GameObject>(this.pointLightPref, this.character.GetBodypart(BodypartType.Head).transform);
    this.pointLightInstance.transform.localPosition = Vector3.zero;
  }

  public override void OnRemoved()
  {
    base.OnRemoved();
    Material material = this.character.refs.mainRenderer.materials[0];
    float num = material.GetFloat("_Glow");
    Debug.Log((object) $"Removing Glow from character {this.character.gameObject.name}, amount {num}");
    material.SetFloat("_Glow", num - 1f);
    Object.DestroyImmediate((Object) this.pointLightInstance);
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
  }
}
