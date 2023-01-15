using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class CameraController
{
    private readonly Camera _cam;

    public CameraController(Camera camera)
    {
        _cam = camera;
    }

    public void SetCameraPosition(Vector2Int gridSize)
    {
        _cam.orthographicSize = gridSize.x;
        var camTransform = _cam.transform;
        camTransform.position = camTransform.position.WithY(-1 * (gridSize.y - 4));
    }
}