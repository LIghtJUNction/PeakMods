// Decompiled with JetBrains decompiler
// Type: Bugfixer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class Bugfixer : MonoBehaviour
{
  public bool useLocalCharacter;

  private void Start()
  {
  }

  private void Update()
  {
    if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKeyDown(KeyCode.Period))
      return;
    Character target = this.GetTarget();
    if (!((Object) target != (Object) null))
      return;
    PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, (object) target.photonView.ViewID);
  }

  private Character GetTarget()
  {
    if (this.useLocalCharacter)
      return Character.localCharacter;
    Character target = (Character) null;
    float num1 = float.MaxValue;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num2 = Vector3.Angle(MainCamera.instance.transform.forward, allCharacter.Center - MainCamera.instance.transform.position);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        target = allCharacter;
      }
    }
    return target;
  }
}
