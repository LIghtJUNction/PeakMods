// Decompiled with JetBrains decompiler
// Type: RigCreatorCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class RigCreatorCollider : MonoBehaviour
{
  internal CapsuleCollider col;
  internal Vector3 position;
  internal Quaternion rotation;
  internal Vector3 scale;
  internal float height;
  internal float radius;
  public bool disableOnStart;
  internal global::RigCreator rigCreator;

  private void Start()
  {
    if (this.disableOnStart)
      this.Col().enabled = false;
    else
      this.GetComponentInParent<CharacterRagdoll>().colliderList.Add((Collider) this.Col());
  }

  private void Awake()
  {
    if (this.IsEditor())
    {
      this.SetValues();
    }
    else
    {
      this.RegisterCollider();
      this.Col();
    }
  }

  private void RegisterCollider()
  {
    this.transform.parent.GetComponent<Bodypart>().RegisterCollider(this);
  }

  private bool IsEditor() => Application.isEditor && !Application.isPlaying;

  private void OnDestroy()
  {
    if (!this.IsEditor() || !(bool) (Object) this.RigCreator())
      return;
    this.RigCreator().RemoveCollider(this);
  }

  private CapsuleCollider Col()
  {
    if (!(bool) (Object) this.col)
      this.col = this.GetComponent<CapsuleCollider>();
    return this.col;
  }

  private global::RigCreator RigCreator()
  {
    if (!(bool) (Object) this.rigCreator)
      this.rigCreator = this.GetComponentInParent<global::RigCreator>();
    return this.rigCreator;
  }

  private void Update()
  {
    if (!this.IsEditor())
      return;
    this.CheckEditorDataChanged();
  }

  private void CheckEditorDataChanged()
  {
    if (!(this.position != this.transform.localPosition) && !(this.rotation != this.transform.localRotation) && !(this.scale != this.transform.localScale) && (double) this.height == (double) this.Col().height && (double) this.radius == (double) this.Col().radius)
      return;
    this.RigCreator().ColliderChanged(this, this.transform.localPosition, this.transform.localRotation, this.transform.localScale, this.height, this.radius);
    this.SetValues();
  }

  private void SetValues()
  {
    this.position = this.transform.localPosition;
    this.rotation = this.transform.localRotation;
    this.scale = this.transform.localScale;
    this.height = this.Col().height;
    this.radius = this.Col().radius;
  }
}
