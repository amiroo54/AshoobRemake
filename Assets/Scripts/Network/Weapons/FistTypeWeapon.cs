using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
namespace Project.Network.Weapons
{public class FistTypeWeaponSer : Weapon
{
    public float AttackForce;
    [ServerRpc(RequireOwnership=false)]
    public override void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer)
    {
        PlayerMovement p = GetPlayerByID(AttackingPlayer).GetComponent<PlayerMovement>();
        PlayerMovement d = GetPlayerByID(PlayerToAttack).GetComponent<PlayerMovement>();
        if (PlayerToAttack == AttackingPlayer)
        {
            p.fist.AddForce((p.torso.transform.forward) * AttackForce, ForceMode.Impulse);
        }
        else
        {
            p.fist.AddForce(Vector3.Normalize(d.torso.transform.position - p.torso.transform.position) * AttackForce, ForceMode.Impulse);
        }
    }
    [ServerRpc(RequireOwnership=false)]
    public override void SecondaryServerRpc()
    {
        
    }
    [ServerRpc(RequireOwnership=false)]
    public override void StartServerRpc()
    {
    }
}}