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
    public int ChunkRes;
    public float Scale;
    public GameObject MapPrefab;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            
            Seed.Value = Random.Range(0, 10000);
        }
        for (int x = 0; x < Res; x++)
            {
                for (int y = 0; y < Res; y++)
                {
                    GameObject MapGO = Instantiate(MapPrefab, transform);
                    MapGO.transform.localPosition  = new Vector3(x * ChunkRes, 0, y * ChunkRes);
                    Noise map = MapGO.GetComponent<Noise>();
                    map.Res = ChunkRes + 1;
                    map.seed = Seed.Value;
                    map.OffsetX = x;
                    map.OffsetY = y;
                    map.Scale = Scale;
                    map.UpdateMesh();
                }
            }
        
    }
}
