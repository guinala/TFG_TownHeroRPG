using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonContentGenerator : MonoBehaviour
{
    [Header("Dungeon Data")]
    private DungeonData dungeonData;
    private RandomWalkRoomData randomWalkRoomData;
    public UnityEvent<DungeonData> OnFinishedRoomProcessing;

    [Header("Room Parameters")]
    [SerializeField] private BossRoomParameters bossRoomParameters;
    [SerializeField] private PlayerRoomParameters playerRoomParameters;
    [SerializeField] private ChestRoomParameters chestRoomParameters;
    [SerializeField] private EnemyRoomParameters enemyRoomParameters;
    [SerializeField] private RandomWalkDungeonParameters randomWalkDungeonParameters;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmo = false;
    [SerializeField] private GameObject square;

    public void ProcessRooms(DungeonData data)
    {
        if (data == null)
            return;

        dungeonData = data;

        Debug.Log("Se supone que tengo" + dungeonData.rooms.Count + " habitaciones en el dungeon data. Se supone que tengo que procesar las habitaciones de la data del dungeon. Las habitaciones son: ");
        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.rooms.Count);
        Debug.Log(randomRoomIndex);
        Vector2Int startPos = dungeonData.rooms.Keys.ElementAt(randomRoomIndex);
        HashSet<Vector2Int> playerFloor = dungeonData.rooms.Values.ElementAt(randomRoomIndex);

        dungeonData.PlayerRoom = new PlayerRoom(startPos, playerFloor, playerRoomParameters);
        dungeonData.Rooms.Add(dungeonData.PlayerRoom);
        Dijkstra dijkstra = new Dijkstra(dungeonData.floor);
        Dictionary<Vector2Int, int> distances = dijkstra.DijkstraAlgorithm(dungeonData.PlayerRoom.StartPosition);

        CreateRoomsFromDictionary(distances);
        
        foreach (Room room in dungeonData.Rooms)
        {
            ClassifyRoomTiles(room);
        }
        OnFinishedRoomProcessing?.Invoke(dungeonData);
    }

    private void ClassifyRoomTiles(Room room)
    {
        square = Instantiate(square, new Vector3(room.RoomCenterPos.x, room.RoomCenterPos.y, 0), Quaternion.identity);

        room.NearWallUpTiles.Clear();
        room.NearWallDownTiles.Clear();
        room.NearWallLeftTiles.Clear();
        room.NearWallRightTiles.Clear();
        room.CornerTiles.Clear();
        room.InnerTiles.Clear();
        room.DeadEndTiles.Clear();

        foreach (Vector2Int tilePosition in room.FloorTiles)
        {
            HashSet<Vector2Int> wallNeighbours = new HashSet<Vector2Int>();

            foreach (Vector2Int dir in Directions.cardinalDirections)
            {
                if (!room.FloorTiles.Contains(tilePosition + dir))
                    wallNeighbours.Add(dir);
            }

            if (wallNeighbours.Contains(Vector2Int.up)) room.NearWallUpTiles.Add(tilePosition);
            if (wallNeighbours.Contains(Vector2Int.down)) room.NearWallDownTiles.Add(tilePosition);
            if (wallNeighbours.Contains(Vector2Int.left)) room.NearWallLeftTiles.Add(tilePosition);
            if (wallNeighbours.Contains(Vector2Int.right)) room.NearWallRightTiles.Add(tilePosition);

            if (wallNeighbours.Count == 3)
                room.DeadEndTiles.Add(tilePosition);

            else if (wallNeighbours.Count == 2)
            {
                Vector2Int dir1 = wallNeighbours.ElementAt(0);
                Vector2Int dir2 = wallNeighbours.ElementAt(1);


                if ((wallNeighbours.Contains(Vector2Int.left) && wallNeighbours.Contains(Vector2Int.right)) || (wallNeighbours.Contains(Vector2Int.up) && wallNeighbours.Contains(Vector2Int.down)))
                {
                    room.CorridorTiles.Add(tilePosition);
                }

                else if (dir1.x != dir2.x && dir1.y != dir2.y)
                {
                    Vector2Int diagonalDir = dir1 + dir2;
                    if (!room.FloorTiles.Contains(tilePosition + diagonalDir))
                    {
                        room.CornerTiles.Add(tilePosition);
                    }
                }
            }

            if (CheckCardinalNeighbors(tilePosition, room.FloorTiles))
            {
                room.InnerTiles.Add(tilePosition);
            }

        }

        room.CornerTiles.ExceptWith(room.InnerTiles);
        room.CornerTiles.ExceptWith(room.DeadEndTiles);
        room.CornerTiles.ExceptWith(room.CorridorTiles);

        room.NearWallUpTiles.ExceptWith(room.CornerTiles);
        room.NearWallDownTiles.ExceptWith(room.CornerTiles);
        room.NearWallLeftTiles.ExceptWith(room.CornerTiles);
        room.NearWallRightTiles.ExceptWith(room.CornerTiles);

        room.NearWallUpTiles.ExceptWith(room.DeadEndTiles);
        room.NearWallDownTiles.ExceptWith(room.DeadEndTiles);
        room.NearWallLeftTiles.ExceptWith(room.DeadEndTiles);
        room.NearWallRightTiles.ExceptWith(room.DeadEndTiles);

        room.NearWallUpTiles.ExceptWith(room.CorridorTiles);
        room.NearWallDownTiles.ExceptWith(room.CorridorTiles);
        room.NearWallLeftTiles.ExceptWith(room.CorridorTiles);
        room.NearWallRightTiles.ExceptWith(room.CorridorTiles);
    }

    private bool CheckCardinalNeighbors(Vector2Int position, HashSet<Vector2Int> floorTiles)
    {
        return floorTiles.Contains(position + Vector2Int.up) &&
               floorTiles.Contains(position + Vector2Int.down) &&
               floorTiles.Contains(position + Vector2Int.left) &&
               floorTiles.Contains(position + Vector2Int.right);
    }

    private void CreateRoomsFromDictionary(Dictionary<Vector2Int, int> distances)
    {
        List<(Vector2Int center, int distance)> roomDistances = new List<(Vector2Int, int)>();

        foreach (var roomEntry in dungeonData.rooms)
        {
            Vector2Int roomCenter = roomEntry.Key;
            HashSet<Vector2Int> floorTiles = roomEntry.Value;

            int minDistance = distances.ContainsKey(roomCenter) ? distances[roomCenter] : int.MaxValue;
            roomDistances.Add((roomCenter, minDistance));
        }

        var sortedRooms = roomDistances.OrderByDescending(r => r.distance).ToList();

        for (int i = 0; i < sortedRooms.Count; i++)
        {
            Vector2Int center = sortedRooms[i].center;

            if (center == dungeonData.PlayerRoom.StartPosition)
            {
                continue;
            }
                
            HashSet<Vector2Int> floorTiles = dungeonData.rooms[center];

            Room room;

            if (i == 0)
            {
               room = new BossRoom(center, floorTiles, bossRoomParameters);
            }
               
            else if (i == 1)
            {
               room = new ChestRoom(center, floorTiles, chestRoomParameters);
            }
                
            else
            {
               room = new EnemyRoom(center, floorTiles, enemyRoomParameters);
            }

            dungeonData.Rooms.Add(room);
        }
    }

    public void ProcessSingleRoom(RandomWalkRoomData data)
    {
        if(data == null)
            return;
        this.randomWalkRoomData = data;

        RandomWalkDungeonRoom randomWalkDungeonRoom = new RandomWalkDungeonRoom(dungeonData.Rooms[0].RoomCenterPos, dungeonData.Rooms[0].FloorTiles, randomWalkDungeonParameters);
        dungeonData.Rooms.Add(randomWalkDungeonRoom);
        ClassifyRoomTiles(randomWalkDungeonRoom);
        OnFinishedRoomProcessing?.Invoke(dungeonData);
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            //Draw inner tiles
            Gizmos.color = Color.yellow;
            foreach (Vector2Int floorPosition in room.InnerTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles UP
            Gizmos.color = Color.blue;
            foreach (Vector2Int floorPosition in room.NearWallUpTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles DOWN
            Gizmos.color = Color.green;
            foreach (Vector2Int floorPosition in room.NearWallDownTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles RIGHT
            Gizmos.color = Color.white;
            foreach (Vector2Int floorPosition in room.NearWallRightTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles LEFT
            Gizmos.color = Color.cyan;
            foreach (Vector2Int floorPosition in room.NearWallLeftTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles CORNERS
            Gizmos.color = Color.magenta;
            foreach (Vector2Int floorPosition in room.CornerTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }

            Gizmos.color = Color.black;
            foreach (Vector2Int floorPosition in room.DeadEndTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }

            Gizmos.color = Color.red;
            foreach (Vector2Int floorPosition in room.CorridorTiles)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }

        }
    }
}
