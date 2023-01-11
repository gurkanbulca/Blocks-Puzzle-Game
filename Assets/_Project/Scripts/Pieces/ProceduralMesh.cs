using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;

    private Mesh _mesh;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        GenerateTestMeshData();
        Initialize(vertices, triangles, Color.yellow);
    }

    public void Initialize(Vector3[] vertices, int[] triangles, Color color)
    {
        GenerateMesh(vertices, triangles);
        SetMeshColor(color);
    }

    private void SetMeshColor(Color color)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor(BaseColor, color);
        GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }

    private void GenerateMesh(Vector3[] vertices, int[] triangles)
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
    }

    private void GenerateTestMeshData()
    {
        vertices = new[]
        {
            new Vector3(-.5f, .5f, 0),
            new Vector3(.5f, -.5f, 0),
            new Vector3(-.5f, -.5f, 0),
            new Vector3(.5f, .5f, 0)
        };
        triangles = new[] {0, 1, 2, 0, 3, 1};
    }
}