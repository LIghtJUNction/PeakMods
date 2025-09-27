// Decompiled with JetBrains decompiler
// Type: LoadingScreenAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
[ExecuteAlways]
public class LoadingScreenAnimation : MonoBehaviour
{
  public Image barFill;
  public Transform planeRotation;
  public TMP_Text loadingText;
  [Range(0.0f, 1f)]
  public float fillAmount;
  public Vector2 barFillMinMax;
  public Vector2 planeRotationMinMax;
  private string loadingString;
  private float defaultLoadingStringLength = 50f;
  public float maxFill;

  private void Start()
  {
    string text = LocalizedText.GetText("LOADING");
    switch (LocalizedText.CURRENT_LANGUAGE)
    {
      case LocalizedText.Language.SimplifiedChinese:
        this.loadingString = $"{text}...{text}...{text}...{text}...";
        this.defaultLoadingStringLength = (float) this.loadingString.Length;
        break;
      case LocalizedText.Language.Japanese:
      case LocalizedText.Language.Korean:
        this.loadingString = $"{text}...{text}...{text}...";
        this.defaultLoadingStringLength = (float) this.loadingString.Length;
        break;
      default:
        this.loadingString = $"{text}...{text}...{text}...{text}...{text}...";
        this.defaultLoadingStringLength = 50f;
        break;
    }
  }

  private void Update()
  {
    this.barFill.fillAmount = Mathf.Lerp(this.barFillMinMax.x, this.barFillMinMax.y, this.fillAmount);
    this.planeRotation.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.planeRotationMinMax.x, this.planeRotationMinMax.y, this.fillAmount));
    this.loadingText.text = this.loadingString.Substring(0, Mathf.RoundToInt(this.defaultLoadingStringLength * this.fillAmount));
  }
}
