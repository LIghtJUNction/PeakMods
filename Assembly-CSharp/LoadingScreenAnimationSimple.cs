// Decompiled with JetBrains decompiler
// Type: LoadingScreenAnimationSimple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using TMPro;
using UnityEngine;

#nullable disable
public class LoadingScreenAnimationSimple : MonoBehaviour
{
  public float yieldTime = 1f;
  public TMP_Text loading;

  private void Start() => this.StartCoroutine(this.AnimateRoutine());

  private IEnumerator AnimateRoutine()
  {
    float dots = 0.0f;
    while (true)
    {
      do
      {
        yield return (object) new WaitForSeconds(this.yieldTime);
        if ((double) dots == 0.0)
          this.loading.text = LocalizedText.GetText("LOADING");
        else if ((double) dots == 1.0)
          this.loading.text = LocalizedText.GetText("LOADING") + ".";
        else if ((double) dots == 2.0)
          this.loading.text = LocalizedText.GetText("LOADING") + "..";
        else if ((double) dots == 3.0)
          this.loading.text = LocalizedText.GetText("LOADING") + "...";
        ++dots;
      }
      while ((double) dots <= 3.0);
      dots = 0.0f;
    }
  }
}
