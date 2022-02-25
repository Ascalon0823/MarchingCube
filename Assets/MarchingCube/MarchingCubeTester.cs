using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class MarchingCubeTester : MonoBehaviour
{
    public Vector3 from;

    public Vector3 to;

    public float cubeSize;

    private List<Bounds> boxes;

    public Mesh mesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material meshMaterial;

    public float xOrg;

    public float yOrg;

    public float scale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        var marchingCube = new MarchingCube();
        boxes = new List<Bounds>();
        var data = marchingCube.March(from, to, cubeSize, (x) =>
        {
            var mul = 1f;
            x = x/10f* scale;
            float xy = Mathf.PerlinNoise(xOrg + x.x, yOrg + x.y);
            float xz = Mathf.PerlinNoise(xOrg + x.x, yOrg + x.z);
            float yz = Mathf.PerlinNoise(xOrg + x.y, yOrg + x.z);
            float yx = Mathf.PerlinNoise(xOrg + x.y, yOrg + x.x);
            float zx = Mathf.PerlinNoise(xOrg + x.z, yOrg + x.x);
            float zy = Mathf.PerlinNoise(xOrg + x.z, yOrg + x.y);

            return ((xy + xz + yz + yx + zx + zy) / 6f)*mul * 2f - 1f;
        }, 0, (min, max) => { boxes.Add(new Bounds((min + max) / 2f, (max - min))); });

        mesh = new Mesh()
        {
            indexFormat = IndexFormat.UInt32,
            vertices = data.Vertices.ToArray(),
            triangles = data.Triangles.ToArray()
        };


        if (!meshFilter)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        if (!meshRenderer)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer.sharedMaterial = meshMaterial;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (boxes == null) return;
        foreach (var box in boxes)
        {
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}