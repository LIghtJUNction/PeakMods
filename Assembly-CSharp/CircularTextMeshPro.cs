// Decompiled with JetBrains decompiler
// Type: CircularTextMeshPro
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (TextMeshProUGUI))]
public class CircularTextMeshPro : MonoBehaviour
{
  private TextMeshProUGUI m_TextComponent;
  [SerializeField]
  [HideInInspector]
  private float m_radius = 10f;

  [Tooltip("The radius of the text circle arc")]
  public float Radius
  {
    get => this.m_radius;
    set
    {
      this.m_radius = value;
      this.OnCurvePropertyChanged();
    }
  }

  private void Awake() => this.m_TextComponent = this.gameObject.GetComponent<TextMeshProUGUI>();

  private void OnEnable()
  {
    this.m_TextComponent.OnPreRenderText += new Action<TMP_TextInfo>(this.UpdateTextCurve);
    this.OnCurvePropertyChanged();
  }

  private void OnDisable()
  {
    this.m_TextComponent.OnPreRenderText -= new Action<TMP_TextInfo>(this.UpdateTextCurve);
  }

  protected void OnCurvePropertyChanged()
  {
    this.UpdateTextCurve(this.m_TextComponent.textInfo);
    this.m_TextComponent.ForceMeshUpdate(false, false);
  }

  protected void UpdateTextCurve(TMP_TextInfo textInfo)
  {
    for (int charIdx = 0; charIdx < textInfo.characterInfo.Length; ++charIdx)
    {
      if (textInfo.characterInfo[charIdx].isVisible)
      {
        int vertexIndex = textInfo.characterInfo[charIdx].vertexIndex;
        int materialReferenceIndex = textInfo.characterInfo[charIdx].materialReferenceIndex;
        Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
        Vector3 charMidBaselinePos = (Vector3) new Vector2((float) (((double) vertices[vertexIndex].x + (double) vertices[vertexIndex + 2].x) / 2.0), textInfo.characterInfo[charIdx].baseLine);
        vertices[vertexIndex] += -charMidBaselinePos;
        vertices[vertexIndex + 1] += -charMidBaselinePos;
        vertices[vertexIndex + 2] += -charMidBaselinePos;
        vertices[vertexIndex + 3] += -charMidBaselinePos;
        Matrix4x4 transformationMatrix = this.ComputeTransformationMatrix(charMidBaselinePos, textInfo, charIdx);
        vertices[vertexIndex] = transformationMatrix.MultiplyPoint3x4(vertices[vertexIndex]);
        vertices[vertexIndex + 1] = transformationMatrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
        vertices[vertexIndex + 2] = transformationMatrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
        vertices[vertexIndex + 3] = transformationMatrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
      }
    }
  }

  protected Matrix4x4 ComputeTransformationMatrix(
    Vector3 charMidBaselinePos,
    TMP_TextInfo textInfo,
    int charIdx)
  {
    float num1 = this.m_radius + textInfo.lineInfo[textInfo.characterInfo[charIdx].lineNumber].baseline;
    float num2 = (float) (2.0 * (double) num1 * 3.1415927410125732);
    double f = (((double) charMidBaselinePos.x / (double) num2 - 0.5) * 360.0 + 90.0) * (Math.PI / 180.0);
    float x = Mathf.Cos((float) f);
    float y = Mathf.Sin((float) f);
    Vector2 vector2 = new Vector2(x * num1, -y * num1);
    float angle = (float) (-(double) Mathf.Atan2(y, x) * 57.295780181884766 - 90.0);
    return Matrix4x4.TRS(new Vector3(vector2.x, vector2.y, 0.0f), Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
  }
}
