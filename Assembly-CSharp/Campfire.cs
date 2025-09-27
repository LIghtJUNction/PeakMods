// Decompiled with JetBrains decompiler
// Type: Campfire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

#nullable disable
public class Campfire : MonoBehaviour, IInteractibleConstant, IInteractible
{
  public Segment advanceToSegment;
  public Campfire.FireState state;
  public GameObject enableWhenLit;
  public GameObject disableWhenLit;
  [FormerlySerializedAs("litTime")]
  public float burnsFor = 180f;
  public float cookTime = 5f;
  public Transform logRoot;
  public int requiredFireWoods = 3;
  public Vector2 endSize = new Vector2(0.1f, 0.2f);
  public float endRot = 3f;
  [FormerlySerializedAs("litTimeElapsed")]
  public float beenBurningFor;
  public ParticleSystem fireParticles;
  public ParticleSystem smokeParticlesOff;
  public ParticleSystem smokeParticlesLit;
  public float moraleBoostRadius;
  public float moraleBoostBaseline;
  public float moraleBoostPerAdditionalScout;
  public float injuryReduction = 0.2f;
  public SFX_Instance[] fireStart;
  public SFX_Instance[] extinguish;
  public SFX_Instance[] moraleBoost;
  public AudioSource loop;
  public bool isPyre;
  public string nameOverride;
  private Item currentlyCookingItem;
  private Renderer mainRenderer;
  private float startRot;
  private Vector2 startSize;
  private bool t;
  private PhotonView view;
  public bool disableFogFakeMountain;

  public bool Lit => this.state == Campfire.FireState.Lit;

  public float LitProgress => (this.beenBurningFor / this.burnsFor).Clamp01();

  private void Awake()
  {
    this.view = this.GetComponent<PhotonView>();
    this.mainRenderer = this.GetComponentInChildren<Renderer>();
    this.startRot = this.fireParticles.emission.rateOverTime.constant;
    ParticleSystem.MinMaxCurve startSize = this.fireParticles.main.startSize;
    double constantMin = (double) startSize.constantMin;
    startSize = this.fireParticles.main.startSize;
    double constantMax = (double) startSize.constantMax;
    this.startSize = new Vector2((float) constantMin, (float) constantMax);
    this.SetFireWoodCount(3);
    this.UpdateLit();
  }

