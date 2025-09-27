// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.ProceduralImage.ProceduralImageModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.UI.ProceduralImage;

[DisallowMultipleComponent]
public abstract class ProceduralImageModifier : MonoBehaviour
{
  protected Graphic graphic;

  protected Graphic _Graphic
  {
    get
    {
      if ((Object) this.graphic == (Object) null)
        this.graphic = this.GetComponent<Graphic>();
      return this.graphic;
    }
  }

  public abstract Vector4 CalculateRadius(Rect imageRect);
}
