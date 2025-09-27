// Decompiled with JetBrains decompiler
// Type: CustomTypeRPCSerialization
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using Unity.Collections;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
public static class CustomTypeRPCSerialization
{
  public static void Initialize()
  {
    PhotonPeer.RegisterType(typeof (PhotonView), byte.MaxValue, new SerializeMethod(CustomTypeRPCSerialization.SerializePhotonView), new DeserializeMethod(CustomTypeRPCSerialization.DeserializePhotonView));
    PhotonPeer.RegisterType(typeof (ItemInstanceData), (byte) 254, new SerializeMethod(CustomTypeRPCSerialization.SerializeItemData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeItemData));
    PhotonPeer.RegisterType(typeof (BackpackReference), (byte) 253, new SerializeMethod(CustomTypeRPCSerialization.SerializeBackpackRef), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeBackpackRef));
    PhotonPeer.RegisterType(typeof (ReconnectData), (byte) 252, new SerializeMethod(CustomTypeRPCSerialization.SerializeReconnectData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeReconnectData));
  }

  private static object DeserializeReconnectData(byte[] buffer)
  {
    return (object) ReconnectData.Deserialize(buffer);
  }

  private static byte[] SerializeReconnectData(object customObject)
  {
    return customObject is ReconnectData reconnectData ? reconnectData.Serialize() : throw new Exception("Could not serialize reconnect data, type: " + customObject.GetType().Name);
  }

  private static object DeserializeBackpackRef(byte[] serializedcustomobject)
  {
    return (object) IBinarySerializable.GetFromManagedArray<BackpackReference>(serializedcustomobject);
  }

  private static byte[] SerializeBackpackRef(object customobject)
  {
    return IBinarySerializable.ToManagedArray<BackpackReference>((BackpackReference) customobject);
  }

  private static object DeserializeItemData(byte[] serializedcustomobject)
  {
    NativeArray<byte> nativeArray = serializedcustomobject.ToNativeArray<byte>(Allocator.Temp);
    BinaryDeserializer deserializer = new BinaryDeserializer(nativeArray);
    Guid guid = deserializer.ReadGuid();
    ItemInstanceData o;
    if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out o))
    {
      o = new ItemInstanceData(guid);
      ItemInstanceDataHandler.AddInstanceData(o);
    }
    o.Deserialize(deserializer);
    nativeArray.Dispose();
    return (object) o;
  }

  private static byte[] SerializeItemData(object d)
  {
    ItemInstanceData itemInstanceData = (ItemInstanceData) d;
    BinarySerializer serializer = new BinarySerializer(24, Allocator.Temp);
    serializer.WriteGuid(itemInstanceData.guid);
    itemInstanceData.Serialize(serializer);
    byte[] byteArray = serializer.buffer.ToByteArray();
    serializer.Dispose();
    return byteArray;
  }

  public static object DeserializePhotonView(byte[] data)
  {
    return (object) PhotonView.Find(BitConverter.ToInt32(ReadOnlySpan<byte>.op_Implicit(data)));
  }

  public static byte[] SerializePhotonView(object customType)
  {
    return BitConverter.GetBytes(((PhotonView) customType).ViewID);
  }
}
