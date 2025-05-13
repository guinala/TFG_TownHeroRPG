using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : Room
{
    public EnemyRoomParameters EnemyRoomParameters;

    public EnemyRoom(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, EnemyRoomParameters parameters) : base(roomCenterPos, floorTiles)
    {
        EnemyRoomParameters = parameters;
    }
}

