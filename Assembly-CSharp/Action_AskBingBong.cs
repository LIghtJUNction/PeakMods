// Decompiled with JetBrains decompiler
// Type: Action_AskBingBong
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

#nullable disable
public class Action_AskBingBong : ItemAction
{
  public AnimationCurve animationCurve;
  public SFX_Instance shake;
  public SFX_Instance squeak;
  public Action_AskBingBong.BingBongResponse[] responses;
  public Animator squishAnim;
  public Animator anim;
  public bool debugCycle;
  private int debug;
  public TextMeshPro subtitles;
  private float lastAsked;
  public AnimationCurve scaleCurve;
  public AudioSource source;
  public static int MOUTHBLENDID = Animator.StringToHash("Mouth Blend");
  private Coroutine askRoutine;
  private Coroutine subtitleRoutine;

  public event Action_AskBingBong.AskEvent OnAsk;

  public override void RunAction()
  {
    int num = UnityEngine.Random.Range(0, this.responses.Length);
    if (this.debugCycle)
    {
      num = this.debug;
      ++this.debug;
      if (this.debug >= this.responses.Length)
        this.debug = 0;
    }
    this.item.photonView.RPC("Ask", RpcTarget.All, (object) num, (object) ((double) Time.time < (double) this.lastAsked + 1.0));
    if ((double) Time.time <= (double) this.lastAsked + 1.0)
      return;
    this.lastAsked = Time.time;
  }

  [PunRPC]
  public void Ask(int index, bool spamming)
  {
    if (!((UnityEngine.Object) this.item.holderCharacter != (UnityEngine.Object) null))
      return;
    this.squishAnim.SetTrigger("Squish");
    SFX_Player.instance.PlaySFX(this.squeak, this.transform.position, this.transform);
    this.subtitles.gameObject.SetActive(false);
    if (this.askRoutine != null)
      this.StopCoroutine(this.askRoutine);
    this.StartCoroutine(this.AskRoutine(index, spamming));
  }

  private IEnumerator SubtitleRoutine(string subtitleID)
  {
    yield return (object) new WaitForSeconds(0.19f);
    string text = LocalizedText.GetText(subtitleID);
    this.subtitles.gameObject.SetActive(true);
    this.subtitles.text = text;
    float t = 0.0f;
    while ((double) t < 1.7999999523162842)
    {
      t += Time.deltaTime;
      this.subtitles.alpha = Mathf.Clamp01(t * 12f);
      this.subtitles.transform.localScale = Vector3.one * this.scaleCurve.Evaluate(Vector3.Distance(this.subtitles.transform.position, MainCamera.instance.cam.transform.position));
      this.subtitles.transform.forward = MainCamera.instance.cam.transform.forward;
      yield return (object) null;
    }
    this.subtitles.gameObject.SetActive(false);
  }

  private IEnumerator AskRoutine(int index, bool spamming)
  {
    Action_AskBingBong actionAskBingBong = this;
    float t = 0.0f;
    while ((double) t < 0.5)
    {
      actionAskBingBong.item.holderCharacter.refs.items.UpdateAttachedItem();
      t += Time.deltaTime;
      yield return (object) null;
    }
    if (!spamming)
    {
      actionAskBingBong.source.Stop();
      actionAskBingBong.subtitles.gameObject.SetActive(false);
      yield return (object) new WaitForSeconds(0.5f);
      if (actionAskBingBong.subtitleRoutine != null)
        actionAskBingBong.StopCoroutine(actionAskBingBong.subtitleRoutine);
      actionAskBingBong.subtitleRoutine = actionAskBingBong.StartCoroutine(actionAskBingBong.SubtitleRoutine(actionAskBingBong.responses[index].subtitleID));
      if ((UnityEngine.Object) actionAskBingBong.responses[index].sfx != (UnityEngine.Object) null)
      {
        actionAskBingBong.source.PlayOneShot(actionAskBingBong.responses[index].sfx.clips[0]);
        if (actionAskBingBong.OnAsk != null)
          actionAskBingBong.OnAsk(actionAskBingBong.responses[index].sfx.clips[0]);
      }
    }
  }

  public delegate void AskEvent(AudioClip clip);

  [Serializable]
  public class BingBongResponse
  {
    public string subtitleID;
    public SFX_Instance sfx;
    public AnimationCurve mouthCurve;
    public float mouthCurveTime = 1f;
  }
}
