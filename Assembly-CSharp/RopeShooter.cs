// Decompiled with JetBrains decompiler
// Type: RopeShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RopeShooter : ItemComponent
{
  public ParticleSystem gunshotVFX;
  public ParticleSystem fumesVFX;
  public bool cantReFire;
  public Transform spawnPoint;
  public float length;
  public GameObject ropeAnchorWithRopePref;
  public GameObject hideOnFire;
  public float screenshakeIntensity = 30f;
  public int startAmmo = 1;
  public SFX_Instance[] shotSound;
  public SFX_Instance[] emptySound;
  public float maxLength = 30f;

  private int Ammo
  {
    get
    {
      return this.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value;
    }
    set
    {
      this.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value = value;
      this.item.SetUseRemainingPercentage((float) value / (float) this.startAmmo);
    }
  }

  private IntItemData GetNew()
  {
    Debug.Log((object) $"GetNew startAmmo: {this.startAmmo}");
    return new IntItemData() { Value = this.startAmmo };
  }

  public override void Awake()
  {
    base.Awake();
    this.item.OnPrimaryFinishedCast += new Action(this.OnPrimaryFinishedCast);
  }

  private void OnDestroy()
  {
    this.item.OnPrimaryFinishedCast -= new Action(this.OnPrimaryFinishedCast);
  }

  public void Update()
  {
    this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out RaycastHit _));
  }

  public bool HasAmmo => this.Ammo >= 1;

  private void OnPrimaryFinishedCast()
  {
    if (!this.WillAttach(out RaycastHit _))
      return;
    Debug.Log((object) nameof (OnPrimaryFinishedCast));
    if (!this.HasAmmo)
    {
      this.fumesVFX.Play();
      Debug.Log((object) $"totalUses < 1,  {this.item.totalUses}");
      for (int index = 0; index < this.emptySound.Length; ++index)
        this.emptySound[index].Play(this.transform.position);
    }
    else
    {
      RaycastHit hit;
      if (!Camera.main.ForwardRay<Camera>().Raycast(out hit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0.0f))
        return;
      Quaternion identity = Quaternion.identity;
      if ((double) Vector3.Angle(hit.normal, Vector3.up) < 45.0)
      {
        Debug.Log((object) "Angle is less than 45");
        ExtQuaternion.FromUpAndRightPrioUp(this.transform.forward, hit.normal);
      }
      else
      {
        Debug.Log((object) "Angle is more than 45");
        ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, -Camera.main.transform.forward);
      }
      GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.spawnPoint.position, ExtQuaternion.FromUpAndRightPrioUp(this.transform.forward, hit.normal));
      float num = Vector3.Distance(this.spawnPoint.position, hit.point) * 0.01f;
      this.gunshotVFX.Play();
      for (int index = 0; index < this.shotSound.Length; ++index)
        this.shotSound[index].Play(this.transform.position);
      GamefeelHandler.instance.AddPerlinShakeProximity(this.gunshotVFX.transform.position, this.screenshakeIntensity, 0.3f);
      this.hideOnFire.SetActive(this.HasAmmo);
      --this.Ammo;
      this.photonView.RPC("Sync_Rpc", RpcTarget.AllBuffered, (object) this.HasAmmo);
      gameObject.GetComponent<RopeAnchorProjectile>().photonView.RPC("GetShot", RpcTarget.AllBuffered, (object) hit.point, (object) num, (object) this.length, (object) -Camera.main.transform.forward);
      if (!this.photonView.IsMine)
        return;
      Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, Rope.GetLengthInMeters(this.length));
      GameUtils.instance.IncrementPermanentItemsPlaced();
    }
  }

  [PunRPC]
  private void Sync_Rpc(bool show)
  {
    Debug.Log((object) $"Sync_Rpc: {show}");
    this.hideOnFire.SetActive(show);
  }

  public bool WillAttach(out RaycastHit hit)
  {
    hit = new RaycastHit();
    return Character.localCharacter.data.isGrounded && this.HasAmmo && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, (int) HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
  }

  public override void OnInstanceDataSet()
  {
    this.hideOnFire.SetActive(this.HasAmmo);
    Debug.Log((object) $" OnInstanceDataSet item.totalUses: {this.Ammo}");
    this.item.SetUseRemainingPercentage((float) this.Ammo / (float) this.startAmmo);
  }
}
