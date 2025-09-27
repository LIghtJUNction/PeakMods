// Decompiled with JetBrains decompiler
// Type: Looker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using Zorro.Settings;

#nullable disable
public class Looker : MonoBehaviour
{
  public GameObject guy;
  private Animator anim;
  private Transform pivot;
  private float untilSwitch;
  private bool neverReturn;
  private float selfDestructCounter = 3f;
  private bool isActive;
  private PhotonView view;
  private bool hasChecked;
  private float hasLookedAtMeFor;

  private void Start()
  {
    this.anim = this.GetComponent<Animator>();
    this.pivot = this.transform.Find("Pivot");
    this.SetRandomSwitch();
    this.view = this.GetComponent<PhotonView>();
    if (GameHandler.Instance.SettingsHandler.GetSetting<LookerSetting>().Value != OffOnMode.OFF)
      return;
    this.guy.SetActive(false);
  }

  private void ToggleLookers()
  {
    Looker[] componentsInChildren = this.transform.parent.GetComponentsInChildren<Looker>();
    int num = Random.Range(0, componentsInChildren.Length);
    if ((double) Random.value < 0.949999988079071)
      num = -1;
    foreach (Looker looker in componentsInChildren)
    {
      if (looker.transform.GetSiblingIndex() != num)
        looker.view.RPC("RPCA_DisableLooker", RpcTarget.AllBuffered);
    }
  }

  [PunRPC]
  public void RPCA_DisableLooker() => this.gameObject.SetActive(false);

  private void SetRandomSwitch()
  {
    if ((double) Random.Range(0.0f, 1f) < 0.10000000149011612)
      this.untilSwitch = Random.Range(5f, 20f);
    else
      this.untilSwitch = Random.Range(1f, 5f);
  }

  private void Update()
  {
    if (!PhotonNetwork.InRoom)
      return;
    if (!this.hasChecked)
    {
      if (PhotonNetwork.IsMasterClient && this.transform.GetSiblingIndex() == 0)
        this.ToggleLookers();
      this.hasChecked = true;
    }
    Transform transform = MainCamera.instance.transform;
    this.pivot.LookAt(transform.position);
    if (this.neverReturn)
    {
      this.selfDestructCounter -= Time.deltaTime;
      if ((double) this.selfDestructCounter > 0.0)
        return;
      Object.Destroy((Object) this);
    }
    else
    {
      this.untilSwitch -= Time.deltaTime;
      float num = Vector3.Distance(transform.position, this.pivot.position);
      if (this.isActive & ((double) Vector3.Dot(transform.forward, (this.pivot.position - transform.position).normalized) > 0.800000011920929 && (double) num < 40.0))
      {
        this.untilSwitch -= Time.deltaTime * 5f;
        this.hasLookedAtMeFor += Time.deltaTime;
      }
      bool flag1 = (double) this.hasLookedAtMeFor > 3.0;
      bool flag2 = (double) num < 5.0;
      if (this.view.IsMine && (double) this.untilSwitch <= 0.0)
      {
        this.isActive = !this.isActive;
        this.view.RPC("RPCA_Switch", RpcTarget.All, (object) this.isActive);
      }
      if (!(flag1 | flag2))
        return;
      this.view.RPC("RPCA_CodeRed", RpcTarget.AllBuffered);
    }
  }

  [PunRPC]
  private void RPCA_Switch(bool switchTo)
  {
    if (this.neverReturn)
      return;
    this.anim.SetBool("IsActive", switchTo);
    this.SetRandomSwitch();
  }

  [PunRPC]
  private void RPCA_CodeRed()
  {
    this.anim.SetBool("IsActive", false);
    this.neverReturn = true;
  }
}
