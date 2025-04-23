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

    protected override void RunAlgorithm()
    {
        HashSet<Vector2Int> path = RunRandomWalkAlgorithm(startPos);
        if(path != null)
        {
            PaintDungeon(path);
        }
    }

    protected HashSet<Vector2Int> RunRandomWalkAlgorithm(Vector2Int origin)
    {
        if(parameters == null)
        {
            Debug.LogError("The parameters SO is null");
            return null;
        }
        if(parameters.iterations <= 0)
        {
            Debug.LogError("Insufficient iterations");
            return null;
        }
        if(parameters.walkLength <= 0)
        {
            Debug.LogError("Insufficient walk length");
            return null;
        }

        int iterations = parameters.iterations;
        int walkLength = parameters.walkLength;
        bool randomIterations = parameters.randomIterations;

        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
        Vector2Int currentPos = origin;

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

    private void PaintDungeon(HashSet<Vector2Int> path)
    {
        painter.ClearTiles();
        painter.PaintGround(path);
    }
}
