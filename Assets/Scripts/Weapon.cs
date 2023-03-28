using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;


public class Weapon : NetworkBehaviour, INetworkSerializable
{
    public Mesh displayMesh;
    public float Damage;
    public Material material;
    [ServerRpc]
    public virtual void StartServerRpc(){}
    [ServerRpc]
    public virtual void MainServerRpc(PlayerMovement PlayerToAttack, PlayerMovement AttackingPlayer){}
    [ServerRpc]
    public virtual void SecondaryServerRpc(){}
    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(Damage);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out Damage);
        }
    }
}