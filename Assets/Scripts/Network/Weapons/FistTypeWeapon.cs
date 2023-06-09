using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;
namespace Project.Network.Weapons
{public class FistTypeWeapon : Weapon
{
    public float AttackForce;
    [ServerRpc]
    public override void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer)
    {
        Player p = GetPlayerByID(AttackingPlayer).GetComponent<Player>();
        Player d = GetPlayerByID(PlayerToAttack).GetComponent<Player>();
        if (PlayerToAttack == AttackingPlayer)
        {
            p.fist.AddForce((p.torso.transform.forward) * AttackForce, ForceMode.Impulse);
        }
        else
        {
            p.fist.AddForce(Vector3.Normalize(d.torso.transform.position - p.torso.transform.position) * AttackForce, ForceMode.Impulse);
        }
    }
    [ServerRpc]
    public override void SecondaryServerRpc()
    {
        
    }
}}