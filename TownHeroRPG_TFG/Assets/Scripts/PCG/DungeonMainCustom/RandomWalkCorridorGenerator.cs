using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkCorridorGenerator : RandomWalkGenerator
{
    [Header("Parameters")]
    [SerializeField] private int corridorLength = 10, corridors = 10;
    [SerializeField] [Range(0.1f, 1)] private float roomPercentage = 0.5f;

    [Header("Customization")]
    [SerializeField] private bool branches;
    [SerializeField] private bool variableLength;
    [SerializeField] private bool increaseCorridorWidth;

    private void Start()
    {
        RunAlgorithm();
    }

    protected override void RunAlgorithm()
    {
        RunRandomWalkCorridorAlgorithm();
    }

    private void RunRandomWalkCorridorAlgorithm()
    {
        if(corridors <= 0)
        {
            Debug.LogError("Insufficient quantity of corridors");
            return;
        }

        if(corridorLength <= 0)
        {
            Debug.LogError("Insufficient length for corridors");
            return;
        }
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        HashSet<Vector2Int> possibleRoomPos = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPos;

        List<List<Vector2Int>> corridorsList = GenerateCorridors(path, possibleRoomPos);
        roomPos = GenerateRooms(possibleRoomPos);
        RoomsInDeadEnds(path, roomPos);
        path.UnionWith(roomPos);

        //Aumentar width
        if(increaseCorridorWidth)
        {
            for(int i = 0; i < corridorsList.Count; i++)
            {
                
                corridorsList[i] = IncreaseCorridorWidth(corridorsList[i]);
                path.UnionWith(corridorsList[i]); 
            }
        }

        PaintDungeon(path);
    }

    private void RoomsInDeadEnds(HashSet<Vector2Int> path, HashSet<Vector2Int> roomPos)
    {
        List<Vector2Int> deadEndList = new List<Vector2Int>(path.Count);

        foreach (var pos in path)
        {
            int neighborCount = Directions.cardinalDirections.Select(dir => pos + dir).Count(p => path.Contains(p));
            if (neighborCount == 1)
                deadEndList.Add(pos);
        }

        foreach (var pos in deadEndList)
        {
            if (!roomPos.Contains(pos))
            {
                var room = RunRandomWalkAlgorithm(pos);
                roomPos.UnionWith(room);
            }
        }
    }

    private HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> possibleRoomPositions)
    {
        int roomsToCreate = Mathf.RoundToInt(possibleRoomPositions.Count * roomPercentage);
        List<Vector2Int> shuffledCandidates = possibleRoomPositions.OrderBy(_ => Guid.NewGuid()).Take(roomsToCreate).ToList();

        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        foreach (var point in shuffledCandidates)
        {
            HashSet<Vector2Int> room = RunRandomWalkAlgorithm(point);
            roomPositions.UnionWith(room);
        }
        
        return roomPositions;
    }

    private List<List<Vector2Int>> GenerateCorridors(HashSet<Vector2Int> path, HashSet<Vector2Int> possibleRoomPos)
    {
        Vector2Int currentPos = startPos;
        possibleRoomPos.Add(currentPos);
        List<List<Vector2Int>> corridorsList = new List<List<Vector2Int>>(); 

        for (int i = 0; i < corridors; i++)
        {
            List<Vector2Int> corridor = new List<Vector2Int>();
            //variable length
            if(variableLength)
            {
                int variedLength = corridorLength + UnityEngine.Random.Range(-2, 3);
                corridor = DungeonAlgorithms.RandomWalkCorridors(currentPos, variedLength);
                corridorsList.Add(corridor);
            }
            else
            {
                corridor = DungeonAlgorithms.RandomWalkCorridors(currentPos, corridorLength);
                corridorsList.Add(corridor);
            }
           
            int branchPossibility = Random.Range(0,10);
            //Branch
            if (branches && branchPossibility < 5 && corridor.Count > 5)
            {
                Vector2Int branchStart = corridor[corridor.Count / 2];
                List<Vector2Int> branch = DungeonAlgorithms.RandomWalkCorridors(branchStart, corridorLength);
                corridorsList.Add(branch);
                path.UnionWith(branch);
            }

            currentPos = corridor.Last();
            possibleRoomPos.Add(currentPos);
            path.UnionWith(corridor);
        }

        return corridorsList;
    }

    public List<Vector2Int> IncreaseCorridorWidth(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for(int i = 1; i < corridor.Count; i++)
        {
            for(int x = -1; x < 2; x++)
            {
                for(int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    private void PaintDungeon(HashSet<Vector2Int> path)
    {
        painter.ClearTiles();
        painter.PaintGround(path);
    }
}
