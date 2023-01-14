using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    private PieceData _pieceData;
    private Mesh _mesh;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");


    public void Initialize(PieceData pieceData, bool applyOrigin = false)
    {
        _pieceData = pieceData;
        if (applyOrigin)
            _pieceData.vertices = _pieceData.vertices.Select(vertex => vertex - _pieceData.origin).ToArray();

        GenerateMesh(pieceData.vertices, pieceData.triangles);
        SetMeshColor(pieceData.color);
    }

    private Vector3 CalculateOrigin(Vector3[] vertices)
    {
        var origin = Vector3.zero;
        foreach (var vertex in vertices)
        {
            origin += vertex;
        }

        origin /= vertices.Length;
        return origin;
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
}