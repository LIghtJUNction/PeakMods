// Decompiled with JetBrains decompiler
// Type: WarpCompassVFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public class WarpCompassVFX : ItemVFX
{
  public Volume warpPost;
  public Volume warpPost2;
  public float maxCastProgress = 1.1f;
  public AnimationCurve warpPost2Curve;
  public CompassPointer compassPointer;
  public float timeStartedBeingUsedOnMe;

  private new void Start()
  {
    base.Start();
    GameUtils.instance.OnUpdatedFeedData += new Action(this.OnUpdatedFeedData);
  }

  private void OnDestroy()
  {
    GameUtils.instance.OnUpdatedFeedData -= new Action(this.OnUpdatedFeedData);
  }

  protected override void Update()
  {
    base.Update();
    float b = this.item.castProgress;
    if ((!this.item.isUsingPrimary || this.item.finishedCast) && (double) this.timeStartedBeingUsedOnMe == 0.0)
      b = 0.0f;
    else if ((double) this.timeStartedBeingUsedOnMe > 0.0)
      b = (Time.time - this.timeStartedBeingUsedOnMe) / this.item.totalSecondaryUsingTime;
    this.warpPost.enabled = (double) this.warpPost.weight > 0.0099999997764825821;
    this.warpPost2.enabled = (double) this.warpPost2.weight > 0.0099999997764825821;
    this.warpPost.weight = (double) this.warpPost2.weight < 1.0 ? Mathf.Lerp(this.warpPost.weight, b, Time.deltaTime * 10f) : 0.0f;
    this.warpPost2.weight = this.warpPost2Curve.Evaluate(this.warpPost.weight);
    this.compassPointer.speedMultiplier = (float) (1.0 + (double) this.warpPost.weight * 4.0);
  }

  protected override void Shake()
  {
    GamefeelHandler.instance.AddPerlinShake((float) ((double) this.warpPost.weight * (double) this.shakeAmount * (double) Time.deltaTime * 100.0));
  }

  private void OnUpdatedFeedData()
  {
    bool flag = false;
    foreach (FeedData feedData in GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID))
    {
      if ((int) feedData.itemID == (int) this.item.itemID)
      {
        flag = true;
        if ((double) this.timeStartedBeingUsedOnMe == 0.0)
          this.timeStartedBeingUsedOnMe = Time.time;
      }
    }
    if (flag)
      return;
    this.timeStartedBeingUsedOnMe = 0.0f;
  }
}
