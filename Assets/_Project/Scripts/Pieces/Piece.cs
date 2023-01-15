using System;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Piece : MonoBehaviour
{
    [ReadOnly] public bool isPlaced;
    
    private PieceData _pieceData;
    private Mesh _mesh;
    public PieceTriangle[] Tris { get; private set; }
    public Vector3 StartPosition { get; private set; }

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void OnDestroy()
    {
        transform.DOKill();
    }


    public void Initialize(PieceData pieceData, Vector3 startPosition, bool applyOrigin = false)
    {
        _pieceData = pieceData;
        StartPosition = startPosition;
        transform.position = startPosition;
        if (applyOrigin)
            _pieceData.vertices = _pieceData.vertices.Select(vertex => vertex - _pieceData.origin).ToArray();

        GenerateMesh(pieceData.vertices, pieceData.triangles);
        SetMeshColor(pieceData.color);
        CalculateTris();
    }


    private void CalculateTris()
    {
        Tris = new PieceTriangle[_pieceData.triangles.Length / 3];
        for (int i = 0; i < _pieceData.triangles.Length; i += 3)
        {
            var tri = new PieceTriangle(new[]
            {
                _pieceData.vertices[_pieceData.triangles[i]],
                _pieceData.vertices[_pieceData.triangles[i + 1]],
                _pieceData.vertices[_pieceData.triangles[i + 2]]
            });
            Tris[i / 3] = tri;
        }
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
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
    }

    public void ReturnToStartPosition()
    {
        transform.DOMove(StartPosition, .5f);
    }

    public Vector3 GetClosestTriangleOrigin(Vector3 position)
    {
        var selfPosition = transform.position;
        PieceTriangle closestTriangle = null;
        float closestDistance = Mathf.Infinity;

        foreach (var triangle in Tris)
        {
            var distance = Vector3.Distance(position, selfPosition + triangle.Origin);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTriangle = triangle;
            }
        }

        return closestTriangle?.Origin ?? Vector3.zero;
    }

    public void SetStartPosition(Vector3 position)
    {
        StartPosition = position;
    }
}

public class PieceTriangle
{
    public Vector3 Origin;

    public PieceTriangle(Vector3[] points)
    {
        Origin = Vector3.zero;
        foreach (var point in points)
        {
            Origin += point;
        }

        Origin /= points.Length;
    }
}