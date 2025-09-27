// Decompiled with JetBrains decompiler
// Type: GUIManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Settings;

#nullable disable
public class GUIManager : MonoBehaviour
{
  public static GUIManager instance;
  public Canvas hudCanvas;
  public Canvas binocularOverlay;
  public Canvas letterboxCanvas;
  public BoardingPass boardingPass;
  public StaminaBar bar;
  public InventoryItemUI[] items;
  public InventoryItemUI backpack;
  public InventoryItemUI temporaryItem;
  public CanvasGroup hudCanvasGroup;
  public Sprite emptySprite;
  public UI_Rope ui_rope;
  public GameObject emoteWheel;
  public BackpackWheel backpackWheel;
  public UIPlayerNames playerNames;
  public UI_UseItemProgressFriend friendUseItemProgressPrefab;
  public Transform friendProgressTF;
  public GameObject fogRises;
  public GameObject lavaRises;
  public LoadingScreen loadingScreenPrefab;
  [FormerlySerializedAs("endgameCounter")]
  public EndgameCounter endgame;
  public EndScreen endScreen;
  public GameObject pauseMenu;
  public List<UI_UseItemProgressFriend> friendUseItemProgressList = new List<UI_UseItemProgressFriend>();
  private TextMeshProUGUI text;
  public GameObject interactName;
  public TextMeshProUGUI interactNameText;
  public GameObject interactPromptPrimary;
  public GameObject interactPromptSecondary;
  public GameObject interactPromptHold;
  public GameObject interactPromptLunge;
  public TextMeshProUGUI interactPromptText;
  public TextMeshProUGUI secondaryInteractPromptText;
  public TextMeshProUGUI itemPromptMain;
  public TextMeshProUGUI itemPromptScroll;
  public TextMeshProUGUI itemPromptSecondary;
  public TextMeshProUGUI itemPromptDrop;
  public TextMeshProUGUI itemPromptThrow;
  public GameObject throwGO;
  public Image throwBar;
  public Gradient throwGradient;
  public GameObject dyingBarObject;
  public RectTransform dyingBarRect;
  public Image dyingBarImage;
  public Gradient dyingBarGradient;
  public Animator dyingBarAnimator;
  public GameObject spectatingObject;
  public TextMeshProUGUI spectatingNameText;
  public Color spectatingNameColor;
  public Color spectatingYourselfColor;
  public GameObject heroObject;
  public GameObject heroCanvasObject;
  public Camera heroCamera;
  public Image heroBG;
  public RawImage heroImage;
  public RawImage heroShadowImage;
  public TextMeshProUGUI heroText;
  public TextMeshProUGUI heroDayText;
  public TextMeshProUGUI heroTimeOfDayText;
  public AudioSource stingerSound;
  public Volume blurVolume;
  public Volume coldVolume;
  public Volume sugarRushVolume;
  public ScreenVFX injurySVFX;
  public ScreenVFX coldSVFX;
  public ScreenVFX poisonSVFX;
  public ScreenVFX sugarRushSVFX;
  public ScreenVFX hotSVFX;
  public ScreenVFX energySVFX;
  public ScreenVFX drowsyFX;
  public ScreenVFX heatSVFX;
  public ScreenVFX curseSVFX;
  public ScreenVFX sunscreenSVFX;
  public ScreenVFX thornsSVFX;
  private Character character;
  public GameObject reticleDefault;
  public GameObject reticleX;
  public GameObject reticleClimb;
  public GameObject reticleClimbJump;
  public GameObject reticleThrow;
  public GameObject reticleReach;
  public GameObject reticleGrasp;
  public GameObject reticleSpike;
  public GameObject reticleRope;
  public GameObject reticleClimbTry;
  public GameObject reticleVine;
  public GameObject reticleBoost;
  public GameObject reticleShoot;
  public Image reticleDefaultImage;
  public Color reticleColorDefault;
  public Color reticleColorHighlight;
  private Coroutine _heroRoutine;
  public BadgeManager mainBadgeManager;
  public bool photosensitivity;
  public bool colorblindness;
  private bool wasPitonClimbing;
  private int lastBlockedInput;
  private bool dead;
  private Character currentSpecCharacter;
  private int ROPE_INVERT = Shader.PropertyToID("Invert");
  private float reticleLock;
  private GameObject lastReticle;
  private List<GameObject> reticleList = new List<GameObject>();
  private DG.Tweening.Sequence injurySequence;
  private DG.Tweening.Sequence hungerSequence;
  private DG.Tweening.Sequence coldSequence;
  private DG.Tweening.Sequence poisonSequence;
  public int sinceShowedBinocularOverlay = 10;
  private bool canUsePrimaryPrevious;
  private bool canUseSecondaryPrevious;

