// Decompiled with JetBrains decompiler
// Type: StaminaBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class StaminaBar : MonoBehaviour
{
  public Image backing;
  public RectTransform fullBar;
  public RectTransform staminaBar;
  public Image staminaGlow;
  public Image extraStaminaGlow;
  public RectTransform maxStaminaBar;
  public RectTransform staminaBarOutline;
  public RectTransform staminaBarOutlineOverflowBar;
  public RectTransform extraBar;
  public RectTransform extraBarStamina;
  public RectTransform extraBarOutline;
  public Image rainbowStamina;
  [HideInInspector]
  public BarAffliction[] afflictions;
  public float staminaBarOffset;
  private float desiredStaminaSize;
  private float desiredMaxStaminaSize;
  private float desiredExtraStaminaSize;
  public float minAfflictionWidth = 60f;
  public float minStaminaBarWidth = 20f;
  public TextMeshProUGUI moraleBoostText;
  public Animator moraleBoostAnimator;
  public GameObject shield;
  public Color defaultBackingColor;
  public Color outOfStaminaBackingColor;
  private float TAU;
  public SFX_Instance noStaminaSFX;
  private float allAfflictionSizes;
  private bool outOfStamina;
  private float sinTime;
  private bool sequencingExtraBar;
  private Coroutine rainbowRoutine;
  private DOTweenTMPAnimator animator;

  private void Start()
  {
    this.afflictions = this.GetComponentsInChildren<BarAffliction>();
    this.TAU = 6.28318548f;
    foreach (Component affliction in this.afflictions)
      affliction.gameObject.SetActive(false);
  }

  public void ChangeBar()
  {
    for (int index = 0; index < this.afflictions.Length; ++index)
      this.afflictions[index].ChangeAffliction(this);
  }

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    for (int index = 0; index < this.afflictions.Length; ++index)
      this.afflictions[index].UpdateAffliction(this);
    this.desiredStaminaSize = Mathf.Max(0.0f, Character.observedCharacter.data.currentStamina * this.fullBar.sizeDelta.x + this.staminaBarOffset);
    if ((double) Character.observedCharacter.data.currentStamina <= 0.004999999888241291)
    {
      if (!this.outOfStamina)
      {
        this.outOfStamina = true;
        this.OutOfStaminaPulse();
      }
    }
    else
      this.outOfStamina = false;
    this.staminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.staminaBar.sizeDelta.x, this.desiredStaminaSize, Time.deltaTime * 10f), this.staminaBar.sizeDelta.y);
    Color color1 = this.staminaGlow.color;
    float num1 = Mathf.Clamp01((float) (((double) this.staminaBar.sizeDelta.x - (double) this.desiredStaminaSize) * 0.5));
    this.sinTime += Time.deltaTime * 10f * num1;
    color1.a = (float) ((double) num1 * 0.40000000596046448 - (double) Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.20000000298023224);
    this.staminaGlow.color = color1;
    this.desiredMaxStaminaSize = Mathf.Max(0.0f, Character.observedCharacter.GetMaxStamina() * this.fullBar.sizeDelta.x + this.staminaBarOffset);
    this.maxStaminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.maxStaminaBar.sizeDelta.x, this.desiredMaxStaminaSize, Time.deltaTime * 10f), this.maxStaminaBar.sizeDelta.y);
    float statusSum = Character.observedCharacter.refs.afflictions.statusSum;
    this.staminaBarOutline.sizeDelta = new Vector2((float) (14.0 + (double) Mathf.Max(1f, statusSum) * (double) this.fullBar.sizeDelta.x), this.staminaBarOutline.sizeDelta.y);
    this.staminaBarOutlineOverflowBar.gameObject.SetActive((double) statusSum > 201.0 / 200.0);
    this.staminaBar.gameObject.SetActive((double) this.staminaBar.sizeDelta.x > (double) this.minStaminaBarWidth);
    this.maxStaminaBar.gameObject.SetActive((double) this.maxStaminaBar.sizeDelta.x > (double) this.minStaminaBarWidth);
    bool flag = (double) Character.observedCharacter.data.extraStamina > 0.0;
    if (!this.extraBar.gameObject.activeSelf & flag)
    {
      this.extraBar.sizeDelta = Vector2.zero;
      this.extraBar.DOKill();
      this.extraBar.DOSizeDelta(new Vector2(45f, 45f), 0.25f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutCubic);
      this.extraBar.gameObject.SetActive(true);
      this.desiredExtraStaminaSize = Mathf.Max(0.0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
      this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
      this.extraBarStamina.sizeDelta = new Vector2(this.desiredExtraStaminaSize, this.extraBarStamina.sizeDelta.y);
    }
    if (this.extraBar.gameObject.activeSelf)
    {
      this.desiredExtraStaminaSize = Mathf.Max(0.0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
      this.extraBarStamina.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarStamina.sizeDelta.x, Mathf.Max(6f, this.desiredExtraStaminaSize), Time.deltaTime * 10f), this.extraBarStamina.sizeDelta.y);
      if ((double) Mathf.Abs(this.desiredExtraStaminaSize - this.extraBarStamina.sizeDelta.x) < 0.05000000074505806)
        this.extraBarOutline.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarOutline.sizeDelta.x, Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), Time.deltaTime * 10f), this.extraBarOutline.sizeDelta.y);
      else if ((double) this.desiredExtraStaminaSize + 12.0 > (double) this.extraBarOutline.sizeDelta.x)
        this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
      Color color2 = this.extraStaminaGlow.color;
      float num2 = Mathf.Clamp01((float) (((double) this.extraBar.sizeDelta.x - (double) this.desiredExtraStaminaSize) * 0.5));
      this.sinTime += Time.deltaTime * 10f * num2;
      color2.a = (float) ((double) num2 * 0.40000000596046448 - (double) Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.20000000298023224);
      this.extraStaminaGlow.color = color2;
      if (!flag && !this.sequencingExtraBar)
      {
        this.sequencingExtraBar = true;
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append((Tween) this.extraBar.DOSizeDelta(new Vector2(this.extraBar.sizeDelta.x, 0.0f), 0.2f));
        sequence.OnComplete<DG.Tweening.Sequence>(new TweenCallback(this.DisableExtraBar));
      }
    }
    this.shield.gameObject.SetActive(Character.observedCharacter.data.isInvincible);
    if ((double) this.sinTime <= (double) this.TAU)
      return;
    this.sinTime -= this.TAU;
  }

  public void OutOfStaminaPulse()
  {
    this.backing.color = this.outOfStaminaBackingColor;
    DOTweenModuleUI.DOColor(this.backing, this.defaultBackingColor, 0.5f);
    this.noStaminaSFX.Play();
  }

  private void DisableExtraBar()
  {
    this.extraBar.gameObject.SetActive(false);
    this.sequencingExtraBar = false;
  }

  public void AddRainbow()
  {
    if (this.rainbowRoutine != null)
      this.StopCoroutine(this.rainbowRoutine);
    this.rainbowStamina.enabled = true;
    this.rainbowStamina.color = GUIManager.instance.photosensitivity ? new Color(0.5f, 0.5f, 0.5f, 0.0f) : new Color(1f, 1f, 1f, 0.0f);
    DOTweenModuleUI.DOFade(this.rainbowStamina, 1f, 0.5f);
  }

  public void RemoveRainbow()
  {
    DOTweenModuleUI.DOFade(this.rainbowStamina, 0.0f, 0.5f);
    this.rainbowRoutine = this.StartCoroutine(RemoveRainbowRoutine());

    IEnumerator RemoveRainbowRoutine()
    {
      yield return (object) new WaitForSeconds(0.5f);
      this.rainbowStamina.enabled = false;
    }
  }

  public void PlayMoraleBoost(int scoutCount)
  {
    this.moraleBoostText.enabled = true;
    this.moraleBoostText.text = LocalizedText.GetText("MORALEBOOST");
    this.StartCoroutine(this.MoraleBoostRoutine());
  }

  private IEnumerator MoraleBoostRoutine()
  {
    if (this.animator == null)
      this.animator = new DOTweenTMPAnimator((TMP_Text) this.moraleBoostText);
    this.animator.Refresh();
    this.moraleBoostAnimator.Play("Boost", 0, 0.0f);
    for (int charIndex = 0; charIndex < this.animator.textInfo.characterCount; ++charIndex)
      this.animator.SetCharScale(charIndex, Vector3.zero);
    yield return (object) null;
    for (int i = 0; i < this.animator.textInfo.characterCount; ++i)
    {
      this.animator.DOScaleChar(i, Vector3.one, 0.2f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutBack);
      yield return (object) new WaitForSeconds(0.033f);
    }
    yield return (object) new WaitForSeconds(2f);
    yield return (object) new WaitForSeconds(0.5f);
    this.moraleBoostText.enabled = false;
  }
}
