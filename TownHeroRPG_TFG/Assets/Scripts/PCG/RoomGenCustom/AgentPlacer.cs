//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class AgentPlacer : MonoBehaviour
//{
//    [SerializeField] private DungeonData dungeonData;
//    [SerializeField] private GameObject playerPrefab;
//    [SerializeField] private GameObject bossPrefab; // Jefe asignado vía Inspector
//    [SerializeField] private GameObject[] enemyPrefabs;
//    [SerializeField] private int playerRoomIndex = 0;

//    public void PlaceAgents()
//    {
//        if (dungeonData.Rooms.Count == 0) return;

//        // Colocar al jugador
//        int index = Mathf.Clamp(playerRoomIndex, 0, dungeonData.Rooms.Count - 1);
//        Room playerRoom = dungeonData.Rooms[index];
//        Vector2Int playerTile = Vector2Int.RoundToInt(playerRoom.RoomCenterPos);
//        GameObject player = Instantiate(playerPrefab);
//        player.transform.localPosition = (Vector2)playerTile + Vector2.one * 0.5f;
//        dungeonData.PlayerStartTile = playerTile;
//        dungeonData.PlayerRoom = playerRoom;

//        // Determinar habitaciones especiales (jefe y cofre) usando Dijkstra
//        DetermineSpecialRooms();

//        // Calcular posiciones accesibles para cada habitación
//        CalculateAccessiblePositionsForRooms();

//        // Colocar jefe y enemigos
//        foreach (Room room in dungeonData.Rooms)
//        {
//            if (room.Type == RoomType.Boss)
//            {
//                // Colocar solo al jefe en la habitación del jefe
//                if (room.PositionsAccessibleFromPath.Count > 0)
//                {
//                    Vector2Int bossPosition = room.PositionsAccessibleFromPath[0];
//                    GameObject boss = Instantiate(bossPrefab);
//                    boss.transform.localPosition = (Vector2)bossPosition + Vector2.one * 0.5f;
//                    room.EnemiesInTheRoom.Add(boss);
//                }
//                else
//                {
//                    Debug.LogWarning("No hay posiciones accesibles en la habitación del jefe.");
//                }
//            }
//            else if (room.Type != RoomType.Chest && room != dungeonData.PlayerRoom)
//            {
//                // Colocar enemigos regulares en habitaciones no especiales (excluyendo la habitación del jugador)
//                int enemyCount = Random.Range(1, 3); // Ajustar según sea necesario
//                PlaceEnemies(room, enemyCount);
//            }
//        }
//    }

//    private void PlaceEnemies(Room room, int count)
//    {
//        List<Vector2Int> accessiblePositions = new List<Vector2Int>(room.PositionsAccessibleFromPath);
//        count = Mathf.Min(count, accessiblePositions.Count);

//        for (int i = 0; i < count; i++)
//        {
//            Vector2Int pos = accessiblePositions[Random.Range(0, accessiblePositions.Count)];
//            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
//            enemy.transform.localPosition = (Vector2)pos + Vector2.one * 0.5f;
//            room.EnemiesInTheRoom.Add(enemy);
//            accessiblePositions.Remove(pos);
//        }
//    }

//    private void DetermineSpecialRooms()
//    {
//        // Crear grafo y ejecutar Dijkstra
//        Graph graph = new Graph(dungeonData.floor);
//        Dictionary<Vector2Int, int> distances = DijkstraAlgorithm.Dijkstra(graph, dungeonData.PlayerStartTile);

//        // Calcular distancias a cada habitación (excluyendo la del jugador)
//        List<(Room room, int distance)> roomDistances = new List<(Room, int)>();
//        foreach (Room room in dungeonData.Rooms)
//        {
//            if (room == dungeonData.PlayerRoom) continue;
//            Vector2Int centerTile = Vector2Int.RoundToInt(room.RoomCenterPos);
//            if (distances.ContainsKey(centerTile))
//            {
//                roomDistances.Add((room, distances[centerTile]));
//            }
//            else
//            {
//                Debug.LogWarning("La habitación no es accesible: " + centerTile);
//            }
//        }

//        // Ordenar habitaciones por distancia (más lejana primero)
//        var sortedRooms = roomDistances.OrderByDescending(r => r.distance).ToList();
//        if (sortedRooms.Count >= 1)
//        {
//            sortedRooms[0].room.Type = RoomType.Boss; // Habitación más lejana es la del jefe
//        }
//        if (sortedRooms.Count >= 2)
//        {
//            sortedRooms[1].room.Type = RoomType.Chest; // Segunda más lejana es la del cofre
//        }
//    }

//    private void CalculateAccessiblePositionsForRooms()
//    {
//        foreach (Room room in dungeonData.Rooms)
//        {
//            // Encontrar tiles accesibles usando BFS
//            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);
//            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
//            roomFloor.IntersectWith(dungeonData.Path);
//            if (roomFloor.Count > 0)
//            {
//                Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);
//                room.PositionsAccessibleFromPath = roomMap.Keys.ToList();
//            }
//            else
//            {
//                Debug.LogWarning("No se encontró un tile de camino en la habitación.");
//                room.PositionsAccessibleFromPath = new List<Vector2Int>();
//            }
//        }
//    }
//}

