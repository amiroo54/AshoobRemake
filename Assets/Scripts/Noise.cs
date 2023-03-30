using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public ComputeShader Shader;
    public int Res;

    public Gradient grad;

    public int seed;
    public Vector2[] FnA;

    public float OffsetX;
    public float OffsetY;
    public float Scale;

    private Vector3[] Vertecies;
    private Vector2[] UV;

    public void UpdateMesh()
    {
        print(seed);
        Compute();
        Mesh Readymesh = TexGen(AddVertsAndTris());
        Readymesh.Optimize();
        GetComponent<MeshFilter>().mesh = Readymesh;
        GetComponent<MeshCollider>().sharedMesh = Readymesh;
    }

    public void Compute()
    {
        Vertecies = new Vector3[Res * Res];
        //setting the shader constants
        Shader.SetFloat("Scale", Scale);
        Shader.SetFloat("Res", Res);
        Shader.SetFloat("AnFLenght", FnA.Length);
        Shader.SetInt("seed", seed);
        Shader.SetFloats("Offset", new float[2]{OffsetX, OffsetY});
        //setting Frequencies and Amplitudes
        ComputeBuffer FreqAndAmp = new ComputeBuffer(FnA.Length, sizeof(float) * 2 * FnA.Length);
        FreqAndAmp.SetData(FnA);
        Shader.SetBuffer(0, "FreqAndAmp", FreqAndAmp);

        ComputeBuffer Result = new ComputeBuffer(Res * Res, sizeof(float) * Res * Res * 3);
        Shader.SetBuffer(0, "Output", Result);

        ComputeBuffer MinMax = new ComputeBuffer(1, sizeof(float) * 2);
        Shader.SetBuffer(0, "MinMax", MinMax);

        Shader.Dispatch(0, Res, Res, 1);
        
        Result.GetData(Vertecies);

        Result.Release();
        FreqAndAmp.Release();

    }

    private Mesh TexGen(Mesh mesh)
    {
        Color[] colors = new Color[Vertecies.Length];
        for (int i = 0; i < Vertecies.Length; i++)
        {
            colors[i] = grad.Evaluate(Vertecies[i].y / FnA[0].y);
        }
        mesh.colors = colors;
        return mesh;
    }
    public Mesh AddVertsAndTris()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertecies;
        int[] triangles = new int[Res * Res * 6];
        int triIndex = 0;

       
        int[,] simplevertex = vertexsimplyfi();


        //adding tris based on the x and y.
        for (int x = 0; x < Res - 1; x++)
        {
            for (int y = 0; y < Res - 1; y++)
            {
                triangles[triIndex + 0] = simplevertex[x, y];
                triangles[triIndex + 1] = simplevertex[x, y + 1];
                triangles[triIndex + 2] = simplevertex[x + 1, y];
                triangles[triIndex + 3] = simplevertex[x + 1, y];
                triangles[triIndex + 4] = simplevertex[x, y + 1];
                triangles[triIndex + 5] = simplevertex[x + 1, y + 1];
                triIndex += 6;
            }
        }
        mesh.triangles = triangles;
        mesh.uv = UV;
        mesh.RecalculateNormals();
        mesh.name = "Map";
        return mesh;
    }


    
    //this is for converting x and y coords into a single index.
    int[,] vertexsimplyfi()
    {
        int[,] simplevertex = new int[Res, Res];
        int index = 0;
        UV = new Vector2[Res * Res];
        for (int x = 0; x < Res; x++)
        {
            for (int y = 0; y < Res; y++)
            {
                simplevertex[x, y] = index;
                UV[index] = new Vector2(x, y);
                index++;

            }
        }
        return simplevertex;
    }


}