  private void Update()
  {
    if (this.Lit)
    {
      this.beenBurningFor += Time.deltaTime;
      ParticleSystem.MainModule main = this.fireParticles.main;
      ParticleSystem.MinMaxCurve startSize = main.startSize with
      {
        constantMin = Mathf.Lerp(this.startSize.x, this.endSize.x, this.LitProgress),
        constantMax = Mathf.Lerp(this.startSize.y, this.endSize.y, this.LitProgress)
      };
      main.startSize = startSize;
      ParticleSystem.EmissionModule emission = this.fireParticles.emission;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime with
      {
        constant = Mathf.Lerp(this.startRot, this.endRot, this.LitProgress)
      };
      emission.rateOverTime = rateOverTime;
      if (!this.t)
      {
        if (!this.isPyre && MoraleBoost.SpawnMoraleBoost(this.transform.position, this.moraleBoostRadius, this.moraleBoostBaseline, this.moraleBoostPerAdditionalScout, minScouts: 2))
        {
          for (int index = 0; index < this.moraleBoost.Length; ++index)
            this.moraleBoost[index].Play(this.transform.position);
          Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MoraleBoosts, 1);
        }
        if ((Object) Character.localCharacter != (Object) null && (double) Vector3.Distance(this.transform.position, Character.localCharacter.Center) <= (double) this.moraleBoostRadius)
          Character.localCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.2f);
        for (int index = 0; index < this.fireStart.Length; ++index)
          this.fireStart[index].Play(this.transform.position);
        this.t = true;
      }
      if (this.view.IsMine && (double) this.beenBurningFor > (double) this.burnsFor && !this.isPyre)
        this.view.RPC("Extinguish_Rpc", RpcTarget.AllBuffered);
    }
    else if (this.t)
    {
      for (int index = 0; index < this.extinguish.Length; ++index)
        this.extinguish[index].Play(this.transform.position);
      this.t = false;
    }
    this.StupidTextUpdate();
    this.UpdateAudioLoop();
  }

  private void StupidTextUpdate()
  {
    if (GUIManager.instance.currentInteractable != this)
      return;
    GUIManager.instance.RefreshInteractablePrompt();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.moraleBoostRadius);
  }

  public Vector3 Center() => this.mainRenderer.bounds.center;

  public string GetInteractionText()
  {
    string printout;
    return !this.Lit ? (!this.EveryoneInRange(out printout) ? printout : LocalizedText.GetText("LIGHT")) : (this.Lit ? LocalizedText.GetText("COOK") : "");
  }

  public string GetName()
  {
    return !string.IsNullOrEmpty(this.nameOverride) ? LocalizedText.GetText(this.nameOverride) : LocalizedText.GetText("CAMPFIRE");
  }

  public Transform GetTransform() => this.transform;

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }

  public void Interact(Character interactor)
  {
    if (!this.Lit || !((Object) interactor.data.currentItem != (Object) null) || !interactor.data.currentItem.cooking.canBeCooked)
      return;
    this.currentlyCookingItem = interactor.data.currentItem;
    interactor.data.currentItem.GetComponent<ItemCooking>().StartCookingVisuals();
  }

  public void Interact_CastFinished(Character interactor)
  {
    if (this.Lit)
    {
      if (!(bool) (Object) this.currentlyCookingItem)
        return;
      if (this.currentlyCookingItem.GetData<IntItemData>(DataEntryKey.CookedAmount).Value == 0)
        Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MealsCooked, 1);
      this.currentlyCookingItem.GetComponent<ItemCooking>().FinishCooking();
    }
    else
    {
      if (!this.EveryoneInRange(out string _))
        return;
      this.view.RPC("Light_Rpc", RpcTarget.All);
    }
  }

  public void CancelCast(Character interactor)
  {
    if ((bool) (Object) this.currentlyCookingItem)
      this.currentlyCookingItem.GetComponent<ItemCooking>().CancelCookingVisuals();
    this.currentlyCookingItem = (Item) null;
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public bool holdOnFinish => false;

  public bool IsInteractible(Character interactor)
  {
    if (this.state == Campfire.FireState.Off)
      return true;
    return this.state != Campfire.FireState.Spent && (Object) interactor.data.currentItem != (Object) null;
  }

  public bool EveryoneInRange(out string printout)
  {
    bool flag = true;
    printout = "";
    foreach (Character allPlayerCharacter in PlayerHandler.GetAllPlayerCharacters())
    {
      if (!((Object) allPlayerCharacter == (Object) null) && !allPlayerCharacter.photonView.Owner.IsInactive)
      {
        float num = Vector3.Distance(this.transform.position, allPlayerCharacter.Center);
        if ((double) num > 15.0 && !allPlayerCharacter.data.dead)
        {
          flag = false;
          printout += $"\n{allPlayerCharacter.photonView.Owner.NickName} {Mathf.RoundToInt(num * CharacterStats.unitsToMeters)}m";
        }
      }
    }
    if (!flag)
      printout = $"{LocalizedText.GetText("CANTLIGHT")}\n{printout}";
    return flag;
  }

  public bool IsConstantlyInteractable(Character interactor)
  {
    if (this.state == Campfire.FireState.Off)
      return true;
    return this.state != Campfire.FireState.Spent && (Object) interactor.data.currentItem != (Object) null;
  }

  public float GetInteractTime(Character interactor) => this.cookTime;

  public void DebugLight() => this.view.RPC("Light_Rpc", RpcTarget.All);

  [PunRPC]
  private void SetFireWoodCount(int count)
  {
  }

  private void UpdateAudioLoop()
  {
    if (!(bool) (Object) this.loop)
      return;
    this.loop.volume = Mathf.Lerp(this.loop.volume, this.Lit ? 0.5f : 0.0f, Time.deltaTime * 5f);
  }

  private void HideLogs()
  {
    foreach (Component component in this.logRoot)
      component.gameObject.SetActive(false);
  }

  [PunRPC]
  private void Light_Rpc()
  {
    this.state = Campfire.FireState.Lit;
    Shader.SetGlobalFloat("FakeMountainEnabled", this.disableFogFakeMountain ? 0.0f : 1f);
    this.UpdateLit();
    this.smokeParticlesOff.Stop();
    this.smokeParticlesLit.Play();
    GUIManager.instance.RefreshInteractablePrompt();
    if (!(bool) (Object) Singleton<MapHandler>.Instance)
      return;
    Singleton<MapHandler>.Instance.GoToSegment(this.advanceToSegment);
  }

  [PunRPC]
  private void Extinguish_Rpc()
  {
    this.beenBurningFor = 0.0f;
    this.state = Campfire.FireState.Spent;
    this.HideLogs();
    this.UpdateLit();
    this.smokeParticlesOff.Stop();
    this.smokeParticlesLit.Stop();
    this.fireParticles.Stop();
  }

  private void UpdateLit()
  {
    if ((bool) (Object) this.enableWhenLit)
      this.enableWhenLit.SetActive(this.state == Campfire.FireState.Lit);
    if (!(bool) (Object) this.disableWhenLit)
      return;
    this.disableWhenLit.SetActive(this.state == Campfire.FireState.Off || this.state == Campfire.FireState.Spent);
  }

  public enum FireState
  {
    Off,
    Lit,
    Spent,
  }
}