  public bool wheelActive
  {
    get => this.emoteWheel.gameObject.activeSelf || this.backpackWheel.gameObject.activeSelf;
  }

  internal IInteractible currentInteractable { get; private set; }

  public ControllerManager controllerManager { get; private set; }

  private void Awake()
  {
    GUIManager.instance = this;
    this.controllerManager = new ControllerManager();
    this.controllerManager.Init();
    this.InitReticleList();
  }

  private void OnDestroy()
  {
    this.controllerManager.Destroy();
    if (!((UnityEngine.Object) this.character != (UnityEngine.Object) null))
      return;
    this.character.refs.items.onSlotEquipped -= new Action(this.OnSlotEquipped);
    GameUtils.instance.OnUpdatedFeedData -= new Action(this.OnUpdatedFeedData);
  }

  private void Start()
  {
    this.UpdateItemPrompts();
    this.OnInteractChange();
    this.throwGO.SetActive(false);
    this.spectatingObject.SetActive(false);
    this.heroObject.SetActive(false);
    PhotosensitiveSetting setting1 = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
    ColorblindSetting setting2 = GameHandler.Instance.SettingsHandler.GetSetting<ColorblindSetting>();
    this.photosensitivity = setting1.Value == OffOnMode.ON;
    this.colorblindness = setting2.Value == OffOnMode.ON;
  }

  private void LateUpdate()
  {
    this.UpdateDebug();
    this.UpdateBinocularOverlay();
    this.UpdateWindowStatus();
    if ((bool) (UnityEngine.Object) Character.localCharacter)
    {
      if (Interaction.instance.currentHovered != this.currentInteractable)
        this.OnInteractChange();
      if (this.wasPitonClimbing)
        this.RefreshInteractablePrompt();
      this.interactPromptLunge.SetActive(Character.localCharacter.data.isClimbing && (double) Character.localCharacter.data.currentStamina < 0.05000000074505806 && (double) Character.localCharacter.data.currentStamina > 9.9999997473787516E-05);
      this.wasPitonClimbing = Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing;
      if (!(bool) (UnityEngine.Object) this.character)
      {
        this.character = Character.localCharacter;
        this.character.refs.items.onSlotEquipped += new Action(this.OnSlotEquipped);
        GameUtils.instance.OnUpdatedFeedData += new Action(this.OnUpdatedFeedData);
      }
      this.UpdateReticle();
      this.UpdateThrow();
      this.UpdateRope();
      this.UpdateDyingBar();
      this.UpdateEmoteWheel();
      this.TestUpdateItemPrompts();
      this.UpdateSpectate();
      this.UpdatePaused();
    }
    if (!(bool) (UnityEngine.Object) Character.observedCharacter)
      return;
    this.UpdateItems();
  }

  public bool windowShowingCursor { get; private set; }

  public bool windowBlockingInput { get; private set; }

  public void UpdateWindowStatus()
  {
    this.windowShowingCursor = false;
    this.windowBlockingInput = false;
    foreach (MenuWindow allActiveWindow in MenuWindow.AllActiveWindows)
    {
      if (allActiveWindow.blocksPlayerInput)
        this.lastBlockedInput = Time.frameCount;
      if (allActiveWindow.showCursorWhileOpen)
        this.windowShowingCursor = true;
    }
    if (this.pauseMenu.activeSelf)
    {
      this.windowShowingCursor = true;
      this.windowBlockingInput = true;
    }
    if (Time.frameCount >= this.lastBlockedInput + 2)
      return;
    this.windowBlockingInput = true;
  }

