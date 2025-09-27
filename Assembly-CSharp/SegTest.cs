// Decompiled with JetBrains decompiler
// Type: SegTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SegTest : MonoBehaviour
{
  [Range(0.0f, 1f)]
  public float val;
  private ConfigurableJoint joint2;

  private void Start()
  {
    ConfigurableJoint component = this.transform.GetChild(0).GetComponent<ConfigurableJoint>();
    this.joint2 = this.transform.GetChild(1).GetComponent<ConfigurableJoint>();
    this.joint2.connectedBody = component.GetComponent<Rigidbody>();
  }

  private void Update()
  {
    this.joint2.connectedAnchor = new Vector3(0.0f, Mathf.Lerp(0.5f, -0.5f, this.val), 0.0f);
  }
}
