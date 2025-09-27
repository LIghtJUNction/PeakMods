// Decompiled with JetBrains decompiler
// Type: RemoveAfterSeconds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class RemoveAfterSeconds : MonoBehaviour
{
  public float seconds = 5f;
  public bool shrink;
  public bool photonRemove;
  private PhotonView view;

  private void Start()
  {
    if (!this.photonRemove)
      return;
    this.view = this.GetComponent<PhotonView>();
  }

  public void Config(bool setShrink, float setSeconds)
  {
    this.seconds = setSeconds;
    this.shrink = setShrink;
  }

  private void Update()
  {
    if ((double) this.seconds < 0.0)
    {
      if (this.shrink && (double) this.transform.localScale.x > 0.0099999997764825821)
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, Time.deltaTime);
      else if (this.photonRemove && (bool) (Object) this.view)
      {
        if (!this.view.IsMine)
          return;
        PhotonNetwork.Destroy(this.gameObject);
      }
      else
        Object.Destroy((Object) this.gameObject);
    }
    else
      this.seconds -= Time.deltaTime;
  }
}
