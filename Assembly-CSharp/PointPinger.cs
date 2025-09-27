// Decompiled with JetBrains decompiler
// Type: PointPinger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class PointPinger : MonoBehaviour
{
  public GameObject pointPrefab;
  public float coolDown;
  public Character character;
  private GameObject pingInstance;
  private float coolDownLeft = 1f;
  private PhotonView photonView;

  private void Awake()
  {
    this.character = this.GetComponent<Character>();
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void Start()
  {
  }

  private void Update()
  {
    if (!this.photonView.IsMine)
      return;
    this.coolDownLeft -= Time.deltaTime;
    RaycastHit hit;
    if ((double) this.coolDownLeft > 0.0 || !this.character.input.pingWasPressed || this.character.data.dead || !Camera.main.ScreenPointToRay(Input.mousePosition).Raycast(out hit, HelperFunctions.LayerType.TerrainMap.ToLayerMask()))
      return;
    this.coolDownLeft = this.coolDown;
    this.photonView.RPC("ReceivePoint_Rpc", RpcTarget.All, (object) hit.point, (object) hit.normal);
  }

  [PunRPC]
  private void ReceivePoint_Rpc(Vector3 point, Vector3 hitNormal)
  {
    bool flag = PExt.LineCast(this.character.Head, Character.localCharacter.Head, out RaycastHit _, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), true);
    float num1 = Vector3.Distance(this.character.Head, Character.localCharacter.Head);
    PointPing component1 = this.pointPrefab.GetComponent<PointPing>();
    Vector2 visibilityFullNoneNoLos = component1.visibilityFullNoneNoLos;
    float num2 = 1f - Mathf.InverseLerp(visibilityFullNoneNoLos.x, visibilityFullNoneNoLos.x + (float) (((double) visibilityFullNoneNoLos.y - (double) visibilityFullNoneNoLos.x) * (flag ? (double) component1.NoLosVisibilityMul : 1.0)), num1);
    if ((double) num2 <= 0.0)
      return;
    if ((Object) this.pingInstance != (Object) null)
      Object.DestroyImmediate((Object) this.pingInstance);
    this.pingInstance = Object.Instantiate<GameObject>(this.pointPrefab, point, Quaternion.LookRotation((point - this.character.Head).normalized, Vector3.up));
    PointPing component2 = this.pingInstance.GetComponent<PointPing>();
    component2.hitNormal = hitNormal;
    component2.Init(this.character);
    component2.pointPinger = this;
    component2.renderer.material = Object.Instantiate<Material>(this.character.refs.mainRenderer.sharedMaterial);
    component2.material.SetFloat("_Opacity", num2);
    Object.Destroy((Object) this.pingInstance, 2f);
  }
}
