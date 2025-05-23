using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BinarySpacePartitioningGenerator : RandomWalkGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    [SerializeField] private bool circularRooms = false;

    private Dictionary<Vector2Int, HashSet<Vector2Int>> rooms;
    private HashSet<Vector2Int> path;
    private HashSet<Vector2Int> corridorPath;
    public DungeonData dungeonData;
    public UnityEvent<DungeonData> OnDungeonGenerated;

    private void Start()
    {
        RunAlgorithm();
    }

    protected override void RunAlgorithm()
    {
        rooms = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        path = new HashSet<Vector2Int>();
        corridorPath = new HashSet<Vector2Int>();
        RunBinarySpaceAlgorithm();
    }

    private void RunBinarySpaceAlgorithm()
    {
        if(minRoomWidth <= 0 || minRoomHeight <= 0)
        {
            Debug.LogError("Insufficient Min Room parameters");
            return;
        }

        if(dungeonHeight <= 0 || dungeonHeight <= 0)
        {
            Debug.LogError("Insufficient Dungeon parameters");
            return;
        }

        BoundsInt dungeonArea = new BoundsInt((Vector3Int)startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0));
        List<BoundsInt> roomsList = DungeonAlgorithms.BinarySpacePartitioning(dungeonArea, minRoomWidth, minRoomHeight);

        path = randomWalkRooms ? GenerateRandomWalkRooms(roomsList) : (circularRooms ? GenerateCircularRooms(roomsList) : GenerateSimpleRooms(roomsList));

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        { 
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenters.Add(center);
        }

        corridorPath = ConnectRooms(roomCenters);
        path.UnionWith(corridorPath);
        PaintDungeon(path);

        dungeonData.InitializeRoomDictionary(rooms, corridorPath, path);
        OnDungeonGenerated?.Invoke(dungeonData);
    }

    private void SaveData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        rooms[roomPosition] = roomFloor;
    }

    private HashSet<Vector2Int> GenerateRandomWalkRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            Vector2Int roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);

            HashSet<Vector2Int> roomFloor = RunRandomWalkAlgorithm(roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (room.xMin + offset) &&
                    position.x <= (room.xMax - offset) &&
                    position.y >= (room.yMin - offset) &&
                    position.y <= (room.yMax - offset))
                {
                    path.Add(position);
                }
            }
            SaveData(roomCenter, roomFloor);
        }
        return path;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        if (roomCenters == null || roomCenters.Count == 0)
        {
            return corridors;
        }

        Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestRoomPoint(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = GenerateCorridors(currentRoomCenter, closest);
            corridors.UnionWith(newCorridor);
            currentRoomCenter = closest;
        }
        return corridors;
    }

    private HashSet<Vector2Int> GenerateCorridors(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            position = new Vector2Int(position.x, position.y + (destination.y > position.y ? 1 : -1));
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            position = new Vector2Int(position.x + (destination.x > position.x ? 1 : -1), position.y);
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestRoomPoint(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distanceMinima = float.MaxValue;

        foreach (Vector2Int candidate in roomCenters)
        {
            float distancia = Vector2.Distance(candidate, currentRoomCenter);
            if (distancia < distanceMinima)
            {
                distanceMinima = distancia;
                closest = candidate;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> GenerateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(pos);
                    roomFloor.Add(pos);
                }
            }
            Vector2Int roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            SaveData(roomCenter, roomFloor);
        }
        return floor;
    }

    private HashSet<Vector2Int> GenerateCircularRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(room.center);
            int radius = Mathf.Min(room.size.x, room.size.y) / 2 - offset;
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        Vector2Int pos = center + new Vector2Int(x, y);
                        if (pos.x >= room.xMin + offset && pos.x <= room.xMax - offset &&
                            pos.y >= room.yMin + offset && pos.y <= room.yMax - offset)
                        {
                            floor.Add(pos);
                            roomFloor.Add(pos);
                        }
                    }
                }
            }
            SaveData(center, roomFloor);
        }
        return floor;
    }

    private void PaintDungeon(HashSet<Vector2Int> path)
    {
        painter.ClearTiles();
        painter.PaintGround(path);
        WallPlacer.GenerateWalls(path, painter);
    }
}
