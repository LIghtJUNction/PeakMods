// Decompiled with JetBrains decompiler
// Type: PSM_BakedVolumeLightModiferIntensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_BakedVolumeLightModiferIntensity : PropSpawnerMod
{
  public bool customColor;
  public Color color = new Color(0.86f, 0.56f, 0.04f, 0.87f);
  public bool customIntensity;
  public float intensity = 0.5f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    BakedVolumeLight component = spawned.GetComponent<BakedVolumeLight>();
    if (!(bool) (Object) component)
      return;
    if (this.customIntensity)
      component.intensity = this.intensity;
    if (!this.customColor)
      return;
    component.color = this.color;
  }
}
