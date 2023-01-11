using System;
using UnityEngine;

[Serializable]
public class LevelData
{
    public Vector2Int gridSize;
    public LevelDifficulty levelDifficulty;
    public PieceData[] pieces;

    public LevelData(Vector2Int size, LevelDifficulty difficulty, PieceData[] pieces)
    {
        gridSize = size;
        levelDifficulty = difficulty;
        this.pieces = pieces;
    }

}