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
                Weapons[w].GetComponent<Weapon>().index = w;
            }
        }
        foreach (GameObject g in Weapons)
        {
            NetworkManager.Singleton.AddNetworkPrefab(g);
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
            Instantiate(Weapons[Random.Range(0, Weapons.Length)]);
        }
        
    }
}}