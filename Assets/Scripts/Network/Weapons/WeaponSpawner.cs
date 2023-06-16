using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System.Collections.Generic;
namespace Project.Network.Weapons
{
public class WeaponSpawner : NetworkBehaviour
{
    public static WeaponSpawner instance {get; private set;}
    public static GameObject[] Weapons {get; private set;}
    [SerializeField] GameObject[] _weapons;
    private void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
            WeaponSpawner.Weapons = _weapons;
            for (int w = 0; w < Weapons.Length; w++)
            {
                NetworkManager.Singleton.AddNetworkPrefab(Weapons[w]);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(spawn());
    }
    IEnumerator spawn()
    {
        SpawnWeapon();
        yield return new WaitForSeconds(10);
        StartCoroutine(spawn());
    }
    public void SpawnWeapon()
    {
        if (IsHost)
        {
            GameObject o = Instantiate(Weapons[Random.Range(0, Weapons.Length)]);
            o.GetComponent<NetworkObject>().Spawn();
        }
        
    }
}}