using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room
{
    public BossRoomParameters BossRoomParameters;

    public BossRoom(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, BossRoomParameters bossRoomParameters) : base(roomCenterPos, floorTiles)
    {
        BossRoomParameters = bossRoomParameters;
    }
}
