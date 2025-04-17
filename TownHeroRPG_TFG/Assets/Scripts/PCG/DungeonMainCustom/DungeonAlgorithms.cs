using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DungeonAlgorithms
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int start, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(start);
        
        Vector2Int pos = start;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int direction = Directions.GetCardinalDirectionRandomly();
            pos += direction;
            path.Add(pos);
        }
        
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridors(Vector2Int start, int corridorWidth)
    {
        List<Vector2Int> corridorPath = new List<Vector2Int>();
        Vector2Int direction = Directions.GetCardinalDirectionRandomly();
        Vector2Int pos = start;

        corridorPath.Add(pos);

        for (int i = 0; i < corridorWidth; i++)
        {
            pos += direction;
            corridorPath.Add(pos);
        }

        return corridorPath;
    }
}








