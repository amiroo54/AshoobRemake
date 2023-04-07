using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class Noise : MonoBehaviour
{
    private ComputeShader _shader;
    private int _res;

    private Gradient _grad;

    private int _seed;
    private Vector2[] _fna;

    [SerializeField] float _offsetX;
    [SerializeField] float _offsetY;
    private float _scale;
    private Vector2[] _uv;
    public float[] MinMaxData;
    private Mesh _finalmesh;
    private float _islandscale;
    private int _totalres;
    public void Construct(float _Scale, Vector2 _Offset, Vector2[] _FnA, Gradient _grad, int _res, int _seed, float _iSclae, int _TotalRes)
    {
        _scale = _Scale;
        _offsetX = _Offset.x;
        _offsetY = _Offset.y;
        _fna = _FnA;
        this._grad = _grad;
        this._res = _res;
        this._seed = _seed;
        _islandscale = _iSclae;
        _totalres = _TotalRes;
    }
    public void UpdateMeshVerts()
    {
        UnsafeUtility.SetLeakDetectionMode(Unity.Collections.NativeLeakDetectionMode.EnabledWithStackTrace);
        _shader = Resources.Load<ComputeShader>("PerlinNoise");
        _finalmesh = _getbasemesh();
    }
    public void UpdateMeshData(Vector2 minmax)
    {
        _finalmesh = _adddata(_finalmesh, minmax);
        GetComponent<MeshFilter>().mesh = _finalmesh;
        GetComponent<MeshCollider>().sharedMesh = _finalmesh;
    }

    private Vector3[] _getverts()
    {
        Vector3[] Vertecies = new Vector3[_res * _res];
        //setting the shader constants
        _shader.SetFloat("Scale", _scale);
        _shader.SetInt("Res", _res);
        _shader.SetFloat("AnFLenght", _fna.Length);
        _shader.SetInt("seed", _seed);
        _shader.SetFloats("Offset", new float[2]{_offsetX, _offsetY});
        _shader.SetFloat("IslandShapeScale", _islandscale);
        _shader.SetInt("TotalRes", _totalres);
        //setting Frequencies and Amplitudes
        ComputeBuffer FreqAndAmp = new ComputeBuffer(_fna.Length, sizeof(float) * 2 * _fna.Length);
        FreqAndAmp.SetData(_fna);
        _shader.SetBuffer(0, "FreqAndAmp", FreqAndAmp);

        ComputeBuffer Result = new ComputeBuffer(_res * _res, sizeof(float) * _res * _res * 3);
        _shader.SetBuffer(0, "Output", Result);

        MinMaxData = new float[]{10, 0};
        ComputeBuffer MinMax = new ComputeBuffer(2, sizeof(float) * 2);
        MinMax.SetData(MinMaxData);
        _shader.SetBuffer(0, "MinMaxHeight", MinMax);

        _shader.Dispatch(0, _res, _res, 1);
        
        Result.GetData(Vertecies);
        MinMax.GetData(MinMaxData);
        Result.Release();
        FreqAndAmp.Release();
        return Vertecies;
    }
    private int[] _gettris()
    {
        int[] meshDatas = new int[(_res) * (_res) * 6];
        ComputeBuffer meshDatasBuffer = new ComputeBuffer(meshDatas.Length, sizeof(int) * 3);
        meshDatasBuffer.SetData(meshDatas);
        _shader.SetBuffer(1, "meshdata", meshDatasBuffer);
        _shader.Dispatch(1, _res - 1, _res - 1, 1);
        meshDatasBuffer.GetData(meshDatas);
        meshDatasBuffer.Release();
        return meshDatas;
    }
    private VertData[] _getvertsdata(Mesh mesh, Vector2 minmax)
    {
        VertData[] vertDatas = new VertData[_res * _res];
        ComputeBuffer vertDataBuffer = new ComputeBuffer(vertDatas.Length, sizeof(float) * 3);
        vertDataBuffer.SetData(vertDatas);
        _shader.SetBuffer(3, "vertdata", vertDataBuffer);
        ComputeBuffer vertpos = new ComputeBuffer(mesh.vertexCount, sizeof(float) * 3);
        vertpos.SetData(mesh.vertices);
        _shader.SetBuffer(3, "Output", vertpos);
        _shader.SetFloat("minh", minmax.x);
        _shader.SetFloat("maxh", minmax.y);
        _shader.Dispatch(3, _res, _res, 1);
        vertDataBuffer.GetData(vertDatas);
        vertDataBuffer.Release();
        vertpos.Release();
        return vertDatas;
    }
    private Mesh _getbasemesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _getverts();
        mesh.triangles = _gettris();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.name = "Map";
        return mesh;
    }

    private Mesh _adddata(Mesh mesh, Vector2 minmax)
    {
        VertData[] vertData = _getvertsdata(mesh, minmax);
        
        mesh.colors = System.Array.ConvertAll<VertData, Color>(vertData, VertDatatoColor);
        mesh.uv = System.Array.ConvertAll<VertData, Vector2>(vertData, VertDatatoUV);
        return mesh;
    }
    Color VertDatatoColor(VertData n)
    {
        return _grad.Evaluate(n.color);
    }
    Vector2 VertDatatoUV(VertData n)
    {
        //Debug.Log(n.UVx + "  :  " + n.UVy);
        return n.UV;
    }
    public struct VertData
    {
        public float color;
        public Vector2 UV;
    }
}
