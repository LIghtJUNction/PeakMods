// Decompiled with JetBrains decompiler
// Type: Action_ShowBinocularOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Action_ShowBinocularOverlay : ItemAction
{
  public bool binocularsActive;
  public CameraOverride cameraOverride;
  public ItemRenderFeatureManager featureManager;
  public bool isProp;

  private void Update()
  {
    if (!this.binocularsActive || this.isProp)
      return;
    this.TestLookAtSun();
  }

  public override void RunAction()
  {
    this.binocularsActive = !this.binocularsActive;
    if (!this.isProp)
    {
      this.featureManager.setFeatureActive(this.binocularsActive);
      MainCamera.instance.SetCameraOverride(this.binocularsActive ? this.cameraOverride : (CameraOverride) null);
    }
    this.item.photonView.RPC("ToggleUseRPC", RpcTarget.All, (object) this.binocularsActive);
  }

  [PunRPC]
  private void ToggleUseRPC(bool open)
  {
    this.item.defaultPos = new Vector3(this.item.defaultPos.x, open ? 1f : 0.0f, this.item.defaultPos.z);
  }

  private void TestLookAtSun()
  {
    if ((Object) Character.localCharacter == (Object) null || Singleton<MapHandler>.Instance.GetCurrentBiome() != Biome.BiomeType.Mesa || (double) DayNightManager.instance.sun.intensity < 5.0)
      return;
    Transform transform = DayNightManager.instance.sun.transform;
    if ((double) Vector3.Angle(MainCamera.instance.transform.forward, -transform.forward) > 10.0)
      return;
    RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical);
    if (((Object) raycastHit.transform == (Object) null ? 1 : ((Object) raycastHit.transform.root == (Object) Character.localCharacter.transform.root ? 1 : 0)) == 0)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AstronomyBadge);
  }
}
