// Decompiled with JetBrains decompiler
// Type: PlayerGhost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PlayerGhost : MonoBehaviour
{
  private static readonly int MainTex = Shader.PropertyToID("_MainTex");
  private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");
  public Character m_target;
  public Character m_owner;
  public PhotonView m_view;
  [Header("Customization Refrences")]
  public Renderer[] PlayerRenderers;
  public Renderer[] EyeRenderers;
  public Renderer mouthRenderer;
  public Renderer accessoryRenderer;
  public AnimatedMouth animatedMouth;

  private void Awake() => this.m_view = this.GetComponent<PhotonView>();

  [PunRPC]
  public void RPCA_InitGhost(PhotonView character, PhotonView t)
  {
    this.m_owner = character.GetComponent<Character>();
    this.m_owner.Ghost = this;
    this.RPCA_SetTarget(t);
    PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(this.m_owner.photonView.Owner);
    this.animatedMouth.audioSource = character.GetComponent<AnimatedMouth>().audioSource;
    this.CustomizeGhost(playerData);
    if (!character.IsMine)
      return;
    ((IEnumerable<Renderer>) this.PlayerRenderers).ForEach<Renderer>((Action<Renderer>) (r => r.enabled = false));
    ((IEnumerable<Renderer>) this.EyeRenderers).ForEach<Renderer>((Action<Renderer>) (r => r.enabled = false));
    this.mouthRenderer.enabled = false;
    this.accessoryRenderer.enabled = false;
  }

  private void CustomizeGhost(PersistentPlayerData data)
  {
    int skinIndex = CharacterCustomization.GetSkinIndex(data);
    for (int index = 0; index < this.PlayerRenderers.Length; ++index)
      this.PlayerRenderers[index].material.SetColor("_PlayerColor", Singleton<Customization>.Instance.skins[skinIndex].color);
    for (int index = 0; index < this.EyeRenderers.Length; ++index)
      this.EyeRenderers[index].material.SetColor(PlayerGhost.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
    int eyesIndex = CharacterCustomization.GetEyesIndex(data);
    for (int index = 0; index < this.EyeRenderers.Length; ++index)
      this.EyeRenderers[index].material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.eyes[eyesIndex].texture);
    int accessoryIndex = CharacterCustomization.GetAccessoryIndex(data);
    this.accessoryRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
    int mouthIndex = CharacterCustomization.GetMouthIndex(data);
    this.mouthRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
  }

  [PunRPC]
  public void RPCA_SetTarget(PhotonView t) => this.m_target = t.GetComponent<Character>();

  private void Update()
  {
    Vector3 center = this.m_target.Center;
    this.transform.rotation = Quaternion.LookRotation(this.m_owner.data.lookDirection);
    this.transform.position = Vector3.Lerp(this.transform.position, center + this.transform.forward * -1f * this.m_owner.data.spectateZoom + this.transform.up * 0.5f, Time.deltaTime * 3f);
    this.transform.rotation = Quaternion.LookRotation(MainCamera.instance.cam.transform.position - this.transform.position);
  }
}
