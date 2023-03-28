using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public static GameManager instance {get; private set;}
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
    private NetworkVariable<int> Seed = new NetworkVariable<int>(0);
    public int Res;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            Seed.Value = Random.Range(0, 10000);
        }
        Noise map = GameObject.FindObjectOfType<Noise>();
        map.Res = Res;
        map.seed = Seed.Value;
        map.UpdateMesh();
    }
}
