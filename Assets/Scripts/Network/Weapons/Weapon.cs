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
        StartServerRpc();
        PlayerMovement m = GetPlayerByID(player).GetComponentInParent<PlayerMovement>();
        transform.parent = m.fist.gameObject.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        GetComponent<Collider>().enabled = false;
        GetComponent<NetworkObject>().enabled = false;
        m.CurrentWeapon = this;
    }
    protected static GameObject GetPlayerByID(ulong id)
    {
        return NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
    }
}}