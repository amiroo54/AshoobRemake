using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
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
    public Vector2[] FnA;
    public Gradient grad;
    public Noise[] Maps;
    public Material mapmat;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            Seed.Value = UnityEngine.Random.Range(0, 10000);
        }
        
        SpawnMaps();
    }
    public void SpawnMaps()
    {   
        Maps = new Noise[Res * Res];
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != this.gameObject)
            {
            DestroyImmediate(child.gameObject);
            }
        }
        for (int x = 0; x < Res; x++)
        {
            for (int y = 0; y < Res; y++)
            {
                if (Maps[x * Res + y] != null)
                {
                    DestroyImmediate(Maps[x * Res + y]);
                }
                GameObject MapGO = new GameObject("map", new Type[]{typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider)});
                Noise map = MapGO.AddComponent<Noise>();
                map.Construct(Scale, new Vector2(x, y), FnA, grad, ChunkRes, Seed.Value);
                MapGO.transform.parent = transform;
                MapGO.transform.localPosition  = new Vector3(x * (ChunkRes - 1), 0, y * (ChunkRes - 1));
                MapGO.GetComponent<MeshRenderer>().material = mapmat;
                map.UpdateMeshVerts();
                map.UpdateMeshData();
                Maps[x * Res + y] = map;
            }
        }
    }
}
