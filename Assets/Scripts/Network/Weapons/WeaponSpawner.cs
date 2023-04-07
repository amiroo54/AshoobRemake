using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System.Collections.Generic;
namespace Project.Network.Weapons
{
public class WeaponSpawner : NetworkBehaviour
{
    public static WeaponSpawner instance {get; private set;}
    public static List<GameObject> Weapons;
    private void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
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
        NetworkVariable<Vector3> SpawnPos = new NetworkVariable<Vector3>(new Vector3(0, 0, 0));
        if (IsHost)
        {
            //SpawnPos.Value = new Vector3(UnityEngine.Random.Range(GameManager.Instance.Res / 2, - GameManager.Instance.Res / 2), 100, UnityEngine.Random.Range(GameManager.Instance.Res / 2, -GameManager.Instance.Res/ 2));
        }
        Instantiate(Weapons[Random.Range(0, Weapons.Count)]);
    }
}}