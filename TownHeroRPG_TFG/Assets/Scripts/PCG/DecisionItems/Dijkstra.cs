using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra
{
    List<Vector2Int> graph;

    public Dijkstra(IEnumerable<Vector2Int> vertices)
    {
        graph = new List<Vector2Int>(vertices);

    }

    private List<Vector2Int> GetNeighbours(Vector2Int startPosition, 
        List<Vector2Int> neighboursOffsetList)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var neighbourDirection in neighboursOffsetList)
        {
            Vector2Int potentialNeoghbour = startPosition + neighbourDirection;
            if (graph.Contains(potentialNeoghbour))
                neighbours.Add(potentialNeoghbour);
        }
        return neighbours;
    }

    public Dictionary<Vector2Int, int> DijkstraAlgorithm(Vector2Int startposition)
    {
        Queue<Vector2Int> unfinishedVertices = new Queue<Vector2Int>();

        Dictionary<Vector2Int, int> distanceDictionary = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> parentDictionary = new Dictionary<Vector2Int, Vector2Int>();

        distanceDictionary[startposition] = 0;
        parentDictionary[startposition] = startposition;

        foreach (Vector2Int vertex in GetNeighbours(startposition, Directions.cardinalDirections))
        {
            unfinishedVertices.Enqueue(vertex);
            parentDictionary[vertex] = startposition;
        }

        while (unfinishedVertices.Count > 0)
        {
            Vector2Int vertex = unfinishedVertices.Dequeue();
            int newDistance = distanceDictionary[parentDictionary[vertex]] + 1;
            if (distanceDictionary.ContainsKey(vertex) && distanceDictionary[vertex] <= newDistance)
                continue;
            distanceDictionary[vertex] = newDistance;

            foreach (Vector2Int neighbour in GetNeighbours(vertex, Directions.cardinalDirections))
            {
                if (distanceDictionary.ContainsKey(neighbour))
                    continue;
                unfinishedVertices.Enqueue(neighbour);
                parentDictionary[neighbour] = vertex;
            }
        }

        return distanceDictionary;
    }
}
