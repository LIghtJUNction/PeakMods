// Decompiled with JetBrains decompiler
// Type: PlayerShaderParams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayerShaderParams : MonoBehaviour
{
  public Vector3 playerCenterOffset;

  private void Update()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    Shader.SetGlobalVector("PlayerPos", (Vector4) (Character.localCharacter.Center + this.playerCenterOffset));
  }
}
