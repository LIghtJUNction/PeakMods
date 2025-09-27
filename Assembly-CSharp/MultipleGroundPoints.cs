// Decompiled with JetBrains decompiler
// Type: MultipleGroundPoints
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MultipleGroundPoints : CustomSpawnCondition
{
  public HelperFunctions.LayerType layerType;
  public float maxAngle = 30f;
  public float checkRange = 5f;
  public float checkHeight;

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    Transform transform = this.transform.Find("GroundPoints");
    for (int index = 0; index < transform.childCount; ++index)
    {
      Transform child = transform.GetChild(index);
      RaycastHit raycastHit = HelperFunctions.LineCheck(child.position + Vector3.up * this.checkHeight, child.position + Vector3.down * this.checkRange, this.layerType);
      if (!(bool) (Object) raycastHit.transform || (double) Vector3.Angle(Vector3.up, raycastHit.normal) > (double) this.maxAngle)
        return false;
    }
    return true;
  }
}
