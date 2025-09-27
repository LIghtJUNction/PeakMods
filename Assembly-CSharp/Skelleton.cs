// Decompiled with JetBrains decompiler
// Type: Skelleton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Skelleton : MonoBehaviour
{
  public void SpawnSkelly(Character target)
  {
    foreach (Bodypart componentsInChild in this.transform.GetComponentsInChildren<Bodypart>())
    {
      Bodypart bodypart = target.GetBodypart(componentsInChild.partType);
      if (!((Object) bodypart == (Object) null))
      {
        componentsInChild.transform.position = bodypart.transform.position;
        componentsInChild.transform.rotation = bodypart.transform.rotation;
      }
    }
  }
}
