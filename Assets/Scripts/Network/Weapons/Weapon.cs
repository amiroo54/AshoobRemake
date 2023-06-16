using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

namespace Project.Network.Weapons
{public class Weapon : NetworkBehaviour
{
    public int Damage;
    [ServerRpc]
    public virtual void StartServerRpc()
    {
        Destroy(GetComponent<NetworkRigidbody>());
        Destroy(GetComponent<NetworkTransform>());
        Destroy(GetComponent<Rigidbody>());
        GetComponent<Collider>().enabled = false;
    }
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
    protected virtual void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PickUpWeaponServerRpc(other.gameObject.GetComponentInParent<Player>().PlayerIndex);
        }
    }
    [ServerRpc]
    public void PickUpWeaponServerRpc(ulong player)
    {
        GetPlayerByID(player).GetComponent<Player>().ChangeWeaponServerRpc(NetworkObjectId);
        
    }
    public static GameObject GetPlayerByID(ulong id)
    {
        //Debug.Log(NetworkManager.Singleton.ConnectedClients[id]);
        return NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
    }
}}