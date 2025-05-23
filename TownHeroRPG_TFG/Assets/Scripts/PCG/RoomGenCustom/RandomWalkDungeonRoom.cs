using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkDungeonRoom : Room
{
    public RandomWalkDungeonParameters RandomWalkDungeonParameters;

    public RandomWalkDungeonRoom(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, RandomWalkDungeonParameters randomWalkDungeonParameters) : base(roomCenterPos, floorTiles)
    {
        RandomWalkDungeonParameters = randomWalkDungeonParameters;
    }
}
