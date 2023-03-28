using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class FistTypeWeaponSer : Weapon
{
    public float AttackForce;
    [ServerRpc]
    public override void MainServerRpc(PlayerMovement PlayerToAttack, PlayerMovement AttackingPlayer)
    {
        if (PlayerToAttack == AttackingPlayer)
        {
            AttackingPlayer.fist.AddForce((AttackingPlayer.torso.transform.forward) * AttackForce, ForceMode.Impulse);
        }
        else
        {
            AttackingPlayer.fist.AddForce(Vector3.Normalize(PlayerToAttack.torso.transform.position - AttackingPlayer.torso.transform.position) * AttackForce, ForceMode.Impulse);
        }
    }
    [ServerRpc]
    public override void SecondaryServerRpc()
    {
        
    }
    [ServerRpc]
    public override void StartServerRpc()
    {
    }
    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        base.NetworkSerialize(serializer);
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(AttackForce);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out AttackForce);
        }
    }
}