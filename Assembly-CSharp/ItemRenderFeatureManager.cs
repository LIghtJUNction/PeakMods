// Decompiled with JetBrains decompiler
// Type: ItemRenderFeatureManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

#nullable disable
public class ItemRenderFeatureManager : MonoBehaviour
{
  private ScriptableRendererFeature rendererFeature;
  [FormerlySerializedAs("falseOnStart")]
  public bool disabledOnStart;
  public UniversalRendererData rend;
  public string featureName;

  private void Start()
  {
    this.getRendererFeature();
    if (!this.disabledOnStart)
      return;
    this.setFeatureActive(false);
  }

  private void getRendererFeature()
  {
    foreach (ScriptableRendererFeature rendererFeature in this.rend.rendererFeatures)
    {
      if (rendererFeature.name == this.featureName)
        this.rendererFeature = rendererFeature;
    }
  }

  public void setFeatureActive(bool active)
  {
    this.rendererFeature.SetActive(active);
    MonoBehaviour.print((object) active);
    int num = (Object) this.rendererFeature != (Object) null ? 1 : 0;
  }

  private void OnDisable() => this.setFeatureActive(false);

  private void Update()
  {
  }
}
