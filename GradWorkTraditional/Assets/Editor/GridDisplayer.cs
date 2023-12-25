using System;
using Gradwork.Attacks;
using UnityEditor;
using UnityEngine;
using Grid = Vital.Spatial_Partitioning.Grid;


[CustomEditor(typeof(MonoAttackSpawner))]
public class GridDisplayer : Editor
{
    private void OnSceneGUI()
    {
        if (Grid.Instance == null)
            new Grid();
        var grid = Grid.Instance;
        DrawGridOutline(grid);
        
    }

    private void DrawGridOutline(Grid grid)
    {
        Handles.color = Color.green;
        grid.GetDrawingInfromation(out var numcells, out var cellSizee, out var offset);

        for (int i = 0; i < numcells; i++)
        {
            for (int j = 0; j < numcells; j++)
            {
                var x = i * cellSizee + offset.X;
                var z = j * cellSizee + offset.Z;
                var squareStartPoint = new Vector3(x, 0, z);

                DrawSquare(squareStartPoint, cellSizee);
            }
        }
        
        var startPoint = new Vector3(offset.X, 0, offset.Z);
        var length = numcells * cellSizee;
        
        DrawSquare(startPoint, length);
    }
    
    private void DrawSquare(Vector3 startPoint, float length)
    {
        Handles.DrawLine(startPoint, startPoint + new Vector3(length, 0, 0));
        Handles.DrawLine(startPoint, startPoint + new Vector3(0, 0, length));
        Handles.DrawLine(startPoint + new Vector3(length, 0, 0), startPoint + new Vector3(length, 0, length));
        Handles.DrawLine(startPoint + new Vector3(0, 0, length), startPoint + new Vector3(length, 0, length));
    }
}
