using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class TerrainType
{
    public string name;
    public float height;
    public Color color;
}

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class GridScript : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public float maxheight = 1;
    public float PerlinScale = 10;

    // Start is called before the first frame update
    void Start()
    {
        Generator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Generator()
    {
        Vector3[] vertexbuffer = new Vector3[(xSize +1 ) * (ySize + 1) ];
        int[] indexbuffer = new int[xSize * ySize * 6];

        float[,] heightmap = GenerateNoiseMap(ySize + 1, xSize + 1, PerlinScale);
        Texture2D heightTexture = BuildTexture(heightmap);
        // uv map
        Vector2[] uvbuffer = new Vector2[(xSize +1 ) * (ySize + 1) ];

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++,i++)
            {
                vertexbuffer[i] = new Vector3(x, maxheight *  heightmap[y,x], y); // <- add heightmap rather than perlin noise
                Debug.Log(vertexbuffer[i]);
                uvbuffer[i] = new Vector2( (float)y / (float)ySize, (float)x / (float)xSize);
            }      
        }

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti +=6, vi++)
            {
                indexbuffer[ti] = vi;
                indexbuffer[ti + 3] = indexbuffer[ti + 2] = vi + 1;
                indexbuffer[ti + 4] = indexbuffer[ti + 1] = vi + xSize + 1;
                indexbuffer[ti + 5] =  vi + xSize + 2;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertexbuffer;
        mesh.triangles = indexbuffer;
        mesh.uv = uvbuffer;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material.mainTexture = heightTexture; // <- setz die texture auf die heightmap
    }
    

    /// <summary>
    /// generate a random heightmap
    /// </summary>
    /// <param name="zGrid"></param>
    /// <param name="xGrid"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public float[,] GenerateNoiseMap(int zGrid, int xGrid, float scale)
    {
        float[,] noise = new float[zGrid, xGrid];

        for (int zIndex = 0; zIndex < zGrid; zIndex++)
        {
            for (int xIndex = 0; xIndex < xGrid; xIndex++)
            {
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;
                
                noise[zIndex, xIndex] = Mathf.PerlinNoise(sampleX, sampleZ);
            }
        }
        return noise;
    }

    private Texture2D BuildTexture(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);
        
        Color[] colorMap = new Color[depth * width];
        
        for (int zIndex = 0; zIndex < depth; zIndex++)
        {
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                int colorIndex = zIndex * width + xIndex;
                float height = heightMap[zIndex, xIndex]; // da ist irgendeine farbe drin, diesen möchten wir nun in farbe umwandeln
                colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height); // <- hier wandeln wir den perlin wert in farbe um
            }
        }
        
        Texture2D texture = new Texture2D(width, depth);
        texture.wrapMode = TextureWrapMode.Clamp;
        // da unsere texture noch leer ist
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.black;
    //
    //     for (int i = 0; i < GetComponent<MeshFilter>().mesh.vertexCount; i++)
    //         Gizmos.DrawSphere(GetComponent<MeshFilter>().mesh.vertices[i], 0.1f);
    // }
}
