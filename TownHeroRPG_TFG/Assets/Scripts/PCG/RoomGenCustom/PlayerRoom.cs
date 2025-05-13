using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerRoom : Room
{
    public Vector2Int StartPosition { get; set; }
    public PlayerRoomParameters PlayerRoomParameters { get; set; }

    public PlayerRoom(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, PlayerRoomParameters playerRoomParameters) : base(roomCenterPos, floorTiles)
    {
        StartPosition = Vector2Int.RoundToInt(roomCenterPos);
        PlayerRoomParameters = playerRoomParameters;
    }
}

