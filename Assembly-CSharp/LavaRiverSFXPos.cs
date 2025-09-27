// Decompiled with JetBrains decompiler
// Type: LavaRiverSFXPos
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LavaRiverSFXPos : MonoBehaviour
{
  private void Update()
  {
    if (!(bool) (Object) MainCamera.instance)
      return;
    this.transform.position = new Vector3(MainCamera.instance.transform.position.x, this.transform.position.y, MainCamera.instance.transform.position.z);
    if ((double) this.transform.position.z >= 1050.0)
      return;
    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 1050f);
  }
}
