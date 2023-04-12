using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Project.Network.Weapons
{public class Weapon : NetworkBehaviour
{
    public float Damage;
    [ServerRpc]
    public virtual void StartServerRpc(){}
    [ServerRpc]
    public virtual void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer){}
    [ServerRpc]
    public virtual void SecondaryServerRpc(){}
    public Transform Parent;
    private void Update() {
        if (Parent != null)
        {
            transform.position = Parent.position;
            transform.rotation = Parent.rotation;
        } 
    }
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PickUpWeaponServerRpc(other.gameObject.GetComponentInParent<PlayerMovement>().PlayerIndex);
        }
    }
    [ServerRpc]
    public void PickUpWeaponServerRpc(ulong player)
    {
        GetPlayerByID(player).GetComponent<PlayerMovement>().ChangeWeaponServerRpc(NetworkObjectId);
        
    }
    protected static GameObject GetPlayerByID(ulong id)
    {
        return NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
    }
}}