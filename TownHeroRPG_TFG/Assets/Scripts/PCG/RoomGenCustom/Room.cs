using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2Int RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> DeadEndTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CorridorTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallUpTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallDownTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallLeftTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallRightTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropObjects { get; set; } = new List<GameObject>();

    public List<Vector2Int> AvailablePositionsFromPath { get; set; } = new List<Vector2Int>();

    public List<GameObject> EnemiesInRoom { get; set; } = new List<GameObject>();
    public List<GameObject> SpecialItemsInRoom { get; set; } = new List<GameObject>();

    public Room(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}