using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonContentGenerator : MonoBehaviour
{
    private DungeonData dungeonData;
    [SerializeField] private GameObject square;

    [SerializeField]
    private bool showGizmo = false;

    public UnityEvent<DungeonData> OnFinishedRoomProcessing;
    [SerializeField] private int playerRoomIndex = 0;

    [SerializeField] private BossRoomParameters bossRoomParameters;
    [SerializeField] private PlayerRoomParameters playerRoomParameters;
    [SerializeField] private ChestRoomParameters chestRoomParameters;
    [SerializeField] private EnemyRoomParameters enemyRoomParameters;

    public void ProcessRooms(DungeonData data)
    {
        if (data == null)
            return;

        dungeonData = data;

        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.rooms.Count);
        Vector2Int startPos = dungeonData.rooms.Keys.ElementAt(randomRoomIndex);
        HashSet<Vector2Int> playerFloor = dungeonData.rooms.Values.ElementAt(randomRoomIndex);

        dungeonData.PlayerRoom = new PlayerRoom(startPos, playerFloor, playerRoomParameters);
        dungeonData.Rooms.Add(dungeonData.PlayerRoom);
        Graph graph = new Graph(dungeonData.floor);
        Dictionary<Vector2Int, int> distances = DijkstraAlgorithm.Dijkstra(graph, dungeonData.PlayerRoom.StartPosition);

        CreateRoomsFromDictionary(distances);
        Debug.Log(dungeonData.Rooms.Count + " habitaciones creadas desde el jugador: " + dungeonData.PlayerRoom.StartPosition);
        
        foreach (Room room in dungeonData.Rooms)
        {
            room.NearWallUpTiles.Clear();
            room.NearWallDownTiles.Clear();
            room.NearWallLeftTiles.Clear();
            room.NearWallRightTiles.Clear();
            room.CornerTiles.Clear();
            room.InnerTiles.Clear();
            room.DeadEndTiles.Clear();

            foreach (Vector2Int tilePosition in room.FloorTiles)
            {
                HashSet<Vector2Int> missingDirections = new HashSet<Vector2Int>();

                // Verificar las 8 direcciones para detectar paredes cercanas
                foreach (Vector2Int dir in Directions.cardinalDirections)
                {
                    if (!room.FloorTiles.Contains(tilePosition + dir))
                        missingDirections.Add(dir);
                }

                // Clasificar tiles cercanos a paredes (cardinales espec�ficos)
                if (missingDirections.Contains(Vector2Int.up)) room.NearWallUpTiles.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.down)) room.NearWallDownTiles.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.left)) room.NearWallLeftTiles.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.right)) room.NearWallRightTiles.Add(tilePosition);

                if(missingDirections.Count == 3)
                    room.DeadEndTiles.Add(tilePosition); 

                else if (missingDirections.Count == 2)
                {
                    Vector2Int dir1 = missingDirections.ElementAt(0);
                    Vector2Int dir2 = missingDirections.ElementAt(1);


                    if ((missingDirections.Contains(Vector2Int.left) && missingDirections.Contains(Vector2Int.right)) || (missingDirections.Contains(Vector2Int.up) && missingDirections.Contains(Vector2Int.down)))
                    {
                        room.CorridorTiles.Add(tilePosition);
                    }

                    // Verifica si las direcciones son perpendiculares (ej: up + left)
                    else if (dir1.x != dir2.x && dir1.y != dir2.y)
                    {
                        // Verifica si la diagonal también es pared
                        Vector2Int diagonalDir = dir1 + dir2;
                        if (!room.FloorTiles.Contains(tilePosition + diagonalDir))
                        {
                            room.CornerTiles.Add(tilePosition); 
                        }
                    }
                }

                // Tiles interiores: Todos los vecinos cardinales presentes
                if (CheckCardinalNeighbors(tilePosition, room.FloorTiles))
                {
                    room.InnerTiles.Add(tilePosition);
                }
                    
            }

            room.CornerTiles.ExceptWith(room.InnerTiles);
            room.CornerTiles.ExceptWith(room.DeadEndTiles);
            room.CornerTiles.ExceptWith(room.CorridorTiles);

            // Excluir esquinas de los NearWallTiles
            room.NearWallUpTiles.ExceptWith(room.CornerTiles);
            room.NearWallDownTiles.ExceptWith(room.CornerTiles);
            room.NearWallLeftTiles.ExceptWith(room.CornerTiles);
            room.NearWallRightTiles.ExceptWith(room.CornerTiles);

            // Excluir DeadEndTiles de los NearWallTiles
            room.NearWallUpTiles.ExceptWith(room.DeadEndTiles);
            room.NearWallDownTiles.ExceptWith(room.DeadEndTiles);
            room.NearWallLeftTiles.ExceptWith(room.DeadEndTiles);
            room.NearWallRightTiles.ExceptWith(room.DeadEndTiles);

            room.NearWallUpTiles.ExceptWith(room.CorridorTiles);
            room.NearWallDownTiles.ExceptWith(room.CorridorTiles);
            room.NearWallLeftTiles.ExceptWith(room.CorridorTiles);
            room.NearWallRightTiles.ExceptWith(room.CorridorTiles);
        }
        //AssignPlayerRoomAndSpecialRooms();
        OnFinishedRoomProcessing?.Invoke(dungeonData);
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

            // Calcular distancia m�nima desde el jugador
            int minDistance = distances.ContainsKey(roomCenter) ? distances[roomCenter] : int.MaxValue;
            roomDistances.Add((roomCenter, minDistance));
        }

        // Ordenar por distancia (mayor = m�s lejana)
        var sortedRooms = roomDistances.OrderByDescending(r => r.distance).ToList();

        // Crear instancias de Room/BossRoom/ChestRoom
        for (int i = 0; i < sortedRooms.Count; i++)
        {
            Vector2Int center = sortedRooms[i].center;

            if (center == dungeonData.PlayerRoom.StartPosition)
            {
                Debug.Log("Me lo salto"); 
                continue;
            }
                
            HashSet<Vector2Int> floorTiles = dungeonData.rooms[center];

            Room room;
            if (i == 0)
                room = new BossRoom(center, floorTiles, bossRoomParameters);
            else if (i == 1)
                room = new ChestRoom(center, floorTiles, chestRoomParameters);
            else
                room = new EnemyRoom(center, floorTiles, enemyRoomParameters);
            dungeonData.Rooms.Add(room);
        }

        Debug.Log("Termine dijkstra");
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
                //if (dungeonData.Path.Contains(floorPosition))
                //   continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles UP
            Gizmos.color = Color.blue;
            foreach (Vector2Int floorPosition in room.NearWallUpTiles)
            {
                //if (dungeonData.Path.Contains(floorPosition))
                //    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles DOWN
            Gizmos.color = Color.green;
            foreach (Vector2Int floorPosition in room.NearWallDownTiles)
            {
                //if (dungeonData.Path.Contains(floorPosition))
                //    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles RIGHT
            Gizmos.color = Color.white;
            foreach (Vector2Int floorPosition in room.NearWallRightTiles)
            {
                //if (dungeonData.Path.Contains(floorPosition))
                //    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles LEFT
            Gizmos.color = Color.cyan;
            foreach (Vector2Int floorPosition in room.NearWallLeftTiles)
            {
                //if (dungeonData.Path.Contains(floorPosition))
                //    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles CORNERS
            Gizmos.color = Color.magenta;
            foreach (Vector2Int floorPosition in room.CornerTiles)
            {
                //if (dungeonData.Path.Contains(floorPosition))
                //    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }

            Gizmos.color = Color.black;
            foreach (Vector2Int deadEndPosition in room.DeadEndTiles)
            {
                Gizmos.DrawCube(deadEndPosition + Vector2.one * 0.5f, Vector2.one);
            }

            Gizmos.color = Color.red;
            foreach (Vector2Int corridorTile in room.CorridorTiles)
            {
                Gizmos.DrawCube(corridorTile + Vector2.one * 0.5f, Vector2.one);
            }

        }
    }
}
