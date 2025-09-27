// Decompiled with JetBrains decompiler
// Type: PhotonBinaryStreamSerializer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
public abstract class PhotonBinaryStreamSerializer<T> : MonoBehaviourPunCallbacks, IPunObservable where T : struct, IBinarySerializable
{
  protected Optionable<T> RemoteValue;
  protected float sinceLastPackage;
  protected PhotonView photonView;

  public abstract T GetDataToWrite();

  protected virtual void Awake() => this.photonView = this.GetComponent<PhotonView>();

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.IsWriting)
    {
      if (!this.ShouldSendData())
        return;
      if (IBinarySerializable.shouldLog)
        Debug.Log((object) $"{this.gameObject.name} sending data in type {this.GetType().Name}");
      T dataToWrite = this.GetDataToWrite();
      BinarySerializer serializer = new BinarySerializer(UnsafeUtility.SizeOf<T>(), Allocator.Temp);
      dataToWrite.Serialize(serializer);
      byte[] byteArray = serializer.buffer.ToByteArray();
      NetworkStats.RegisterBytesSent<T>((ulong) byteArray.Length);
      stream.SendNext((object) byteArray);
      serializer.Dispose();
    }
    else
    {
      if (IBinarySerializable.shouldLog)
        Debug.Log((object) $"{this.gameObject.name} received data in type {this.GetType().Name}");
      BinaryDeserializer deserializer = new BinaryDeserializer((byte[]) stream.ReceiveNext(), Allocator.Temp);
      T data = new T();
      data.Deserialize(deserializer);
      deserializer.Dispose();
      this.RemoteValue = Optionable<T>.Some(data);
      this.OnDataReceived(data);
    }
  }

  public virtual void OnDataReceived(T data) => this.sinceLastPackage = 0.0f;

  public virtual bool ShouldSendData() => true;
}
