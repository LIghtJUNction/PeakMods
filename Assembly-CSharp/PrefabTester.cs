// Decompiled with JetBrains decompiler
// Type: PrefabTester
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PrefabTester : MonoBehaviour
{
  public GameObject prefab;
  public GameObject instance;

  private void Awake() => this.instance = this.transform.GetChild(0).gameObject;

  public void Update()
  {
    if (!Input.GetKeyDown(KeyCode.T))
      return;
    if ((Object) this.instance != (Object) null)
      Object.Destroy((Object) this.instance);
    this.instance = Object.Instantiate<GameObject>(this.prefab, this.transform.position, this.transform.rotation);
  }
}
