using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class MapSpawnManager : NetworkBehaviour
{    
    private NetworkVariable<int> _seed = new NetworkVariable<int>(0);
    public int Res;
    [SerializeField] int _chunkRes;
    [SerializeField] float _scale;
    [SerializeField] Vector2[] _freqandamp;
    [SerializeField] Gradient[] _grad;
    private Noise[] Maps;
    [SerializeField] Material _mapmat;
    [SerializeField] float _islandshapescale;
    [SerializeField] Transform _water;
    [SerializeField] private Vector2 _minmaxdata;
    [SerializeField] GameObject[] _dontDestroy; 
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
        _minmaxdata = new Vector2(Mathf.Infinity, Mathf.NegativeInfinity);
        if (Res % 2 == 1)
        {
            Res++;
        }
        Maps = new Noise[Res * Res];
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            bool isinlist = false;
            foreach (GameObject d in _dontDestroy)
            {
                if (d == child.gameObject)
                {
                    isinlist = true;
                    break;
                }
            }
            if (!isinlist)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        Gradient mapcolor = _grad[UnityEngine.Random.Range(0, _grad.Length)];
        for (int x = 0; x < Res; x++)
        {
            for (int y = 0; y < Res; y++)
            {
                if (Maps[x * Res + y] != null)
                {
                    DestroyImmediate(Maps[x * Res + y]);
                }
                GameObject MapGO = new GameObject("map", new Type[]{typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider)});
                MapGO.isStatic = true;
                Noise map = MapGO.AddComponent<Noise>();
                Vector2 offset = new Vector2(x - Res / 2, y - Res/2);
                map.Construct(_scale, offset, _freqandamp, mapcolor, _chunkRes, _seed.Value, _islandshapescale, Res);
                MapGO.transform.parent = transform;
                float mapPosOffset = ((_chunkRes - 1) * Res / 2);
                MapGO.transform.localPosition  = new Vector3(x * (_chunkRes - 1) - mapPosOffset , 0, y * (_chunkRes - 1) - mapPosOffset);
                MapGO.transform.localScale = Vector3.one;
                MapGO.GetComponent<MeshRenderer>().material = _mapmat;
                map.UpdateMeshVerts();
                if (map.MinMaxData[0] < _minmaxdata[0])
                {
                    _minmaxdata[0] = map.MinMaxData[0];
                }
                if (map.MinMaxData[1] > _minmaxdata[1])
                {
                    _minmaxdata[1] = map.MinMaxData[1];
                }
                Maps[x * Res + y] = map;
            }
        }
        for (int x = 0; x < Res; x++)
        {
            for (int y = 0; y < Res; y++)
            {
                Noise map = Maps[x * Res + y]; 
                map.UpdateMeshData(_minmaxdata);
                Maps[x * Res + y] = map;
            }
        }
        if (_water != null)
        {
            float WaterScale = Res * _chunkRes / 2 - (Res / 2);
            _water.localScale = new Vector3(WaterScale, WaterScale, WaterScale);
        }
        
    }
}
