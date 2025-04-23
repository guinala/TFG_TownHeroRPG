using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPainter : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap ground, wall;

    [Header("Tile Ground")]
    public TileBase groundBase;

    [Header("Tiles Walls")]
    [SerializeField] private TileBase wallTopTile, wallBottomTile, wallLeftTile, wallRightTile, wallFullTile;

    [Header("Tiles Corner Walls")]
    [SerializeField] private TileBase innerCornerDownLeftTile, innerCornerDownRightTile, diagonalCornerDownLeftTile,
        diagonalCornerDownRightTile, diagonalCornerUpLeftTile, diagonalCornerUpRightTile;

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
        wall.ClearAllTiles();
        ground.ClearAllTiles();
    }

    public bool PlaceBasicWall(Vector2Int position, string wallType)
    {
        TileBase tile = wallType switch
        {
            "Top" => wallTopTile,
            "Bottom" => wallBottomTile,
            "Left" => wallLeftTile,
            "Right" => wallRightTile,
            "Full" => wallFullTile,
            _ => null
        };

        if (tile == null)
        {
            Debug.LogWarning($"No tile defined for basic wall type: {wallType} at {position}");
            return false;
        }

        PaintTile(wall, tile, position);
        return true;
    }

    public bool PlaceCornerWall(Vector2Int position, string wallType)
    {
        TileBase tile = wallType switch
        {
            "InnerCornerDownLeft" => innerCornerDownLeftTile,
            "InnerCornerDownRight" => innerCornerDownRightTile,
            "DiagonalCornerDownLeft" => diagonalCornerDownLeftTile,
            "DiagonalCornerDownRight" => diagonalCornerDownRightTile,
            "DiagonalCornerUpLeft" => diagonalCornerUpLeftTile,
            "DiagonalCornerUpRight" => diagonalCornerUpRightTile,
            "FullEightDirections" => wallFullTile,
            "BottomEightDirections" => wallBottomTile,
            _ => null
        };

        if (tile == null)
        {
            Debug.LogWarning($"No tile defined for corner wall type: {wallType} at {position}");
            return false;
        }

        PaintTile(wall, tile, position);
        return true;
    }
}
