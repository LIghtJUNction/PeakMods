// Decompiled with JetBrains decompiler
// Type: SpecialDayZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class SpecialDayZone : MonoBehaviour
{
  public bool overrideSun;
  [FormerlySerializedAs("lightIntensity")]
  public float daylLightIntensity = 1.5f;
  public float nightLightIntensity = 1.5f;
  public Color specialSunColor;
  public bool useCustomSun;
  public Light specialLight;
  public bool useCustomColorVals = true;
  public Color specialTopColor;
  public Color specialMidColor;
  public Color specialBottomColor;
  [FormerlySerializedAs("shaderValsToBlend")]
  public ShaderVal[] globalShaderVals;
  private float baseFog;
  public bool overrideFog = true;
  public float fogDensity = 400f;
  public Bounds bounds;
  public Bounds outerBounds;
  public float blendSize = 50f;
  [Header("Debug")]
  public bool inBounds;

  private void Start()
  {
    this.bounds.center = this.transform.position;
    this.outerBounds.center = this.transform.position;
    this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
    if ((bool) (Object) this.specialLight)
      this.specialLight.color = Color.black;
    if (!(bool) (Object) this.specialLight)
      return;
    this.specialLight.enabled = false;
  }

  private void OnDrawGizmosSelected()
  {
    this.bounds.center = this.transform.position;
    this.outerBounds.center = this.transform.position;
    Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
    Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x / 2f, this.bounds.size.y, this.bounds.size.z));
    Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y / 2f, this.bounds.size.z));
    Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y, this.bounds.size.z / 2f));
    Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
    this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
    Gizmos.DrawWireCube(this.outerBounds.center, this.outerBounds.size);
  }

  private void Update()
  {
    if (!(bool) (Object) Character.localCharacter)
      return;
    if (this.outerBounds.Contains(Character.localCharacter.Center))
    {
      if ((bool) (Object) this.specialLight)
        this.specialLight.enabled = true;
      this.inBounds = true;
    }
    else
    {
      if ((bool) (Object) this.specialLight)
        this.specialLight.enabled = false;
      this.inBounds = false;
    }
  }

  private void OnDisable()
  {
    if ((bool) (Object) this.specialLight)
      this.specialLight.color = Color.black;
    this.specialLight.enabled = false;
  }
}
