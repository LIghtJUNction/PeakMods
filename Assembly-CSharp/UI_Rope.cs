// Decompiled with JetBrains decompiler
// Type: UI_Rope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class UI_Rope : MonoBehaviour
{
  public RectTransform rope;
  public float maxRopeLength = 40f;
  public float ropeLength = 40f;
  public float ropeLengthOffset;
  public float ropeLengthMult = 20f;
  public float ropeLengthAlphaMult;
  public Image[] ropeImages;
  private const string M = "m";
  private const string FT = "ft";
  public TextMeshProUGUI ropeLengthText;
  private int segments;
  public Transform ropeEnd;
  public Image ropeEndImage;
  public float ropeSpinA = 2f;
  public float ropeSpinB = 3f;
  public float ropeEndOffset;

  private void OnEnable()
  {
    this.segments = 1;
    this.ropeLength = 1f;
  }

  private void Update()
  {
    this.ropeLength = Mathf.Lerp(this.ropeLength, (float) this.segments, Time.deltaTime * 5f);
    float x = (Mathf.Max(this.ropeLength, 0.0f) + this.ropeLengthOffset) * this.ropeLengthMult;
    this.rope.sizeDelta = new Vector2(x, this.rope.sizeDelta.y);
    for (int index = 0; index < this.ropeImages.Length; ++index)
      this.ropeImages[index].color = new Color(this.ropeImages[index].color.r, this.ropeImages[index].color.g, this.ropeImages[index].color.b, (float) ((double) x * (double) this.ropeLengthAlphaMult - (double) Mathf.Floor(x * this.ropeLengthAlphaMult) + 0.0099999997764825821));
    bool flag = false;
    for (int index = 0; index < 3; ++index)
    {
      this.ropeImages[index].fillAmount = this.ropeSpinA - (this.ropeLength * this.ropeSpinB / this.maxRopeLength - (float) index);
      if ((double) this.ropeImages[index].fillAmount > 0.0 && !flag)
      {
        flag = true;
        this.ropeEnd.position = this.ropeImages[index].transform.position;
        this.ropeEnd.eulerAngles = this.ropeImages[index].transform.eulerAngles + new Vector3(0.0f, 0.0f, this.ropeImages[index].fillAmount * 360f + this.ropeEndOffset);
        this.ropeEndImage.color = new Color(this.ropeImages[index].color.r, this.ropeImages[index].color.g, this.ropeImages[index].color.b, 1f);
      }
    }
    string str = "m";
    int num = Mathf.RoundToInt((float) ((double) this.ropeLength * 100.0 * 0.25));
    this.ropeLengthText.text = $"{(num / 100).ToString()}.{(num % 100).ToString()}{str}";
  }

  public void UpdateRope(int newSegments) => this.segments = newSegments;
}
