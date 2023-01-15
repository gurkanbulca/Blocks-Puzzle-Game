using System.Collections.Generic;
using CameraSystem;
using DG.Tweening;
using GameStateSystem;
using GridSystem;
using InputSystem;
using LevelSystem;
using Pieces;
using UnityEngine;

namespace GameCore
{
    public class GameManager : MonoBehaviour
    {
        private GridController _gridController;
        private GridData _gridData;
        private LevelData _levelData;
        private PieceSpawner _pieceSpawner;
        private GridDrawer _gridDrawer;
        private InputController _inputController;
        private CameraController _cameraController;
        private GameStateController _gameStateController;
        private int _placedPieceCount;

        public int CurrentLevelIndex => LevelController.CurrentLevelIndex;
        public string CurrentLevelDifficulty => _levelData.levelDifficulty.ToString();

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _pieceSpawner = FindObjectOfType<PieceSpawner>();
            _gridDrawer = FindObjectOfType<GridDrawer>();
        }

        private void Start()
        {
            LoadRandomLevel();
        }

        private void Update()
        {
            _inputController.Tick();
        }

        private void LoadRandomLevel()
        {
            _placedPieceCount = 0;
            _levelData = LevelController.GetRandomLevelData();
            CreateGrid(_levelData);
            _cameraController = new CameraController(Camera.main);
            _cameraController.SetCameraPosition(_levelData.gridSize);
            _pieceSpawner.DestroyPieces();
            _pieceSpawner.CalculateBorders(_gridController.grid[0, _gridController.grid.GetLength(1) - 1].Position.y -
                                           .5f);
            foreach (var pieceData in _levelData.pieces)
            {
                _pieceSpawner.SpawnPiece(pieceData);
            }

            _gridDrawer.Initialize(_gridController.grid);
            _inputController = new InputController(_levelData.pieces.Length);
            _inputController.OnPieceDrop += HandlePieceDrop;
            _inputController.OnPiecePicked += HandlePiecePick;
            if (_gameStateController == null)
                _gameStateController = new GameStateController(GameState.Play);
            else
                _gameStateController.CurrentGameState = GameState.Play;
        }

        private void CreateGrid(LevelData levelData)
        {
            _gridData = new GridData(levelData.gridSize, levelData.origin, 1);
            _gridController = new GridController(_gridData);
            _gridController.GenerateGrid();
        }

        private void HandlePieceDrop(Piece piece)
        {
            if (!piece)
                return;
            var piecePosition = piece.transform.position;
            if (!_gridController.IsPointInGridBorders(piecePosition))
            {
                Debug.Log("Grid border fail!");
                if (_pieceSpawner.InBorder(piecePosition))
                    piece.SetStartPosition(piecePosition);
                else
                    piece.ReturnToStartPosition();
                return;
            }

            var tris = new List<Tri>();
            var pieceTris = piece.Tris;
            var offSet = Vector3.zero;
            for (var i = 0; i < pieceTris.Length; i++)
            {
                var pieceTri = pieceTris[i];
                var triPosition = piecePosition + pieceTri.Origin;
                var tri = _gridController.GetClosestCell(triPosition + offSet).GetClosestTri(triPosition + offSet);
                if (tri.IsFull || tris.Contains(tri))
                {
                    Debug.Log("Tri already full!");
                    piece.ReturnToStartPosition();
                    return;
                }

                if (i == 0)
                {
                    offSet = tri.Origin - triPosition;
                }

                tris.Add(tri);
            }

            foreach (var tri in tris)
            {
                tri.IsFull = true;
            }

            var translate = tris[0].Origin - pieceTris[0].Origin - piecePosition;
            piece.transform.DOMove(piecePosition + translate, .25f);
            piece.isPlaced = true;
            UpdatePlacedPieceCount(1);
        }


        private void HandlePiecePick(Piece piece)
        {
            if (!piece || !piece.isPlaced)
                return;
            var piecePosition = piece.transform.position;
            foreach (var pieceTri in piece.Tris)
            {
                var triPosition = pieceTri.Origin + piecePosition;
                _gridController.GetClosestCell(triPosition).GetClosestTri(triPosition).IsFull = false;
            }

            piece.isPlaced = false;
            UpdatePlacedPieceCount(-1);
        }


        private void UpdatePlacedPieceCount(int update)
        {
            _placedPieceCount = Mathf.Clamp(_placedPieceCount + update, 0, _levelData.pieces.Length);
            if (_placedPieceCount == _levelData.pieces.Length)
            {
                _gameStateController.CurrentGameState = GameState.Success;
            }
        }

        public void CompleteLevel()
        {
            LoadRandomLevel();
        }
    }
}