using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions
{
    private static List<Vector2Int> cardinalDirections = new List<Vector2Int>
    {
        new Vector2Int(-1, 0),  // WEST
        new Vector2Int(0, 1),    // NORTH
        new Vector2Int(1, 0),    // EAST
        new Vector2Int(0, -1)    // SOUTH
    };

    // private static List<Vector2Int> diagonalDirections = new List<Vector2Int>
    // {
    //     new Vector2Int(1, 1),    // NORTHEAST
    //     new Vector2Int(1, -1),   // SOUTHEAST
    //     new Vector2Int(-1, -1),  // SOUTHWEST
    //     new Vector2Int(-1, 1)    // NORTHWEST
    // };
    //
    // private static List<Vector2Int> allDirections = new List<Vector2Int>
    // {
    //     new Vector2Int(0, 1),    // NORTH
    //     new Vector2Int(1, 1),     // NORTHEAST
    //     new Vector2Int(1, 0),     // EAST
    //     new Vector2Int(1, -1),    // SOUTHEAST
    //     new Vector2Int(0, -1),    // SOUTH
    //     new Vector2Int(-1, -1),   // SOUTHWEST
    //     new Vector2Int(-1, 0),   // WEST
    //     new Vector2Int(-1, 1)     // NORTHWEST
    // };

    public static Vector2Int GetCardinalDirectionRandomly()
    {
        return cardinalDirections[Random.Range(0, cardinalDirections.Count)];
    }
}