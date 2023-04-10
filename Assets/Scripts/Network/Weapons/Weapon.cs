using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Project.Network.Weapons
{public class Weapon : NetworkBehaviour
{
    private Mesh _displaymesh;
    public float Damage;
    private Material _material;
    public int index;
    [ServerRpc]
    public virtual void StartServerRpc(){}
    [ServerRpc]
    public virtual void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer){}
    [ServerRpc]
    public virtual void SecondaryServerRpc(){}
    public override void OnNetworkSpawn()
    {
        GetComponent<MeshRenderer>().material = _material;
        GetComponent<MeshFilter>().mesh = _displaymesh;
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
        GetPlayerByID(player).GetComponent<PlayerMovement>().ChangeWeapon(index);
        Destroy(this);
    }
    protected static GameObject GetPlayerByID(ulong id)
    {
        return NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
    }
}}