//// Clase auxiliar para el grafo de la habitación y BFS
//public class RoomGraph
//{
//    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();
//    public static List<Vector2Int> fourDirections = new List<Vector2Int>
//    {
//        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
//    };

//    public RoomGraph(HashSet<Vector2Int> roomFloor)
//    {
//        foreach (Vector2Int pos in roomFloor)
//        {
//            List<Vector2Int> neighbours = new List<Vector2Int>();
//            foreach (Vector2Int direction in fourDirections)
//            {
//                Vector2Int newPos = pos + direction;
//                if (roomFloor.Contains(newPos))
//                {
//                    neighbours.Add(newPos);
//                }
//            }
//            graph.Add(pos, neighbours);
//        }
//    }

//    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
//    {
//        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
//        nodesToVisit.Enqueue(startPos);

//        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int> { startPos };

//        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int> { { startPos, startPos } };

//        while (nodesToVisit.Count > 0)
//        {
//            Vector2Int node = nodesToVisit.Dequeue();
//            List<Vector2Int> neighbours = graph[node];

//            foreach (Vector2Int neighbourPosition in neighbours)
//            {
//                if (!visitedNodes.Contains(neighbourPosition) && !occupiedNodes.Contains(neighbourPosition))
//                {
//                    nodesToVisit.Enqueue(neighbourPosition);
//                    visitedNodes.Add(neighbourPosition);
//                    map[neighbourPosition] = node;
//                }
//            }
//        }

//        return map;
//    }
//}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentPlacer : MonoBehaviour
{
    [SerializeField] private DungeonData dungeonData;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bossPrefab; // Jefe asignado vía Inspector
    [SerializeField] private GameObject[] enemyPrefabs;

    public void PlaceAgents()
    {
        if (dungeonData == null || dungeonData.Rooms.Count == 0) return;

        // Colocar al jugador en la habitación designada
        Room playerRoom = dungeonData.PlayerRoom;
        if (playerRoom == null)
        {
            Debug.LogError("No se ha asignado la habitación del jugador.");
            return;
        }
        Vector2Int playerTile = dungeonData.PlayerStartTile;
        GameObject player = Instantiate(playerPrefab);
        player.transform.localPosition = (Vector2)playerTile + Vector2.one * 0.5f;

        // Calcular posiciones accesibles para cada habitación
        CalculateAccessiblePositionsForRooms();

        // Colocar jefe y enemigos
        foreach (Room room in dungeonData.Rooms)
        {
            if (room.Type == RoomType.Boss)
            {
                // Colocar solo al jefe en la habitación del jefe
                if (room.PositionsAccessibleFromPath.Count > 0)
                {
                    Vector2Int bossPosition = room.PositionsAccessibleFromPath[0];
                    GameObject boss = Instantiate(bossPrefab);
                    boss.transform.localPosition = (Vector2)bossPosition + Vector2.one * 0.5f;
                    room.EnemiesInTheRoom.Add(boss);
                }
                else
                {
                    Debug.LogWarning("No hay posiciones accesibles en la habitación del jefe.");
                }
            }
            else if (room.Type != RoomType.Chest && room != dungeonData.PlayerRoom)
            {
                // Colocar enemigos regulares en habitaciones no especiales (excluyendo la del jugador)
                int enemyCount = Random.Range(1, 3); // Ajustar según sea necesario
                PlaceEnemies(room, enemyCount);
            }
        }
    }

    private void PlaceEnemies(Room room, int count)
    {
        List<Vector2Int> accessiblePositions = new List<Vector2Int>(room.PositionsAccessibleFromPath);
        count = Mathf.Min(count, accessiblePositions.Count);

        for (int i = 0; i < count; i++)
        {
            if (accessiblePositions.Count == 0) break;
            Vector2Int pos = accessiblePositions[Random.Range(0, accessiblePositions.Count)];
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
            enemy.transform.localPosition = (Vector2)pos + Vector2.one * 0.5f;
            room.EnemiesInTheRoom.Add(enemy);
            accessiblePositions.Remove(pos);
        }
    }

    private void CalculateAccessiblePositionsForRooms()
    {
        foreach (Room room in dungeonData.Rooms)
        {
            // Encontrar tiles accesibles usando BFS
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
            roomFloor.IntersectWith(dungeonData.Path);
            if (roomFloor.Count > 0)
            {
                Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);
                room.PositionsAccessibleFromPath = roomMap.Keys.ToList();
            }
            else
            {
                Debug.LogWarning("No se encontró un tile de camino en la habitación: " + room.RoomCenterPos);
                room.PositionsAccessibleFromPath = new List<Vector2Int>();
            }
        }
    }
}

public class RoomGraph
{
    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();
    public static List<Vector2Int> fourDirections = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in fourDirections)
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