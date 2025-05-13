using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGraph
{
    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in Directions.allDirections)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }

    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int> { startPos };

        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int> { { startPos, startPos } };

        while (nodesToVisit.Count > 0)
        {
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            foreach (Vector2Int neighbourPosition in neighbours)
            {
                if (!visitedNodes.Contains(neighbourPosition) && !occupiedNodes.Contains(neighbourPosition))
                {
                    nodesToVisit.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }
}
