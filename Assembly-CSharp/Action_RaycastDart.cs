// Decompiled with JetBrains decompiler
// Type: Action_RaycastDart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

#nullable disable
public class Action_RaycastDart : ItemAction
{
  public float maxDistance;
  public float dartCollisionSize;
  [SerializeReference]
  public Affliction[] afflictionsOnHit;
  public Transform spawnTransform;
  public GameObject dartVFX;
  private HelperFunctions.LayerType layerMaskType;
  private RaycastHit lineHit;
  private RaycastHit[] sphereHits;
  public SFX_Instance shotSFX;

  public override void RunAction() => this.FireDart();

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.spawnTransform.position, this.dartCollisionSize);
  }

  private void FireDart()
  {
    if ((bool) (Object) this.shotSFX)
      this.shotSFX.Play(this.transform.position);
    Physics.Raycast(this.spawnTransform.position, MainCamera.instance.transform.forward, out this.lineHit, this.maxDistance, (int) HelperFunctions.terrainMapMask, QueryTriggerInteraction.Ignore);
    if (!(bool) (Object) this.lineHit.collider)
    {
      this.lineHit.distance = this.maxDistance;
      this.lineHit.point = this.spawnTransform.position + MainCamera.instance.transform.forward * this.maxDistance;
    }
    this.sphereHits = Physics.SphereCastAll(this.spawnTransform.position, this.dartCollisionSize, MainCamera.instance.transform.forward, this.lineHit.distance, LayerMask.GetMask("Character"), QueryTriggerInteraction.Ignore);
    foreach (RaycastHit sphereHit in this.sphereHits)
    {
      if ((bool) (Object) sphereHit.collider)
      {
        Character componentInParent = sphereHit.collider.GetComponentInParent<Character>();
        if ((bool) (Object) componentInParent)
        {
          Debug.Log((object) "HIT");
          if ((Object) componentInParent != (Object) this.character)
          {
            this.DartImpact(componentInParent, this.spawnTransform.position, sphereHit.point);
            return;
          }
        }
      }
    }
    this.DartImpact((Character) null, this.spawnTransform.position, this.lineHit.point);
  }

  private void DartImpact(Character hitCharacter, Vector3 origin, Vector3 endpoint)
  {
    if ((bool) (Object) hitCharacter)
      this.photonView.RPC("RPC_DartImpact", RpcTarget.All, (object) hitCharacter.photonView.Owner, (object) origin, (object) endpoint);
    else
      this.photonView.RPC("RPC_DartImpact", RpcTarget.All, null, (object) origin, (object) endpoint);
  }

  [PunRPC]
  private void RPC_DartImpact(Photon.Realtime.Player hitPlayer, Vector3 origin, Vector3 endpoint)
  {
    if (hitPlayer != null && hitPlayer.IsLocal)
    {
      Debug.Log((object) "I'M HIT");
      foreach (Affliction affliction in this.afflictionsOnHit)
        Character.localCharacter.refs.afflictions.AddAffliction(affliction);
    }
    Object.Instantiate<GameObject>(this.dartVFX, endpoint, Quaternion.identity);
    GamefeelHandler.instance.AddPerlinShakeProximity(endpoint, 5f);
  }
}
