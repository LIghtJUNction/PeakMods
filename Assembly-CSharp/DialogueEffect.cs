// Decompiled with JetBrains decompiler
// Type: DialogueEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using TMPro;
using UnityEngine;

#nullable disable
public class DialogueEffect : MonoBehaviour
{
  protected TMP_Text m_TextComponent;
  protected TMP_TextInfo textInfo;
  public DOTweenTMPAnimator DTanimator;
  private bool destroyed;

  private void Awake()
  {
    this.m_TextComponent = this.GetComponent<TMP_Text>();
    this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
  }

  private void Start() => this.Init();

  private void OnEnable()
  {
  }

  private void OnDisable() => this.TryDestroy();

  public virtual void Init()
  {
  }

  private void TryDestroy()
  {
    this.destroyed = true;
    Object.Destroy((Object) this);
  }

  private void LateUpdate()
  {
    if (this.destroyed)
      return;
    this.EffectRoutine();
  }

  protected virtual void EffectRoutine()
  {
    this.textInfo = this.m_TextComponent.textInfo;
    int characterCount = this.textInfo.characterCount;
    if (characterCount == 0)
      return;
    for (int index = 0; index < characterCount; ++index)
      this.UpdateCharacter(index);
  }

  public virtual void UpdateCharacter(int index)
  {
  }
}
