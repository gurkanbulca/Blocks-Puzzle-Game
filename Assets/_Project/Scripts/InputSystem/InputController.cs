using System;
using Pieces;
using UnityEngine;
using Utils;

namespace InputSystem
{
    public class InputController
    {
        public event Action<Piece> OnPieceDrop = delegate { };
        public event Action<Piece> OnPiecePicked = delegate { };

        private Camera _cam;
        private RaycastHit[] _hits;
        private Piece _dragPiece;
        private Vector3 _dragOffset;

        public InputController(int pieceCount)
        {
            _cam = Camera.main;
            _hits = new RaycastHit[pieceCount];
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
                RayCastToPiece();

            if (Input.GetMouseButtonUp(0))
                DropPiece();

            DragPiece();
        }

        private void DragPiece()
        {
            if (!_dragPiece)
                return;
            var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            _dragPiece.transform.position = mousePosition.WithZ(_dragPiece.transform.position.z) - _dragOffset;
        }

        private void DropPiece()
        {
            OnPieceDrop(_dragPiece);
            _dragPiece = null;
        }

        private void RayCastToPiece()
        {
            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            var hitCount = Physics.RaycastNonAlloc(ray, _hits, Mathf.Infinity);
            if (hitCount == 0)
                return;
            _dragPiece = _hits[0].transform.GetComponent<Piece>();
            _dragOffset = _hits[0].point - _dragPiece.transform.position;
            for (var i = 1; i < hitCount; i++)
            {
                if (!(_hits[i].point.z < _dragPiece.transform.position.z)) continue;

                _dragPiece = _hits[i].transform.GetComponent<Piece>();
                _dragOffset = _hits[i].point - _dragPiece.transform.position;
            }

            var dragPieceTransform = _dragPiece.transform;
            dragPieceTransform.position = dragPieceTransform.position.WithZ(_dragPiece.StartPosition.z);
            OnPiecePicked(_dragPiece);
        }
    }
}