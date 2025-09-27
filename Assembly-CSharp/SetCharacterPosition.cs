// Decompiled with JetBrains decompiler
// Type: SetCharacterPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class SetCharacterPosition : MonoBehaviour
{
  public GameObject characterPrefab;

  private void Go()
  {
    this.characterPrefab.transform.position = this.transform.position;
    PExt.SaveObj((Object) this.characterPrefab);
  }

  private void Start()
  {
  }

  private void Update()
  {
  }
}
