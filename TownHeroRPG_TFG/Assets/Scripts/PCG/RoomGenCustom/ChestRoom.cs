using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRoom : Room
{
    public ChestRoomParameters ChestRoomParameters;

    public ChestRoom(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, ChestRoomParameters parameters) : base(roomCenterPos, floorTiles)
    {
        ChestRoomParameters = parameters;
    }
}

