// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.ProceduralImage.ProceduralImage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine.Events;

#nullable disable
namespace UnityEngine.UI.ProceduralImage;

[ExecuteInEditMode]
[AddComponentMenu("UI/Procedural Image")]
public class ProceduralImage : Image
{
  [SerializeField]
  private float borderWidth;
  private ProceduralImageModifier modifier;
  private static Material materialInstance;
  [SerializeField]
  private float falloffDistance = 1f;

  private static Material DefaultProceduralImageMaterial
  {
    get
    {
      if ((UnityEngine.Object) UnityEngine.UI.ProceduralImage.ProceduralImage.materialInstance == (UnityEngine.Object) null)
        UnityEngine.UI.ProceduralImage.ProceduralImage.materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));
      return UnityEngine.UI.ProceduralImage.ProceduralImage.materialInstance;
    }
    set => UnityEngine.UI.ProceduralImage.ProceduralImage.materialInstance = value;
  }

  public float BorderWidth
  {
    get => this.borderWidth;
    set
    {
      this.borderWidth = value;
      this.SetVerticesDirty();
    }
  }

  public float FalloffDistance
  {
    get => this.falloffDistance;
    set
    {
      this.falloffDistance = value;
      this.SetVerticesDirty();
    }
  }

  protected ProceduralImageModifier Modifier
  {
    get
    {
      if ((UnityEngine.Object) this.modifier == (UnityEngine.Object) null)
      {
        this.modifier = this.GetComponent<ProceduralImageModifier>();
        if ((UnityEngine.Object) this.modifier == (UnityEngine.Object) null)
          this.ModifierType = typeof (FreeModifier);
      }
      return this.modifier;
    }
    set => this.modifier = value;
  }

  public System.Type ModifierType
  {
    get => this.Modifier.GetType();
    set
    {
      if ((UnityEngine.Object) this.modifier != (UnityEngine.Object) null && this.modifier.GetType() != value)
      {
        if ((UnityEngine.Object) this.GetComponent<ProceduralImageModifier>() != (UnityEngine.Object) null)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.GetComponent<ProceduralImageModifier>());
        this.gameObject.AddComponent(value);
        this.Modifier = this.GetComponent<ProceduralImageModifier>();
        this.SetAllDirty();
      }
      else
      {
        if (!((UnityEngine.Object) this.modifier == (UnityEngine.Object) null))
          return;
        this.gameObject.AddComponent(value);
        this.Modifier = this.GetComponent<ProceduralImageModifier>();
        this.SetAllDirty();
      }
    }
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    this.Init();
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    this.m_OnDirtyVertsCallback = this.m_OnDirtyVertsCallback - new UnityAction(this.OnVerticesDirty);
  }

  private void Init()
  {
    this.FixTexCoordsInCanvas();
    this.m_OnDirtyVertsCallback = this.m_OnDirtyVertsCallback + new UnityAction(this.OnVerticesDirty);
    this.preserveAspect = false;
    this.material = (Material) null;
    if (!((UnityEngine.Object) this.sprite == (UnityEngine.Object) null))
      return;
    this.sprite = EmptySprite.Get();
  }

  protected void OnVerticesDirty()
  {
    if (!((UnityEngine.Object) this.sprite == (UnityEngine.Object) null))
      return;
    this.sprite = EmptySprite.Get();
  }

  protected void FixTexCoordsInCanvas()
  {
    Canvas componentInParent = this.GetComponentInParent<Canvas>();
    if (!((UnityEngine.Object) componentInParent != (UnityEngine.Object) null))
      return;
    this.FixTexCoordsInCanvas(componentInParent);
  }

  protected void FixTexCoordsInCanvas(Canvas c)
  {
    c.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3;
  }

  private Vector4 FixRadius(Vector4 vec)
  {
    Rect rect = this.rectTransform.rect;
    vec = new Vector4(Mathf.Max(vec.x, 0.0f), Mathf.Max(vec.y, 0.0f), Mathf.Max(vec.z, 0.0f), Mathf.Max(vec.w, 0.0f));
    float num = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(rect.width / (vec.x + vec.y), rect.width / (vec.z + vec.w)), rect.height / (vec.x + vec.w)), rect.height / (vec.z + vec.y)), 1f);
    return vec * num;
  }

  protected override void OnPopulateMesh(VertexHelper toFill)
  {
    base.OnPopulateMesh(toFill);
    this.EncodeAllInfoIntoVertices(toFill, this.CalculateInfo());
  }

  protected override void OnTransformParentChanged()
  {
    base.OnTransformParentChanged();
    this.FixTexCoordsInCanvas();
  }

  private ProceduralImageInfo CalculateInfo()
  {
    Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
    float pixelSize = 1f / Mathf.Max(0.0f, this.falloffDistance);
    Vector4 vector4 = this.FixRadius(this.Modifier.CalculateRadius(pixelAdjustedRect));
    float num = Mathf.Min(pixelAdjustedRect.width, pixelAdjustedRect.height);
    return new ProceduralImageInfo(pixelAdjustedRect.width + this.falloffDistance, pixelAdjustedRect.height + this.falloffDistance, this.falloffDistance, pixelSize, vector4 / num, (float) ((double) this.borderWidth / (double) num * 2.0));
  }

  private void EncodeAllInfoIntoVertices(VertexHelper vh, ProceduralImageInfo info)
  {
    UIVertex vertex = new UIVertex();
    Vector2 vector2_1 = new Vector2(info.width, info.height);
    Vector2 vector2_2 = new Vector2(this.EncodeFloats_0_1_16_16(info.radius.x, info.radius.y), this.EncodeFloats_0_1_16_16(info.radius.z, info.radius.w));
    Vector2 vector2_3 = new Vector2((double) info.borderWidth == 0.0 ? 1f : Mathf.Clamp01(info.borderWidth), info.pixelSize);
    for (int i = 0; i < vh.currentVertCount; ++i)
    {
      vh.PopulateUIVertex(ref vertex, i);
      vertex.position += ((Vector3) vertex.uv0 - new Vector3(0.5f, 0.5f)) * info.fallOffDistance;
      vertex.uv1 = (Vector4) vector2_1;
      vertex.uv2 = (Vector4) vector2_2;
      vertex.uv3 = (Vector4) vector2_3;
      vh.SetUIVertex(vertex, i);
    }
  }

  private float EncodeFloats_0_1_16_16(float a, float b)
  {
    Vector2 rhs = new Vector2(1f, 1.52590219E-05f);
    return Vector2.Dot(new Vector2(Mathf.Floor(a * 65534f) / (float) ushort.MaxValue, Mathf.Floor(b * 65534f) / (float) ushort.MaxValue), rhs);
  }

  public override Material material
  {
    get
    {
      return (UnityEngine.Object) this.m_Material == (UnityEngine.Object) null ? UnityEngine.UI.ProceduralImage.ProceduralImage.DefaultProceduralImageMaterial : base.material;
    }
    set => base.material = value;
  }
}
