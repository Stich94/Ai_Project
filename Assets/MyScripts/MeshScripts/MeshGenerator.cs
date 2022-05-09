using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int worldx;
    [SerializeField] private int worldz;

    private Mesh mesh;
    private MeshRenderer meshRen;
    private MeshFilter meshFilter;
    
    // arrays for mesh
    private int[] tris;
    private Vector3[] verts;

    private void Awake()
    {
        meshRen = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "CustomPlane";
        

    }

    private void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        tris = new int[worldx * worldz * 6];
        verts = new Vector3[(worldx +1) * (worldz + 1)];

        for (int i = 0,  z = 0; z <= worldz; z++)
        {
            for (int x = 0; x <= worldx; x++)
            {
                verts[i] = new Vector3(x, 0, z);
                i++;
            }
        }
        
        int triIdx = 0;
        int vertIdx = 0;

        for (int z = 0; z < worldz; z++)
        {
            for (int x = 0; x < worldx; x++)
            {
                // triangle1 [0, 1, 2]
                tris[triIdx + 0] = vertIdx + 0;
                tris[triIdx + 1] = vertIdx + worldz + 1;
                tris[triIdx + 2] = vertIdx + 1;
                
                // triangle2 [0, 2, 3]
                tris[triIdx + 3] = vertIdx + 1;
                tris[triIdx + 4] = vertIdx + worldz + 1;
                tris[triIdx + 5] = vertIdx + worldz + 2;
                
                
                vertIdx++;
                triIdx += 6;
                // full quad
            }
            vertIdx++;
        }
        mesh.Clear();
        mesh.vertices = verts; // the vertices always must be assigned first
        mesh.triangles = tris;
        
        mesh.RecalculateNormals();
    }

    private void UpdateMesh()
    {
        
    }

    private void Update()
    {
        UpdateMesh();
    }
}
