using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class Noise : MonoBehaviour
{
    private ComputeShader CShader;
    private int Res;

    private Gradient grad;

    private int seed;
    private Vector2[] FnA;

    private float OffsetX;
    private float OffsetY;
    private float Scale;
    public Vector3 SideMeshVerts;
    private Vector2[] UV;
    public float[] MinMaxData;
    public Mesh Readymesh;
    public void Construct(float _Scale, Vector2 _Offset, Vector2[] _FnA, Gradient _grad, int _res, int _seed)
    {
        Scale = _Scale;
        OffsetX = _Offset.x;
        OffsetY = _Offset.y;
        FnA = _FnA;
        grad = _grad;
        Res = _res;
        seed = _seed;
    }
    public void UpdateMeshVerts()
    {
        UnsafeUtility.SetLeakDetectionMode(Unity.Collections.NativeLeakDetectionMode.EnabledWithStackTrace);
        CShader = Resources.Load<ComputeShader>("PerlinNoise");
        Readymesh = AddVertsAndTrisAndColors();
        Readymesh.Optimize();
    }
    public void UpdateMeshData()
    {
        Readymesh = AddData(Readymesh);
        GetComponent<MeshFilter>().mesh = Readymesh;
        GetComponent<MeshCollider>().sharedMesh = Readymesh;
    }

    public Vector3[] GetVerteces()
    {
        Vector3[] Vertecies = new Vector3[Res * Res];
        //setting the shader constants
        CShader.SetFloat("Scale", Scale);
        CShader.SetInt("Res", Res);
        CShader.SetFloat("AnFLenght", FnA.Length);
        CShader.SetInt("seed", seed);
        CShader.SetFloats("Offset", new float[2]{OffsetX, OffsetY});
        //setting Frequencies and Amplitudes
        ComputeBuffer FreqAndAmp = new ComputeBuffer(FnA.Length, sizeof(float) * 2 * FnA.Length);
        FreqAndAmp.SetData(FnA);
        CShader.SetBuffer(0, "FreqAndAmp", FreqAndAmp);

        ComputeBuffer Result = new ComputeBuffer(Res * Res, sizeof(float) * Res * Res * 3);
        CShader.SetBuffer(0, "Output", Result);

        MinMaxData = new float[]{10, 0};
        ComputeBuffer MinMax = new ComputeBuffer(2, sizeof(float) * 2);
        MinMax.SetData(MinMaxData);
        CShader.SetBuffer(0, "MinMaxHeight", MinMax);

        CShader.Dispatch(0, Res, Res, 1);
        
        Result.GetData(Vertecies);

        
        MinMax.GetData(MinMaxData);
        Result.Release();
        FreqAndAmp.Release();
        return Vertecies;
    }
    public int[] GetTris()
    {
        int[] meshDatas = new int[(Res) * (Res) * 6];
        ComputeBuffer meshDatasBuffer = new ComputeBuffer(meshDatas.Length, sizeof(int) * 3);
        meshDatasBuffer.SetData(meshDatas);
        CShader.SetBuffer(1, "meshdata", meshDatasBuffer);
        CShader.Dispatch(1, Res - 1, Res - 1, 1);
        meshDatasBuffer.GetData(meshDatas);
        meshDatasBuffer.Release();
        return meshDatas;
    }
    public VertData[] GetVertDatas(Mesh mesh)
    {
        CShader = Resources.Load<ComputeShader>("PerlinNoise");
        VertData[] vertDatas = new VertData[Res * Res];
        ComputeBuffer vertDataBuffer = new ComputeBuffer(vertDatas.Length, sizeof(float) * 3);
        vertDataBuffer.SetData(vertDatas);
        CShader.SetInt("Res", Res);
        CShader.SetBuffer(3, "vertdata", vertDataBuffer);
        CShader.Dispatch(3, Res, Res, 1);
        vertDataBuffer.GetData(vertDatas);
        vertDataBuffer.Release();
        return vertDatas;
    }
    public Mesh AddVertsAndTrisAndColors()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = GetVerteces();
        mesh.triangles = GetTris();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.name = "Map";
        return mesh;
    }

    public Mesh AddData(Mesh mesh)
    {
        VertData[] vertData = GetVertDatas(mesh);
        mesh.colors = System.Array.ConvertAll<VertData, Color>(vertData, VertDatatoColor);
        mesh.SetUVs(0, System.Array.ConvertAll<VertData, Vector2>(vertData, VertDatatoUV));
        return mesh;
    }
    Color VertDatatoColor(VertData n)
    {
        return grad.Evaluate(n.color);
    }
    Vector2 VertDatatoUV(VertData n)
    {
        Debug.Log(n.UVx + "  :  " + n.UVy);
        return new Vector2(n.UVx, n.UVy);
    }
    //this is for converting x and y coords into a single index.
    public struct VertData
    {
        public float UVx;
        public float UVy;
        public float color;
    }
}
