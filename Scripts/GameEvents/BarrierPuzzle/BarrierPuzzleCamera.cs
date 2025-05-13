using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class BarrierPuzzleCamera : MonoBehaviour
{
    public float newCameraSize = 5f; // Desired camera size when player enters the zone
    public List<GridCellConfig> gridCellConfigs; // List of grid cells and their intervals
    public Grid grid; // Reference to the grid

    private float originalCameraSize; // Store the original camera size
    private Camera mainCamera;
    private CameraFlow cameraFlow;
    private Transform playerTransform;
    
    private List<GameObject> cellCenterObjects;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        
        Collider2D collider = GetComponent<Collider2D>();
        
        GameObject cameraObject = GameObject.Find("Main Camera");
        mainCamera = cameraObject.GetComponent<Camera>();
        cameraFlow = cameraObject.GetComponent<CameraFlow>();

        // Create empty child objects at grid cell centers
        cellCenterObjects = new List<GameObject>();
        for(int i = 0; i < gridCellConfigs.Capacity; i++)
        {
            var config = gridCellConfigs[i];
            Vector3 cellCenter = GetCustomCellCenter(config.gridCoordinate);

            // Create an empty GameObject
            GameObject cellCenterObject = new GameObject($"CellCenter_{config.gridCoordinate}");
            cellCenterObject.transform.position = cellCenter;

            // Make it a child of this object for organization
            cellCenterObject.transform.parent = transform;

            // Store it in the dictionary for later access
            cellCenterObjects.Add(cellCenterObject);
        }
    }
    
    private Vector3 GetCustomCellCenter(Vector3 configGridCoordinate)
    {
        // Convert the integral part of the grid coordinate to a Vector3Int
        Vector3Int baseCellCoordinate = new Vector3Int(
            Mathf.FloorToInt(configGridCoordinate.x),
            Mathf.FloorToInt(configGridCoordinate.y),
            Mathf.FloorToInt(configGridCoordinate.z)
        );

        // Calculate the cell center for the integral part
        Vector3 baseCellCenter = grid.GetCellCenterWorld(baseCellCoordinate);

        // Adjust the center for .5 offsets
        float offsetX = (Mathf.Abs(configGridCoordinate.x % 1) < 0.01f) ? 0 : grid.cellSize.x / 2;
        float offsetY = (Mathf.Abs(configGridCoordinate.y % 1) < 0.01f) ? 0 : grid.cellSize.y / 2;
        
        // Apply the offset to the base cell center
        return new Vector3(
            baseCellCenter.x + offsetX,
            baseCellCenter.y + offsetY,
            baseCellCenter.z
        );
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && mainCamera != null)
        {
            // Save the original camera settings
            originalCameraSize = mainCamera.orthographicSize;

            // Apply new settings for this zone
            mainCamera.orthographicSize = newCameraSize;
            
            StartCoroutine(UpdateCameraRoutine());
        }

    }

    IEnumerator UpdateCameraRoutine()
    {
        for (;;)
        {
            UpdateCamera();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateCamera()
    {
        Vector3 playerPosition = playerTransform.position;

        // Check intervals and set camera position
        for(int i = 0; i < gridCellConfigs.Capacity; i++)
        {
            var config = gridCellConfigs[i];
            if (IsWithinInterval(playerPosition, config.interval))
            {
                // Update objectToFollow to the corresponding empty object
                cameraFlow.objectToFollow = cellCenterObjects[i].transform;
                
                mainCamera.orthographicSize = newCameraSize;

                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            // Restore camera to original settings
            if (mainCamera != null)
            {
                mainCamera.orthographicSize = originalCameraSize;
            }

            if (cameraFlow != null)
            {
                cameraFlow.objectToFollow = playerTransform; // Restore object following
            }
        }
    }

    private bool IsWithinInterval(Vector3 playerWorldPosition, CoordinateInterval interval)
    {
        // Convert player world position to grid coordinates
        Vector3Int playerGridPosition = grid.WorldToCell(playerWorldPosition);
        
        // Check if the player's grid position is within the interval relative to the cell
        return playerGridPosition.x >= interval.minX &&
               playerGridPosition.x <= interval.maxX &&
               playerGridPosition.y >= interval.minY &&
               playerGridPosition.y <= interval.maxY;
    }

    [System.Serializable]
    public class GridCellConfig
    {
        public Vector3 gridCoordinate; // Grid coordinate
        public CoordinateInterval interval; // Interval associated with this grid coordinate
    }

    [System.Serializable]
    public class CoordinateInterval
    {
        public float minX; // Minimum X coordinate
        public float maxX; // Maximum X coordinate
        public float minY; // Minimum Y coordinate
        public float maxY; // Maximum Y coordinate
    }
}
