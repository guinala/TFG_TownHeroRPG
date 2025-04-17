using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPainter : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap ground;

    [Header("Tile")]
    public TileBase groundBase;

    public void PaintGround(IEnumerable<Vector2Int> groundPath)
    {
        PaintTiles(groundPath, ground, groundBase);
    }

    private void PaintTiles(IEnumerable<Vector2Int> path, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in path)
        {
            PaintTile(tilemap, tile, position);
        }
    }

    private void PaintTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void ClearTiles()
    {
        ground.ClearAllTiles();
    }

}
