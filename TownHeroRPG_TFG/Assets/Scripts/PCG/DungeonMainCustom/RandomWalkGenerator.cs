using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGenerator : Dungeon
{
    [Header("Parameters")]
    public RandomWalkParameters parameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> path = RunRandomWalk();
        painter.ClearTiles();
        painter.PaintGround(path);
    }

    private HashSet<Vector2Int> RunRandomWalk()
    {
        int iterations = parameters.iterations;
        int walkLength = parameters.walkLength;
        bool randomIterations = parameters.randomIterations;

        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
        Vector2Int currentPos = startPos;

        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> path = DungeonAlgorithms.RandomWalk(currentPos, walkLength);
            tiles.UnionWith(path);

            if(randomIterations)
            {
                currentPos = tiles.ElementAt(Random.Range(0, tiles.Count));
            }
        }

        return tiles;
    }

}