  public void UpdatePaused()
  {
    if (!Character.localCharacter.input.pauseWasPressed || LoadingScreenHandler.loading || this.pauseMenu.activeSelf || this.wheelActive || this.endScreen.isOpen)
      return;
    this.pauseMenu.SetActive(true);
    Character.localCharacter.input.pauseWasPressed = false;
  }

  private void OnSlotEquipped()
  {
    for (int index = 0; index < this.items.Length; ++index)
    {
      if (index < Character.localCharacter.player.itemSlots.Length)
        this.items[index].SetSelected();
    }
    this.backpack.SetSelected();
  }

  private void OnUpdatedFeedData()
  {
    List<FeedData> feedData = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID);
    for (int i = 0; i < feedData.Count; i++)
    {
      if (!this.friendUseItemProgressList.Any<UI_UseItemProgressFriend>((Func<UI_UseItemProgressFriend, bool>) (f => f.giverID == feedData[i].giverID)))
      {
        UI_UseItemProgressFriend itemProgressFriend = UnityEngine.Object.Instantiate<UI_UseItemProgressFriend>(this.friendUseItemProgressPrefab, this.friendProgressTF);
        this.friendUseItemProgressList.Add(itemProgressFriend);
        itemProgressFriend.Init(feedData[i]);
      }
    }
    for (int i = 0; i < this.friendUseItemProgressList.Count; i++)
    {
      if (!feedData.Any<FeedData>((Func<FeedData, bool>) (f => f.giverID == this.friendUseItemProgressList[i].giverID)))
      {
        this.friendUseItemProgressList[i].Kill();
        this.friendUseItemProgressList.RemoveAt(i);
      }
    }
  }

  public void SetHeroTitle(string text, AudioClip stinger)
  {
    Debug.Log((object) ("Set hero title: " + text));
    if (this._heroRoutine != null)
      this.StopCoroutine(this._heroRoutine);
    if ((bool) (UnityEngine.Object) this.stingerSound && (UnityEngine.Object) stinger != (UnityEngine.Object) null)
    {
      this.stingerSound.clip = stinger;
      this.stingerSound.Play();
    }
    this._heroRoutine = this.StartCoroutine(HeroRoutine(text));

    IEnumerator HeroRoutine(string heroString)
    {
      this.heroCanvasObject.gameObject.SetActive(true);
      yield return (object) null;
      string dayString = DayNightManager.instance.DayCountString();
      string timeOfDayString = DayNightManager.instance.TimeOfDayString();
      this.heroObject.gameObject.SetActive(true);
      this.heroImage.color = new Color(this.heroImage.color.r, this.heroImage.color.g, this.heroImage.color.b, 1f);
      this.heroShadowImage.color = new Color(this.heroShadowImage.color.r, this.heroShadowImage.color.g, this.heroShadowImage.color.b, 0.12f);
      this.heroDayText.text = "";
      this.heroTimeOfDayText.text = "";
      this.heroBG.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
      DOTweenModuleUI.DOFade(this.heroBG, 0.5f, 0.5f);
      int i;
      for (i = 0; i < heroString.Length; ++i)
      {
        this.heroText.text = heroString.Substring(0, i + 1);
        this.heroCamera.Render();
        yield return (object) new WaitForSeconds(0.1f);
      }
      yield return (object) new WaitForSeconds(0.5f);
      for (i = 0; i < dayString.Length; ++i)
      {
        this.heroDayText.text = dayString.Substring(0, i + 1);
        this.heroCamera.Render();
        yield return (object) new WaitForSeconds(0.066f);
      }
      yield return (object) new WaitForSeconds(0.5f);
      for (i = 0; i < timeOfDayString.Length; ++i)
      {
        this.heroTimeOfDayText.text = timeOfDayString.Substring(0, i + 1);
        this.heroCamera.Render();
        yield return (object) new WaitForSeconds(0.066f);
      }
      yield return (object) new WaitForSeconds(1.5f);
      this.heroImage.DOFade(0.0f, 2f);
      this.heroShadowImage.DOFade(0.0f, 1f);
      DOTweenModuleUI.DOFade(this.heroBG, 0.0f, 2f);
      yield return (object) new WaitForSeconds(2f);
      this.heroObject.gameObject.SetActive(false);
      this.heroCanvasObject.gameObject.SetActive(false);
    }
  }

  public void OpenBackpackWheel(BackpackReference backpackReference)
  {
    if (this.wheelActive || this.windowBlockingInput)
      return;
    Character.localCharacter.data.usingBackpackWheel = true;
    this.backpackWheel.InitWheel(backpackReference);
  }

  public void CloseBackpackWheel()
  {
    Debug.Log((object) "Close Input Wheel");
    Character.localCharacter.data.usingBackpackWheel = false;
    this.backpackWheel.gameObject.SetActive(false);
  }

  private void UpdateEmoteWheel()
  {
    if (Character.localCharacter.input.emoteIsPressed)
    {
      if (this.wheelActive || this.windowBlockingInput)
        return;
      this.emoteWheel.SetActive(true);
      Character.localCharacter.data.usingEmoteWheel = true;
    }
    else
    {
      if (!Character.localCharacter.data.usingEmoteWheel)
        return;
      this.emoteWheel.SetActive(false);
      Character.localCharacter.data.usingEmoteWheel = false;
    }
  }

  private void UpdateDyingBar()
  {
    this.dyingBarObject.gameObject.SetActive(Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead);
    if (this.dyingBarObject.gameObject.activeSelf)
    {
      this.dyingBarImage.fillAmount = 1f - Character.localCharacter.data.deathTimer;
      this.dyingBarImage.color = this.dyingBarGradient.Evaluate(1f - Character.localCharacter.data.deathTimer);
      if ((double) Character.localCharacter.data.deathTimer < 1.0 || this.dead)
        return;
      this.dyingBarAnimator.Play("Dead", 0, 0.0f);
      this.dead = true;
    }
    else
      this.dead = false;
  }

  private void UpdateSpectate()
  {
    if (!((UnityEngine.Object) MainCameraMovement.specCharacter != (UnityEngine.Object) this.currentSpecCharacter))
      return;
    this.currentSpecCharacter = MainCameraMovement.specCharacter;
    if ((bool) (UnityEngine.Object) this.currentSpecCharacter)
    {
      this.spectatingObject.SetActive(true);
      if ((UnityEngine.Object) this.currentSpecCharacter == (UnityEngine.Object) Character.localCharacter)
      {
        this.spectatingNameText.text = LocalizedText.GetText("YOURSELF");
        this.spectatingNameText.color = this.spectatingYourselfColor;
      }
      else
      {
        this.spectatingNameText.text = MainCameraMovement.specCharacter.characterName;
        this.spectatingNameText.color = this.spectatingNameColor;
      }
    }
    else
      this.spectatingObject.SetActive(false);
  }

  private void UpdateRope()
  {
    RopeSpool component;
    if ((bool) (UnityEngine.Object) Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.TryGetComponent<RopeSpool>(out component))
    {
      this.ui_rope.gameObject.SetActive(true);
      if ((bool) (UnityEngine.Object) component.rope)
        this.ui_rope.UpdateRope(component.rope.GetRopeSegments().Count);
      Shader.SetGlobalFloat(this.ROPE_INVERT, component.isAntiRope ? 1f : 0.0f);
    }
    else
      this.ui_rope.gameObject.SetActive(false);
  }

  private void UpdateThrow()
  {
    this.throwGO.SetActive((double) Character.localCharacter.refs.items.throwChargeLevel > 0.0);
    if ((double) Character.localCharacter.refs.items.throwChargeLevel <= 0.0)
      return;
    this.throwBar.fillAmount = Mathf.Lerp(0.692f, 0.808f, Character.localCharacter.refs.items.throwChargeLevel);
    this.throwBar.color = this.throwGradient.Evaluate(Character.localCharacter.refs.items.throwChargeLevel);
  }

  private void UpdateReticle()
  {
    this.reticleDefaultImage.color = (double) this.character.data.sinceCanClimb < 0.05000000074505806 ? this.reticleColorHighlight : this.reticleColorDefault;
    if (Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead)
      this.SetReticle((GameObject) null);
    else if ((double) this.reticleLock > 0.0)
      this.reticleLock -= Time.deltaTime;
    else if ((UnityEngine.Object) Character.localCharacter.data.currentClimbHandle != (UnityEngine.Object) null)
      this.SetReticle(this.reticleSpike);
    else if (Character.localCharacter.data.isRopeClimbing)
      this.SetReticle(this.reticleRope);
    else if ((double) Character.localCharacter.data.sincePalJump < 0.5)
      this.SetReticle(this.reticleBoost);
    else if ((double) Character.localCharacter.refs.items.throwChargeLevel > 0.0)
      this.SetReticle(this.reticleThrow);
    else if ((double) Character.localCharacter.data.sincePressClimb < 0.10000000149011612 && Character.localCharacter.refs.climbing.CanClimb())
      this.SetReticle(this.reticleClimbTry);
    else if (Character.localCharacter.data.isClimbing)
    {
      if (Character.localCharacter.OutOfStamina())
        this.SetReticle(this.reticleX);
      else
        this.SetReticle(this.reticleClimb);
    }
    else if (Character.localCharacter.data.isReaching)
      this.SetReticle(this.reticleReach);
    else if (Character.localCharacter.data.isVineClimbing)
      this.SetReticle(this.reticleVine);
    else if ((bool) (UnityEngine.Object) Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.UIData.isShootable && Character.localCharacter.data.currentItem.CanUsePrimary())
      this.SetReticle(this.reticleShoot);
    else
      this.SetReticle(this.reticleDefault);
  }

  public void ReticleLand()
  {
    RectTransform component = this.reticleDefault.GetComponent<RectTransform>();
    component.sizeDelta = new Vector2(40f, 10f);
    component.DOSizeDelta(new Vector2(10f, 10f), 0.33f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.InOutCubic);
  }

  public void Grasp()
  {
    this.SetReticle(this.reticleGrasp);
    this.reticleGrasp.GetComponent<Animator>().Play("Play", 0, 0.0f);
    this.reticleLock = 1f;
  }

  public void ClimbJump()
  {
    this.SetReticle(this.reticleClimbJump);
    this.reticleLock = 0.5f;
  }

  private void SetReticle(GameObject activeReticle)
  {
    if ((UnityEngine.Object) activeReticle == (UnityEngine.Object) this.lastReticle && (UnityEngine.Object) activeReticle != (UnityEngine.Object) null)
      return;
    this.lastReticle = activeReticle;
    for (int index = 0; index < this.reticleList.Count; ++index)
    {
      if ((UnityEngine.Object) this.reticleList[index] != (UnityEngine.Object) activeReticle)
        this.reticleList[index].SetActive(false);
    }
    if (!(bool) (UnityEngine.Object) activeReticle)
      return;
    activeReticle.SetActive(true);
  }

  private void InitReticleList()
  {
    this.reticleList.Add(this.reticleDefault);
    this.reticleList.Add(this.reticleRope);
    this.reticleList.Add(this.reticleSpike);
    this.reticleList.Add(this.reticleThrow);
    this.reticleList.Add(this.reticleReach);
    this.reticleList.Add(this.reticleX);
    this.reticleList.Add(this.reticleClimb);
    this.reticleList.Add(this.reticleClimbJump);
    this.reticleList.Add(this.reticleClimbTry);
    this.reticleList.Add(this.reticleGrasp);
    this.reticleList.Add(this.reticleVine);
    this.reticleList.Add(this.reticleBoost);
    this.reticleList.Add(this.reticleShoot);
  }

  private void UpdateDebug()
  {
  }

  private IEnumerator ScreenshotRoutine(bool disableHud)
  {
    bool cacheEnabled = this.hudCanvas.enabled;
    if (disableHud)
      this.hudCanvas.enabled = false;
    yield return (object) null;
    string str = "";
    if (Application.isEditor)
    {
      str = "Screenshots/";
      if (!Directory.Exists(str))
        Directory.CreateDirectory(str);
    }
    string path2 = $"Screenshot_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.png";
    ScreenCapture.CaptureScreenshot(Path.Combine(str, path2), 2);
    yield return (object) null;
    this.hudCanvas.enabled = cacheEnabled;
  }

  public void AddStatusFX(CharacterAfflictions.STATUSTYPE type, float amount)
  {
    switch (type)
    {
      case CharacterAfflictions.STATUSTYPE.Injury:
        this.InjuryFX(amount);
        break;
      case CharacterAfflictions.STATUSTYPE.Hunger:
        this.HungerFX();
        break;
      case CharacterAfflictions.STATUSTYPE.Cold:
        this.ColdFX(amount);
        break;
      case CharacterAfflictions.STATUSTYPE.Poison:
        this.PoisonFX(amount);
        break;
      case CharacterAfflictions.STATUSTYPE.Curse:
        this.CurseFX(amount);
        break;
      case CharacterAfflictions.STATUSTYPE.Drowsy:
        this.DrowsyFX();
        break;
      case CharacterAfflictions.STATUSTYPE.Hot:
        this.HotFX(amount);
        break;
      case CharacterAfflictions.STATUSTYPE.Thorns:
        this.ThornsFX(amount);
        break;
      default:
        this.InjuryFX(amount);
        break;
    }
  }

  private void InjuryFX(float amount)
  {
    GamefeelHandler.instance.AddPerlinShake((float) (((double) amount + 1.0) * 5.0), 0.3f);
    this.injurySVFX.Play(amount);
  }

  private void CurseFX(float amount)
  {
    GamefeelHandler.instance.AddPerlinShake((float) (((double) amount + 1.0) * 30.0), 0.3f);
    this.curseSVFX.Play(amount);
  }

  private void HungerFX()
  {
  }

  private void DrowsyFX()
  {
    float amount = 1f;
    GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f);
    this.drowsyFX.Play(amount);
  }

  private void PoisonFX(float amount)
  {
    amount = 0.5f;
    GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f);
    this.poisonSVFX.Play(amount);
  }

  private void ThornsFX(float amount)
  {
    amount = 0.5f;
    GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f);
    this.thornsSVFX.Play(amount);
  }

  private void ColdFX(float amount)
  {
    amount = 1f;
    GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
    this.PlayFXSequence(ref this.coldSequence, this.coldVolume, amount);
  }

  private void HotFX(float amount)
  {
    amount = 1f;
    GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
    this.hotSVFX.Play(amount);
  }

  private void PlayFXSequence(ref DG.Tweening.Sequence sequence, Volume volume, float amount)
  {
    sequence.Kill();
    sequence = DOTween.Sequence();
    sequence.Append((Tween) DOTween.To((DOGetter<float>) (() => volume.weight), (DOSetter<float>) (x => volume.weight = x), amount, 0.06f));
    sequence.AppendInterval(0.25f * amount);
    sequence.Append((Tween) DOTween.To((DOGetter<float>) (() => volume.weight), (DOSetter<float>) (x => volume.weight = x), 0.0f, 0.45f));
  }

  public void StartSugarRush()
  {
    float endValue = 1f;
    if (GUIManager.instance.photosensitivity)
      endValue = 0.25f;
    DOTween.To((DOGetter<float>) (() => this.sugarRushVolume.weight), (DOSetter<float>) (x => this.sugarRushVolume.weight = x), endValue, 0.5f);
    GUIManager.instance.bar.AddRainbow();
  }

  public void EndSugarRush()
  {
    DOTween.To((DOGetter<float>) (() => this.sugarRushVolume.weight), (DOSetter<float>) (x => this.sugarRushVolume.weight = x), 0.0f, 0.5f);
    GUIManager.instance.bar.RemoveRainbow();
  }

  public void StartEnergyDrink() => this.energySVFX.StartFX(0.15f);

  public void EndEnergyDrink() => this.energySVFX.EndFX();

  private void HeatFX(float amount)
  {
    amount = 1f;
    this.heatSVFX.Play(amount);
  }

  public void StartHeat() => this.heatSVFX.StartFX();

  public void EndHeat() => this.heatSVFX.EndFX();

  public void StartSunscreen() => this.sunscreenSVFX.StartFX();

  public void EndSunscreen() => this.sunscreenSVFX.EndFX();

  private void OnInteractChange()
  {
    if (this.currentInteractable.UnityObjectExists<IInteractible>())
      this.currentInteractable.HoverExit();
    this.currentInteractable = Interaction.instance.currentHovered;
    if (this.currentInteractable.UnityObjectExists<IInteractible>())
      this.currentInteractable.HoverEnter();
    this.RefreshInteractablePrompt();
  }

  public void RefreshInteractablePrompt()
  {
    if (this.currentInteractable.UnityObjectExists<IInteractible>())
    {
      this.interactPromptText.text = this.currentInteractable.GetInteractionText();
      this.interactName.SetActive(true);
      this.interactPromptPrimary.SetActive(true);
      this.interactPromptSecondary.SetActive(false);
      this.interactPromptHold.SetActive(false);
      if (this.currentInteractable is Item)
        this.interactNameText.text = ((Item) this.currentInteractable).GetItemName();
      else if (this.currentInteractable is CharacterInteractible currentInteractable)
      {
        this.interactPromptPrimary.SetActive(currentInteractable.IsPrimaryInteractible(Character.localCharacter));
        this.interactName.SetActive(false);
        if (currentInteractable.IsSecondaryInteractible(Character.localCharacter))
        {
          this.interactPromptSecondary.SetActive(true);
          this.secondaryInteractPromptText.text = currentInteractable.GetSecondaryInteractionText();
        }
      }
      else
        this.interactNameText.text = this.currentInteractable.GetName();
    }
    else
    {
      this.interactName.SetActive(false);
      this.interactPromptPrimary.SetActive(false);
      this.interactPromptSecondary.SetActive(false);
      this.interactPromptHold.SetActive(false);
    }
    if (!(bool) (UnityEngine.Object) Character.localCharacter || Character.localCharacter.data.climbingSpikeCount <= 0 || !Character.localCharacter.data.isClimbing)
      return;
    this.interactPromptSecondary.SetActive(true);
    this.secondaryInteractPromptText.text = LocalizedText.GetText("SETPITON");
  }

  public void EnableBinocularOverlay() => this.sinceShowedBinocularOverlay = 0;

  private void UpdateBinocularOverlay()
  {
    if (this.sinceShowedBinocularOverlay > 1)
      this.binocularOverlay.enabled = false;
    else
      this.binocularOverlay.enabled = true;
    ++this.sinceShowedBinocularOverlay;
  }

  public void BlurBinoculars()
  {
  }

  public void UpdateItems()
  {
    if ((UnityEngine.Object) Character.observedCharacter == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) Character.observedCharacter == (UnityEngine.Object) null || (UnityEngine.Object) Character.observedCharacter.player == (UnityEngine.Object) null)
    {
      for (int index = 0; index < this.items.Length; ++index)
        this.items[index].SetItem((ItemSlot) null);
      this.backpack.SetItem((ItemSlot) null);
      this.UpdateItemPrompts();
      this.temporaryItem.gameObject.SetActive(false);
    }
    else
    {
      for (int index = 0; index < this.items.Length; ++index)
      {
        if (index < Character.observedCharacter.player.itemSlots.Length)
          this.items[index].SetItem(Character.observedCharacter.player.itemSlots[index]);
      }
      this.backpack.SetItem((ItemSlot) Character.observedCharacter.player.backpackSlot);
      if (!Character.observedCharacter.player.GetItemSlot((byte) 250).IsEmpty())
      {
        this.temporaryItem.gameObject.SetActive(true);
        this.temporaryItem.SetItem(Character.observedCharacter.player.GetItemSlot((byte) 250));
      }
      else
      {
        this.temporaryItem.gameObject.SetActive(false);
        this.temporaryItem.Clear();
      }
      this.UpdateItemPrompts();
      this.bar.ChangeBar();
    }
  }

  public void PlayDayNightText(int x)
  {
  }

  private void TestUpdateItemPrompts()
  {
    if (!(bool) (UnityEngine.Object) Character.localCharacter || !(bool) (UnityEngine.Object) Character.localCharacter.data.currentItem)
    {
      this.canUsePrimaryPrevious = false;
      this.canUseSecondaryPrevious = false;
    }
    else
    {
      bool flag1 = Character.localCharacter.data.currentItem.CanUsePrimary();
      bool flag2 = Character.localCharacter.data.currentItem.CanUseSecondary();
      if (flag1 != this.canUsePrimaryPrevious || flag2 != this.canUseSecondaryPrevious)
        this.UpdateItemPrompts();
      this.canUsePrimaryPrevious = flag1;
      this.canUsePrimaryPrevious = flag2;
    }
  }

  public void UpdateItemPrompts()
  {
    if ((UnityEngine.Object) Character.localCharacter != (UnityEngine.Object) null && (bool) (UnityEngine.Object) Character.localCharacter.data.currentItem)
    {
      Item currentItem = Character.localCharacter.data.currentItem;
      Item.ItemUIData uiData = currentItem.UIData;
      this.itemPromptMain.text = this.GetMainInteractPrompt(currentItem);
      this.itemPromptSecondary.text = this.GetSecondaryInteractPrompt(currentItem);
      this.itemPromptScroll.text = LocalizedText.GetText(uiData.scrollInteractPrompt);
      this.itemPromptMain.gameObject.SetActive(uiData.hasMainInteract && Character.localCharacter.data.currentItem.CanUsePrimary());
      this.itemPromptSecondary.gameObject.SetActive(uiData.hasSecondInteract && Character.localCharacter.data.currentItem.CanUseSecondary());
      this.itemPromptScroll.gameObject.SetActive(uiData.hasScrollingInteract);
      this.itemPromptDrop.gameObject.SetActive(uiData.canDrop);
      this.itemPromptThrow.gameObject.SetActive(uiData.canThrow);
    }
    else
    {
      this.itemPromptMain.gameObject.SetActive(false);
      this.itemPromptSecondary.gameObject.SetActive(false);
      this.itemPromptScroll.gameObject.SetActive(false);
      this.itemPromptDrop.gameObject.SetActive(false);
      this.itemPromptThrow.gameObject.SetActive(false);
    }
  }

  public void TheFogRises()
  {
    this.fogRises.SetActive(true);
    this.StartCoroutine(FogRisesRoutine());

    IEnumerator FogRisesRoutine()
    {
      yield return (object) new WaitForSeconds(4f);
      this.fogRises.SetActive(false);
    }
  }

  public void TheLavaRises()
  {
    this.lavaRises.SetActive(true);
    this.StartCoroutine(FogRisesRoutine());

    IEnumerator FogRisesRoutine()
    {
      yield return (object) new WaitForSeconds(4f);
      this.lavaRises.SetActive(false);
    }
  }

  private string GetMainInteractPrompt(Item item)
  {
    return LocalizedText.GetText(item.UIData.mainInteractPrompt);
  }

  public string GetSecondaryInteractPrompt(Item item)
  {
    return LocalizedText.GetText(item.UIData.secondaryInteractPrompt);
  }

  public delegate void MenuWindowEvent(MenuWindow window);
}
