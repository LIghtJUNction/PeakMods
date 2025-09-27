// Decompiled with JetBrains decompiler
// Type: PointPing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class PointPing : MonoBehaviour
{
  public float sizeOfFrustum = 0.1f;
  public Vector2 minMaxScale = new Vector2(0.2f, 3f);
  public Vector2 visibilityFullNoneNoLos = new Vector2(30f, 50f);
  public float NoLosVisibilityMul = 0.5f;
  public float angleThing = 90f;
  public MeshRenderer renderer;
  public SpriteRenderer ringRenderer;
  public SFX_Instance pingSound;
  public Material material;
  public Vector3 hitNormal;
  public PointPinger pointPinger;
  private Camera camera;
  private Character character;

  public Vector3 PingerForward
  {
    get => (this.transform.position - this.pointPinger.character.Head).normalized;
  }

  public void Init(Character character) => this.character = character;

  private void Awake() => this.material = this.renderer.material;

  private void Start()
  {
    this.camera = Camera.main;
    this.Go();
    this.pingSound.Play(this.transform.position);
  }

  public void Update() => this.Go();

  private void Go()
  {
    float num = this.camera.SizeOfFrustumAtDistance(Vector3.Distance(Character.localCharacter.Center, this.transform.position));
    this.character.GetComponent<CharacterAnimations>().point = this.gameObject;
    this.transform.localScale = (this.minMaxScale.PClampFloat(num) * this.sizeOfFrustum).xxx();
    Vector3 to = this.transform.position - this.camera.transform.position;
    Vector3 vector3 = Vector3.Lerp(-this.hitNormal, this.PingerForward, Vector3.Angle(this.PingerForward, to).Remap(0.0f, this.angleThing, 0.0f, 1f));
    float me = Vector3.Angle(vector3, to);
    this.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(-Vector3.up, vector3, me.Remap(0.0f, this.angleThing, 0.0f, 1f)), Vector3.up);
  }
}
