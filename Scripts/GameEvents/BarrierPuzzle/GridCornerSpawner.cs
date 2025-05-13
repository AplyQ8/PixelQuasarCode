using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridCornerSpawner : MonoBehaviour
{
    public GameObject EdgePref; // Prefab for corners
    public GameObject WallHorizontalPref; // Prefab for horizontal edges
    public GameObject WallVerticalPref; // Prefab for vertical edges

    public Vector2Int bottomLeftCell; // Bottom-left cell coordinates
    public Vector2Int topRightCell; // Top-right cell coordinates

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void SpawnObjects()
    {
        ClearObjects();

        Grid grid = GetComponent<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid component found on the GameObject.");
            return;
        }

        Vector3 cellSize = grid.cellSize;
        Vector3 cellOffset = new Vector3(cellSize.x / 2, cellSize.y / 2, 0);

        HashSet<Vector3> spawnedPositions = new HashSet<Vector3>();

        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
            {
                Vector3 cellCenter = grid.CellToWorld(new Vector3Int(x, y, 0)) + cellOffset;

                // Spawn EdgePref at the corners
                Vector3[] corners = new Vector3[]
                {
                    cellCenter + new Vector3(-cellSize.x / 2, -cellSize.y / 2, 0), // Bottom-left
                    cellCenter + new Vector3(cellSize.x / 2, -cellSize.y / 2, 0),  // Bottom-right
                    cellCenter + new Vector3(-cellSize.x / 2, cellSize.y / 2, 0),  // Top-left
                    cellCenter + new Vector3(cellSize.x / 2, cellSize.y / 2, 0)    // Top-right
                };

                foreach (var corner in corners)
                {
                    if (!spawnedPositions.Contains(corner))
                    {
                        spawnedPositions.Add(corner);
                        GameObject cornerObj = (GameObject)PrefabUtility.InstantiatePrefab(EdgePref, transform);
                        cornerObj.transform.position = corner;
                        spawnedObjects.Add(cornerObj);
                    }
                }

                // Spawn WallHorizontalPref at the horizontal edges
                Vector3[] horizontalEdges = new Vector3[]
                {
                    cellCenter + new Vector3(0, -cellSize.y / 2, 0), // Bottom edge
                    cellCenter + new Vector3(0, cellSize.y / 2, 0)   // Top edge
                };

                foreach (var edge in horizontalEdges)
                {
                    if (!spawnedPositions.Contains(edge))
                    {
                        spawnedPositions.Add(edge);
                        GameObject horizontalObj = (GameObject)PrefabUtility.InstantiatePrefab(WallHorizontalPref, transform);
                        horizontalObj.transform.position = edge;
                        spawnedObjects.Add(horizontalObj);
                    }
                }

                // Spawn WallVerticalPref at the vertical edges
                Vector3[] verticalEdges = new Vector3[]
                {
                    cellCenter + new Vector3(-cellSize.x / 2, 0, 0), // Left edge
                    cellCenter + new Vector3(cellSize.x / 2, 0, 0)   // Right edge
                };

                foreach (var edge in verticalEdges)
                {
                    if (!spawnedPositions.Contains(edge))
                    {
                        spawnedPositions.Add(edge);
                        GameObject verticalObj = (GameObject)PrefabUtility.InstantiatePrefab(WallVerticalPref, transform);
                        verticalObj.transform.position = edge;
                        spawnedObjects.Add(verticalObj);
                    }
                }
            }
        }
    }

    public void ClearObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }

        spawnedObjects.Clear();
    }
}
