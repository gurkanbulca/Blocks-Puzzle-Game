using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private ProceduralMesh proceduralMesh;
    [SerializeField] private Vector3 offSet;
    [SerializeField] private Vector2 size;

    public void SpawnPiece(PieceData pieceData)
    {
        var piece = Instantiate(proceduralMesh, transform);
        var position = new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), 0);
        position += offSet;
        piece.transform.position = position;
        piece.Initialize(pieceData, true);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var origin = offSet;
        var size = new Vector3(this.size.x, this.size.y, 1);
        Gizmos.DrawWireCube(origin, size);
    }
}