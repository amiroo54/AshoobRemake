using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class MapSpawnManager : NetworkBehaviour
{
    public static MapSpawnManager Instance {get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else 
        {
            Instance = this;
        }
    }
    private NetworkVariable<int> _seed = new NetworkVariable<int>(0);
    public int Res;
    [SerializeField] int _chunkRes;
    [SerializeField] float _scale;
    [SerializeField] Vector2[] _freqandamp;
    [SerializeField] Gradient _grad;
    private Noise[] Maps;
    [SerializeField] Material _mapmat;
    [SerializeField] float _islandshapescale;
    [SerializeField] Transform _water;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            _seed.Value = UnityEngine.Random.Range(0, 10000);
        }
        
        SpawnMaps();
    }
    public void SpawnMaps()
    {   
        if (Res % 2 == 1)
        {
            Res++;
        }
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
                Vector2 offset = new Vector2(x - Res / 2, y - Res/2);
                map.Construct(_scale, offset, _freqandamp, _grad, _chunkRes, _seed.Value, _islandshapescale, Res);
                MapGO.transform.parent = transform;
                float mapPosOffset = ((_chunkRes - 1) * Res / 2);
                MapGO.transform.localPosition  = new Vector3(x * (_chunkRes - 1) - mapPosOffset , 0, y * (_chunkRes - 1) - mapPosOffset);
                MapGO.GetComponent<MeshRenderer>().material = _mapmat;
                map.UpdateMeshVerts();
                map.UpdateMeshData();
                Maps[x * Res + y] = map;
            }
        }
        float WaterScale = Res * _chunkRes / 2 - (Res / 2);
        _water.localScale = new Vector3(WaterScale, WaterScale, WaterScale);
    }
}
