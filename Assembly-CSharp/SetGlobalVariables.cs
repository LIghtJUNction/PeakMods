// Decompiled with JetBrains decompiler
// Type: SetGlobalVariables
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SetGlobalVariables : MonoBehaviour
{
  public StringAndFloat[] globalVariables;

  private void Start()
  {
    foreach (StringAndFloat globalVariable in this.globalVariables)
      PlayerPrefs.SetFloat(globalVariable.name, globalVariable.value);
  }
}
