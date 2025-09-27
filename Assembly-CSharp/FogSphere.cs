// Decompiled with JetBrains decompiler
// Type: FogSphere
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteAlways]
public class FogSphere : MonoBehaviour
{
  public float currentSize = 50f;
  [Range(0.0f, 1f)]
  public float ENABLE = 1f;
  [Range(0.0f, 1f)]
  public float REVEAL_AMOUNT;
  public float PADDING = 300f;
  public Vector3 fogPoint;
  private float ratio = 2f;
  private Renderer rend;
  public SFX_Instance[] fogStart;
  private bool t;
  public SFX_Instance[] fogReveal;
  private bool t2;
  private MaterialPropertyBlock mpb;

  private void Start() => this.rend = this.GetComponent<Renderer>();

  private void OnDisable()
  {
    Shader.SetGlobalFloat("FogEnabled", 0.0f);
    Shader.SetGlobalFloat("_FogSphereSize", 9999999f);
  }

  private void Update()
  {
    this.SetSize();
    this.SetSharderVars();
    if ((double) this.currentSize > 120.0)
      this.t = false;
    if (!this.t && (double) this.currentSize < 120.0)
    {
      this.t = true;
      for (int index = 0; index < this.fogStart.Length; ++index)
        this.fogStart[index].Play();
    }
    if (!this.t2 && (double) this.REVEAL_AMOUNT > 0.10000000149011612)
    {
      this.t2 = true;
      for (int index = 0; index < this.fogReveal.Length; ++index)
        this.fogReveal[index].Play();
    }
    if ((double) this.REVEAL_AMOUNT >= 0.10000000149011612)
      return;
    this.t2 = false;
  }

  private void SetSharderVars()
  {
    if (this.mpb == null)
      this.mpb = new MaterialPropertyBlock();
    this.rend.GetPropertyBlock(this.mpb);
    this.mpb.SetFloat("_PADDING", this.PADDING);
    this.mpb.SetFloat("_FogDepth", this.currentSize);
    this.mpb.SetFloat("_RevealAmount", this.REVEAL_AMOUNT);
    this.mpb.SetVector("_FogCenter", (Vector4) this.fogPoint);
    Shader.SetGlobalFloat("_FogSphereSize", this.currentSize);
    Shader.SetGlobalVector("FogCenter", (Vector4) this.fogPoint);
    Shader.SetGlobalFloat("FogEnabled", this.ENABLE);
    if (Application.isPlaying && (Object) Character.localCharacter != (Object) null)
    {
      Character.localCharacter.data.isInFog = false;
      if (Mathf.Approximately(this.ENABLE, 1f) && (Object) Character.localCharacter != (Object) null && (double) Vector3.Distance(this.fogPoint, Character.localCharacter.Center) > (double) this.currentSize)
      {
        Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.0105000008f * Time.deltaTime);
        Character.localCharacter.data.isInFog = true;
      }
    }
    this.rend.SetPropertyBlock(this.mpb);
  }

  private void SetSize()
  {
    this.transform.localScale = Vector3.one * ((this.currentSize + this.PADDING) * this.ratio);
  }

  private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(this.fogPoint, this.currentSize);
}
