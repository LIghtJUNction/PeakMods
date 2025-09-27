// Decompiled with JetBrains decompiler
// Type: PositionSyncer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
public class PositionSyncer : PhotonBinaryStreamSerializer<PositionSyncer.Pos>
{
  private Vector3 currentPos;
  private int forceSyncFrames;
  private Optionable<float3> lastSent;

  public override PositionSyncer.Pos GetDataToWrite()
  {
    this.lastSent = Optionable<float3>.Some((float3) this.transform.position);
    return new PositionSyncer.Pos()
    {
      Position = (float3) this.transform.position
    };
  }

  public override void OnDataReceived(PositionSyncer.Pos data)
  {
    base.OnDataReceived(data);
    this.currentPos = this.transform.position;
  }

  public override bool ShouldSendData()
  {
    float3 last = this.lastSent.Value;
    Vector3 n = this.transform.position;
    if (!IsSame())
      return true;
    if (this.forceSyncFrames <= 0)
      return false;
    --this.forceSyncFrames;
    return true;

    bool IsSame()
    {
      return Mathf.Approximately(last.x, n.x) && Mathf.Approximately(last.y, n.y) && Mathf.Approximately(last.z, n.z);
    }
  }

  private void Update()
  {
    if (this.photonView.IsMine)
      return;
    double num = 1.0 / (double) PhotonNetwork.SerializationRate;
    this.sinceLastPackage += Time.deltaTime;
    float t = this.sinceLastPackage / (float) num;
    if (!this.RemoteValue.IsSome)
      return;
    this.transform.position = Vector3.Lerp(this.currentPos, (Vector3) this.RemoteValue.Value.Position, t);
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!this.photonView.IsMine)
      return;
    this.forceSyncFrames = 10;
  }

  public struct Pos : IBinarySerializable
  {
    public float3 Position;

    public void Serialize(BinarySerializer serializer)
    {
      serializer.WriteHalf3((half3) this.Position);
    }

    public void Deserialize(BinaryDeserializer deserializer)
    {
      this.Position = (float3) deserializer.ReadHalf3();
    }
  }
}
