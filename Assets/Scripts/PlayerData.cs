

using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
   public ulong clientID;
   public int colorId;
   public FixedString64Bytes playerName;
   public FixedString64Bytes playerId;

   public bool Equals(PlayerData other)
   {
      return clientID == other.clientID && colorId == other.colorId && playerName == other.playerName && playerId == other.playerId;
   }

   public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
   {
      serializer.SerializeValue(ref clientID);
      serializer.SerializeValue(ref colorId);
      serializer.SerializeValue(ref playerName);
      serializer.SerializeValue(ref playerId);
   }
}
