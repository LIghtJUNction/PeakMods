// Decompiled with JetBrains decompiler
// Type: SpawnedVine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System.Collections;
using UnityEngine;

#nullable disable
public class SpawnedVine : MonoBehaviour
{
  private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");
  private JungleVine vine;
  public MeshRenderer vineRenderer;
  public float vineWaveDecay = 0.5f;
  public GameObject startObject;
  public GameObject endObject;

  private void Start()
  {
    this.vine = this.GetComponent<JungleVine>();
    this.SpawnVine();
  }

  public void SpawnVine()
  {
    if ((Object) this.startObject != (Object) null)
    {
      this.startObject.transform.position = this.vine.colliderRoot.GetChild(0).transform.position;
      Vector3 worldPosition = this.vine.GetPosition(1f);
      worldPosition = new Vector3(worldPosition.x, this.startObject.transform.position.y, worldPosition.z);
      this.startObject.transform.LookAt(worldPosition);
      if ((Object) this.endObject != (Object) null)
        this.endObject.transform.forward = this.vine.colliderRoot.GetLastChild().transform.up;
    }
    if ((Object) this.endObject != (Object) null)
      this.endObject.transform.position = this.vine.GetPosition(1f);
    this.StartCoroutine(waveFX());

    IEnumerator waveFX()
    {
      float normalizedTime = 0.0f;
      while ((double) normalizedTime < 1.0)
      {
        normalizedTime += Time.deltaTime / this.vineWaveDecay;
        float num = Mathf.Lerp(100f, 0.0f, normalizedTime);
        this.vineRenderer.material.SetFloat(SpawnedVine.JitterAmount, num);
        yield return (object) null;
      }
    }
  }

  private void Update()
  {
  }
}
