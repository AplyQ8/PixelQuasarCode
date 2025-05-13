using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SurfaceRecognizer : MonoBehaviour
{
    
    [SerializeField] private AllTilesData allTilesData;
    private List<Tilemap> groundLevels;
    private Dictionary<TileBase, SurfaceType> tilesSurfaces;
    
    
    // Start is called before the first frame update
    void Start()
    {
        groundLevels = new List<Tilemap>();
        foreach (Transform lvl in GameObject.Find("Ground").transform)
        {
            groundLevels.Add(lvl.GetComponent<Tilemap>());
        }


        tilesSurfaces = new Dictionary<TileBase, SurfaceType>();
        foreach (var tileData in allTilesData.tilesData)
        {
            foreach (var tile in tileData.tiles)
            {
                tilesSurfaces[tile] = tileData.surfaceType;
            }
        }
        
    }

    private TileBase GetTileFromLevel(int level)
    {
        var gridPos = groundLevels[level].WorldToCell(transform.position);
        TileBase tile = groundLevels[level].GetTile(gridPos);
        return tile;
    }

    private SurfaceType GetTileSurface(TileBase tile)
    {
        if (tilesSurfaces.ContainsKey(tile))
            return tilesSurfaces[tile];

        return SurfaceType.Default;
    }

    public SurfaceType GetCurrentTileSurface()
    {
        for (int i = groundLevels.Count - 1; i >= 0; i--)
        {
            var tile = GetTileFromLevel(i);
            if (tile)
            {
                var surfaceType = GetTileSurface(tile);
                if(surfaceType != SurfaceType.Default)
                    return surfaceType;
            }
        }

        return SurfaceType.Default;
    }
}
