#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Extensions;
using GridSystem;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelCreator))]
public class LevelEditor : OdinEditor
{
    private class Shape
    {
        public List<Tri> Tris;
        public Color Color;

        public Shape(Color color)
        {
            Tris = new List<Tri>();
            Color = color;
        }
    }

    private LevelCreator _levelCreator;
    private List<Shape> _shapes;
    private bool _needsRepaint;
    private Tri _selectedTri;
    private List<Color> _colors;
    private int _selectedShapeIndex;

    private Shape SelectedShape =>
        _shapes == null || _shapes.Count == 0 ? null : _shapes[_selectedShapeIndex];

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUI.enabled = _shapes.Count >= 5;
        if (GUILayout.Button("Save", GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10),
                GUILayout.Height(50)))
        {
            Save();
        }

        GUI.enabled = true;

        if (GUILayout.Button("Load", GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10), GUILayout.Height(50)))
        {
            Load();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
        if (GUILayout.Button("Create Shape"))
        {
            CreateShape();
        }

        var shapeDeleteIndex = -1;
        for (var i = 0; i < _shapes.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Shape " + (i + 1));
            GUI.enabled = i != _selectedShapeIndex;
            if (GUILayout.Button("Select"))
            {
                SelectShape(i);
            }

            GUI.enabled = true;
            if (GUILayout.Button("Delete"))
            {
                shapeDeleteIndex = i;
            }

            GUILayout.EndHorizontal();
        }

        if (shapeDeleteIndex > -1)
            DeleteShape(shapeDeleteIndex);
    }

    private void OnSceneGUI()
    {
        var guiEvent = Event.current;


        if (guiEvent.type == EventType.Repaint)
        {
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
        }


        if (_needsRepaint)
            HandleUtility.Repaint();
    }

    private void Load()
    {
        CreateColors();

        while (_levelCreator.transform.childCount > 0)
        {
            DestroyImmediate(_levelCreator.transform.GetChild(0).gameObject);
        }

        ;
        _levelCreator.Load();
        _shapes = new List<Shape>();
        for (var i = 0; i < _levelCreator.pieces.Length; i++)
        {
            var piece = _levelCreator.pieces[i];
            CreateShape();
            _selectedShapeIndex = i;
            for (var j = 0; j < piece.triangles.Length; j += 3)
            {
                var origin = piece.vertices[piece.triangles[j]]
                             + piece.vertices[piece.triangles[j + 1]]
                             + piece.vertices[piece.triangles[j + 2]];
                origin /= 3;

                var tri = _levelCreator.GridController.GetClosestCell(origin).GetClosestTri(origin);
                TryCreateMesh(tri);
            }
        }
    }

    private void Save()
    {
        var pieces = new PieceData[_shapes.Count];
        for (int i = 0; i < _shapes.Count; i++)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var shape = _shapes[i];
            for (int j = 0; j < shape.Tris.Count; j++)
            {
                var tri = shape.Tris[j];
                for (int k = 0; k < tri.Points.Length; k++)
                {
                    var point = tri.Points[k];
                    var containsPoint = false;
                    for (int l = 0; l < vertices.Count; l++)
                    {
                        var vertex = vertices[l];
                        if (vertex != point)
                            continue;
                        containsPoint = true;
                        triangles.Add(l);
                    }

                    if (!containsPoint)
                    {
                        triangles.Add(vertices.Count);
                        vertices.Add(point);
                    }
                }
            }

            var origin = Vector3.zero;
            foreach (var vertex in vertices)
            {
                origin += vertex;
            }

            origin /= vertices.Count;

            pieces[i] = new PieceData()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                origin = origin,
                color = shape.Color
            };
        }

        _levelCreator.pieces = pieces;
        Debug.Log("Saving in progress...");
        _levelCreator.Save();
        AssetDatabase.Refresh();
        Debug.Log("SAVE COMPLETED!");
    }

    private void CreateShape()
    {
        _shapes.Add(new Shape(_colors[^1]));
        _colors.RemoveAt(_colors.Count - 1);
        var go = new GameObject("Shape " + (_levelCreator.transform.childCount + 1));
        go.transform.SetParent(_levelCreator.transform);
    }

    private void SelectShape(int index)
    {
        _selectedShapeIndex = index;
    }

    private void DeleteShape(int index)
    {
        Undo.RecordObject(_levelCreator, "Delete Shape");
        DestroyImmediate(_levelCreator.transform.GetChild(index).gameObject);
        _colors.Add(_shapes[index].Color);
        _shapes.RemoveAt(index);
        _selectedShapeIndex = Mathf.Clamp(_selectedShapeIndex, 0, _shapes.Count - 1);
    }

    private void HandleInput(Event guiEvent)
    {
        var mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        var drawPlaneHeight = 0f;
        var distanceToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.z;
        var mousePosition = mouseRay.GetPoint(distanceToDrawPlane).WithZ(0);
        var tri = _levelCreator.GridController.GetClosestCell(mousePosition).GetClosestTri(mousePosition);

        if (Vector3.Distance(mousePosition, tri.Origin) < .5f)
        {
            _selectedTri = tri;
            _needsRepaint = true;
        }

        if (SelectedShape == null)
            return;
        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            Undo.RecordObject(_levelCreator, "Add tri");
            if (!SelectedShape.Tris.Contains(tri))
            {
                TryCreateMesh(tri);
            }
        }
    }

    private void TryCreateMesh(Tri tri)
    {
        for (var i = 0; i < _shapes.Count; i++)
        {
            var shape = _shapes[i];
            for (var j = 0; j < shape.Tris.Count; j++)
            {
                var shapeTri = shape.Tris[j];
                if (tri != shapeTri)
                    continue;
                if (SelectedShape == shape)
                    return;
                _levelCreator.DestroyMeshVisualizer(i, j);
                shape.Tris.RemoveAt(j);
            }
        }

        _levelCreator.CreateMeshVisualizer(tri, SelectedShape.Color, _selectedShapeIndex);
        SelectedShape.Tris.Add(tri);
    }

    private void Draw()
    {
        if (_selectedTri != null)
            Handles.DrawPolyLine(_selectedTri.Points.Concat(new[] {_selectedTri.Points[0]}).ToArray());
    }

    private void OnEnable()
    {
        _shapes = new List<Shape>();
        _levelCreator = target as LevelCreator;
        while (_levelCreator.transform.childCount > 0)
        {
            DestroyImmediate(_levelCreator.transform.GetChild(0).gameObject);
        }

        CreateColors();
    }

    private void CreateColors()
    {
        ColorUtility.TryParseHtmlString("#FF8B13", out var orange);
        ColorUtility.TryParseHtmlString("#EFA3C8", out var pink);
        ColorUtility.TryParseHtmlString("#82AAE3", out var blue);
        ColorUtility.TryParseHtmlString("#3C6255", out var green);

        _colors = new[]
        {
            Color.blue, Color.red, Color.cyan, Color.green, Color.yellow, Color.magenta, Color.white, Color.gray,
            orange, pink, blue, green
        }.ToList();
    }
}
#endif