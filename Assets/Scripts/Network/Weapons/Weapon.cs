using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Project.Network.Weapons
{public class Weapon : NetworkBehaviour
{
    public float Damage;
    public int index;
    [ServerRpc]
    public virtual void StartServerRpc(){}
    [ServerRpc]
    public virtual void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer){}
    [ServerRpc]
    public virtual void SecondaryServerRpc(){}
    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("Collidsion  " + OwnerClientId);
        if (other.gameObject.CompareTag("Player"))
        {
            PickUpWeaponServerRpc(other.gameObject.GetComponentInParent<PlayerMovement>().PlayerIndex);
        }
    }
    [ServerRpc]
    public void PickUpWeaponServerRpc(ulong player)
    {
        GetPlayerByID(player).GetComponent<PlayerMovement>().ChangeWeapon(index);
        Destroy(this);
    }
    protected static GameObject GetPlayerByID(ulong id)
    {
        return NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
    }
}}