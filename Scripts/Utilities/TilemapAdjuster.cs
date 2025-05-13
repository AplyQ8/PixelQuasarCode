using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapAdjuster : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        // Корректируем позицию каждого тайла
        AdjustTilePivot();
    }

    void AdjustTilePivot()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                    Vector3 tileWorldPosition = tilemap.CellToWorld(tilePosition);

                    // Корректируем позицию спрайта
                    Vector3 offset = new Vector3(0, tilemap.cellSize.y / 2, 0);
                    tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one));
                }
            }
        }
    }
}
