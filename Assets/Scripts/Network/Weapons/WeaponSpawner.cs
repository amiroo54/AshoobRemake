using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System.Collections.Generic;
namespace Project.Network.Weapons
{
public class WeaponSpawner : NetworkBehaviour
{
    public static WeaponSpawner instance {get; private set;}
    public List<GameObject> Weapons;
    NetworkVariable<Vector3> SpawnPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0));
    private void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
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
    }
    public void SpawnWeapon()
    {
        if (IsHost)
        {
            SpawnPos.Value = new Vector3(UnityEngine.Random.Range(MapSpawnManager.Instance.Res / 2, - MapSpawnManager.Instance.Res / 2), 100, UnityEngine.Random.Range(MapSpawnManager.Instance.Res / 2, -MapSpawnManager.Instance.Res/ 2));
            NetworkObject w = Instantiate(Weapons[Random.Range(0, Weapons.Count)]).GetComponent<NetworkObject>();
            w.transform.position = SpawnPos.Value;
            w.Spawn();
        }
    }
}}