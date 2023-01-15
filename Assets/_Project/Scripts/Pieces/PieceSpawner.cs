using UnityEngine;
using Random = UnityEngine.Random;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private Piece piece;

    private Vector3 _offSet;
    private Vector2 _size;
    private float _height = -.1f;
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }


    public void CalculateBorders(float gridBorder)
    {
        var screenPoint = new Vector3(Screen.width, 0, 0);
        var worldPoint = _cam.ScreenToWorldPoint(screenPoint);
        _offSet.y = (worldPoint.y + gridBorder) / 2 - .5f;
        _size.y = Mathf.Abs(worldPoint.y - gridBorder) - 2;
        _size.x = worldPoint.x * 2 - 1;
    }

    public void SpawnPiece(PieceData pieceData)
    {
        var newPiece = Instantiate(piece, transform);
        var position = new Vector3(Random.Range(-_size.x / 2, _size.x / 2), Random.Range(-_size.y / 2, _size.y / 2), 0);
        position += _offSet;
        position.z = _height;
        _height -= .1f;
        newPiece.Initialize(pieceData, position, true);
    }

    public bool InBorder(Vector3 point)
    {
        return point.x <= _offSet.x + _size.x / 2 && point.x >= _offSet.x - _size.x / 2
                                                  && point.y <= _offSet.y + _size.y / 2 &&
                                                  point.y >= _offSet.y - _size.y / 2;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var origin = _offSet;
        var size = new Vector3(this._size.x, this._size.y, 1);
        Gizmos.DrawWireCube(origin, size);
    }

    public void DestroyPieces()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        _height = -.1f;
    }
}