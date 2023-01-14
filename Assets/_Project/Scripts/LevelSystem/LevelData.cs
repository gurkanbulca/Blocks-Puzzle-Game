using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public Vector2Int gridSize;
    public LevelDifficulty levelDifficulty;
    public PieceData[] pieces;
    public Vector3 origin;

    public LevelData(Vector2Int size, LevelDifficulty difficulty, PieceData[] pieces, Vector3 origin)
    {
        gridSize = size;
        levelDifficulty = difficulty;
        this.pieces = pieces;
        this.origin = origin;
    }
}