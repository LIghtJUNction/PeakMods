// Decompiled with JetBrains decompiler
// Type: FootStepPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FootStepPlayer : MonoBehaviour
{
  private Character character;
  public LayerMask floorLayer;
  public StepSoundCollection surfaceLookup;
  private int doStep;
  private bool t;
  public Material beachSand;
  public Material beachRock;
  public Material desertSand;
  public Material[] jungleGrass;
  public Material jungleRock;
  public Material iceSnow;
  public Material iceRock;
  public Material volcanoRock;
  public Material[] metal;
  public Material[] wood;
  private RaycastHit hit;
  public GameObject onGround;
  public GameObject offGround;
  public AmbienceAudio ambience;
  private bool bingBongRoom;
  private float timer;

  private void Start() => this.character = this.transform.root.GetComponent<Character>();

  private void Update()
  {
    this.doStep = 0;
    foreach (Component component in this.transform)
    {
      if (component.gameObject.activeSelf)
        ++this.doStep;
    }
    if (this.doStep == 0)
      this.t = false;
    if (this.doStep > 0 && !this.t)
      this.PlayStep();
    if ((double) this.character.data.sinceGrounded <= 0.0 && !this.onGround.active)
    {
      this.onGround.SetActive(true);
      this.offGround.SetActive(false);
      this.PlayStep();
    }
    if ((double) this.character.data.sinceGrounded > 0.25 && !this.offGround.active)
    {
      this.offGround.SetActive(true);
      this.onGround.SetActive(false);
      this.PlayStep();
    }
    if (!this.bingBongRoom)
      return;
    this.timer += Time.deltaTime;
    if ((double) this.timer <= 4.0)
      return;
    this.ambience.bingBongStatue.gameObject.SetActive(true);
  }

  private void PlayStep()
  {
    if (Physics.Linecast(this.transform.position, this.transform.position + Vector3.down * 100f, out this.hit, (int) this.floorLayer))
    {
      MeshRenderer component = this.hit.collider.GetComponent<MeshRenderer>();
      if (this.hit.collider.name == "BigRoom")
        this.bingBongRoom = true;
      if ((bool) (Object) component)
      {
        if (component.material.name == this.beachSand.name + " (Instance)")
        {
          this.surfaceLookup.PlayStep(this.transform.position, 1);
          this.t = true;
          return;
        }
        if (component.material.name == this.beachRock.name + " (Instance)")
        {
          this.surfaceLookup.PlayStep(this.transform.position, 2);
          this.t = true;
          return;
        }
        if (component.material.name == this.desertSand.name + " (Instance)")
        {
          this.surfaceLookup.PlayStep(this.transform.position, 1);
          this.t = true;
          return;
        }
        if (component.material.name == this.desertSand.name)
        {
          this.surfaceLookup.PlayStep(this.transform.position, 1);
          this.t = true;
          return;
        }
        foreach (Material material in this.jungleGrass)
        {
          if (component.material.name == material.name + " (Instance)")
          {
            if (!this.t)
              this.surfaceLookup.PlayStep(this.transform.position, 3);
            this.t = true;
          }
        }
        if (component.material.name == this.jungleRock.name + " (Instance)")
        {
          this.surfaceLookup.PlayStep(this.transform.position, 4);
          this.t = true;
          return;
        }
        if (component.material.name == this.iceRock.name + " (Instance)")
        {
          if ((bool) (Object) this.ambience)
            this.ambience.naturelessTerrain = 30f;
          this.surfaceLookup.PlayStep(this.transform.position, 5);
          this.t = true;
          return;
        }
        if (component.material.name == this.iceSnow.name + " (Instance)")
        {
          if ((bool) (Object) this.ambience)
            this.ambience.naturelessTerrain = 30f;
          this.surfaceLookup.PlayStep(this.transform.position, 6);
          this.t = true;
          return;
        }
        if (component.material.name == this.volcanoRock.name + " (Instance)")
        {
          if ((bool) (Object) this.ambience)
          {
            this.ambience.naturelessTerrain = 30f;
            this.ambience.vulcanoT = 10f;
          }
          this.surfaceLookup.PlayStep(this.transform.position, 9);
          this.t = true;
          return;
        }
        foreach (Material material in this.metal)
        {
          if (component.material.name == material.name + " (Instance)")
          {
            if (!this.t)
              this.surfaceLookup.PlayStep(this.transform.position, 7);
            this.t = true;
          }
        }
        foreach (Material material in this.wood)
        {
          if (component.material.name == material.name + " (Instance)")
          {
            if (!this.t)
              this.surfaceLookup.PlayStep(this.transform.position, 8);
            this.t = true;
          }
          if (component.material.name == material.name + " (Instance) (Instance)")
          {
            if (!this.t)
              this.surfaceLookup.PlayStep(this.transform.position, 8);
            this.t = true;
          }
        }
        if (!this.t)
        {
          this.surfaceLookup.PlayStep(this.transform.position, 0);
          this.t = true;
        }
      }
      else
      {
        this.surfaceLookup.PlayStep(this.transform.position, 0);
        this.t = true;
      }
    }
    else
    {
      this.surfaceLookup.PlayStep(this.transform.position, 0);
      this.t = true;
    }
    this.t = true;
  }
}
