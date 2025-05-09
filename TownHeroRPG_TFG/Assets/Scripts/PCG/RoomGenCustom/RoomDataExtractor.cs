//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class RoomDataExtractor : MonoBehaviour
//{
//    private DungeonData dungeonData;

//    [SerializeField]
//    private bool showGizmo = false;

//    public UnityEvent OnFinishedRoomProcessing;

//    private void Awake()
//    {
//        dungeonData = FindObjectOfType<DungeonData>();
//    }
//    public void ProcessRooms()
//    {
//        if (dungeonData == null)
//            return;
//        Debug.Log("lLEGO LA HRAS");
//        foreach (Room room in dungeonData.Rooms)
//        {
//            //find corener, near wall and inner tiles
//            foreach (Vector2Int tilePosition in room.FloorTiles)
//            {
//                int neighboursCount = 4;

//                if (room.FloorTiles.Contains(tilePosition + Vector2Int.up) == false)
//                {
//                    room.NearWallTilesUp.Add(tilePosition);
//                    neighboursCount--;
//                }
//                if (room.FloorTiles.Contains(tilePosition + Vector2Int.down) == false)
//                {
//                    room.NearWallTilesDown.Add(tilePosition);
//                    neighboursCount--;
//                }
//                if (room.FloorTiles.Contains(tilePosition + Vector2Int.right) == false)
//                {
//                    room.NearWallTilesRight.Add(tilePosition);
//                    neighboursCount--;
//                }
//                if (room.FloorTiles.Contains(tilePosition + Vector2Int.left) == false)
//                {
//                    room.NearWallTilesLeft.Add(tilePosition);
//                    neighboursCount--;
//                }

//                //find corners
//                if (neighboursCount <= 2)
//                    room.CornerTiles.Add(tilePosition);

//                if (neighboursCount == 4)
//                    room.InnerTiles.Add(tilePosition);
//            }

//            room.NearWallTilesUp.ExceptWith(room.CornerTiles);
//            room.NearWallTilesDown.ExceptWith(room.CornerTiles);
//            room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
//            room.NearWallTilesRight.ExceptWith(room.CornerTiles);
//        }

//        //OnFinishedRoomProcessing?.Invoke();
//        Invoke("RunEvent", 1);
//    }

//    public void RunEvent()
//    {
//        OnFinishedRoomProcessing?.Invoke();
//    }

//    private void OnDrawGizmosSelected()
//    {
//        if (dungeonData == null || showGizmo == false)
//            return;
//        foreach (Room room in dungeonData.Rooms)
//        {
//            //Draw inner tiles
//            Gizmos.color = Color.yellow;
//            foreach (Vector2Int floorPosition in room.InnerTiles)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//            //Draw near wall tiles UP
//            Gizmos.color = Color.blue;
//            foreach (Vector2Int floorPosition in room.NearWallTilesUp)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//            //Draw near wall tiles DOWN
//            Gizmos.color = Color.green;
//            foreach (Vector2Int floorPosition in room.NearWallTilesDown)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//            //Draw near wall tiles RIGHT
//            Gizmos.color = Color.white;
//            foreach (Vector2Int floorPosition in room.NearWallTilesRight)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//            //Draw near wall tiles LEFT
//            Gizmos.color = Color.cyan;
//            foreach (Vector2Int floorPosition in room.NearWallTilesLeft)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//            //Draw near wall tiles CORNERS
//            Gizmos.color = Color.magenta;
//            foreach (Vector2Int floorPosition in room.CornerTiles)
//            {
//                if (dungeonData.Path.Contains(floorPosition))
//                    continue;
//                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
//            }
//        }
//    }
//}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomDataExtractor : MonoBehaviour
{
    private DungeonData dungeonData;
    [SerializeField] private GameObject square;

    [SerializeField]
    private bool showGizmo = false;

    public UnityEvent OnFinishedRoomProcessing;
    [SerializeField] private int playerRoomIndex = 0;

    // Direcciones ampliadas (8 direcciones)
    private List<Vector2Int> allDirections = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
    };

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
    }


    //public void ProcessRooms()
    //{
    //    Debug.Log("LLego la hora");
    //    if (dungeonData == null)
    //        return;


    //    foreach (Room room in dungeonData.Rooms)
    //    {
    //        room.NearWallTilesUp.Clear();
    //        room.NearWallTilesDown.Clear();
    //        room.NearWallTilesLeft.Clear();
    //        room.NearWallTilesRight.Clear();
    //        room.CornerTiles.Clear();
    //        room.InnerTiles.Clear();

    //        foreach (Vector2Int tilePosition in room.FloorTiles)
    //        {
    //            HashSet<Vector2Int> missingDirections = new HashSet<Vector2Int>();

    //            // Verificar las 8 direcciones para detectar paredes cercanas
    //            foreach (Vector2Int dir in allDirections)
    //            {
    //                if (!room.FloorTiles.Contains(tilePosition + dir))
    //                    missingDirections.Add(dir);
    //            }

    //            // Clasificar tiles cercanos a paredes (cardinales específicos)
    //            if (missingDirections.Contains(Vector2Int.up)) room.NearWallTilesUp.Add(tilePosition);
    //            if (missingDirections.Contains(Vector2Int.down)) room.NearWallTilesDown.Add(tilePosition);
    //            if (missingDirections.Contains(Vector2Int.left)) room.NearWallTilesLeft.Add(tilePosition);
    //            if (missingDirections.Contains(Vector2Int.right)) room.NearWallTilesRight.Add(tilePosition);

    //            // Detección de esquinas: 3 o más direcciones faltantes en radio ampliado
    //            int missingInRadius = 0;
    //            foreach (Vector2Int dir in allDirections)
    //            {
    //                if (!room.FloorTiles.Contains(tilePosition + dir)) missingInRadius++;
    //            }
    //            if (missingInRadius >= 5) // Ajuste empírico para formas orgánicas
    //                room.CornerTiles.Add(tilePosition);

    //            // Tiles interiores: Todos los vecinos cardinales presentes
    //            if (CheckCardinalNeighbors(tilePosition, room.FloorTiles))
    //                room.InnerTiles.Add(tilePosition);
    //        }

    //        // Excluir esquinas de los NearWallTiles
    //        room.NearWallTilesUp.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesDown.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesRight.ExceptWith(room.CornerTiles);
    //    }

    //    Invoke("RunEvent", 0.1f);
    //}
    //public void ProcessRooms()
    //{
    //    if (dungeonData == null) return;

    //    Debug.Log("En total hay:" + dungeonData.Rooms.Count);
    //    foreach (Room room in dungeonData.Rooms)
    //    {
    //        room.NearWallTilesUp.Clear();
    //        room.NearWallTilesDown.Clear();
    //        room.NearWallTilesLeft.Clear();
    //        room.NearWallTilesRight.Clear();
    //        room.CornerTiles.Clear();
    //        room.InnerTiles.Clear();

    //        foreach (Vector2Int tilePosition in room.FloorTiles)
    //        {
    //            // Dentro del bucle foreach (Vector2Int tilePosition...)
    //            string neighborPatternBasic = WallPlacer.GenerateNeighborPattern(
    //                tilePosition,
    //                room.FloorTiles,
    //                Directions.cardinalDirections // Usar direcciones cardinales para básicos
    //            );
    //            int patternValueBasic = Convert.ToInt32(neighborPatternBasic, 2);
    //            string wallTypeBasic = WallPlacer.DetermineWallType(patternValueBasic, isBasicWall: true);

    //            string neighborPatternComplex = WallPlacer.GenerateNeighborPattern(
    //                tilePosition,
    //                room.FloorTiles,
    //                Directions.allDirections // Usar todas las direcciones para complejos
    //            );
    //            int patternValueComplex = Convert.ToInt32(neighborPatternComplex, 2);
    //            string wallTypeComplex = WallPlacer.DetermineWallType(patternValueComplex, isBasicWall: false);


    //            switch (wallTypeComplex)
    //            {
    //                case "InnerCornerDownLeft":
    //                case "InnerCornerDownRight":
    //                case "DiagonalCornerDownLeft":
    //                case "DiagonalCornerDownRight":
    //                    room.CornerTiles.Add(tilePosition);
    //                    break;
    //            }

    //            // Clasificar según ambos resultados
    //            switch (wallTypeBasic)
    //            {
    //                case "Top":
    //                    room.NearWallTilesUp.Add(tilePosition);
    //                    break;
    //                case "Bottom":
    //                    room.NearWallTilesDown.Add(tilePosition);
    //                    break;
    //                case "Left":
    //                    room.NearWallTilesLeft.Add(tilePosition);
    //                    break;
    //                case "Right":
    //                    room.NearWallTilesRight.Add(tilePosition);
    //                    break;
    //            }

    //            // Tiles interiores (sin muros básicos)
    //            if (wallTypeBasic == "Default" || wallTypeComplex == "Default")
    //            {
    //                room.InnerTiles.Add(tilePosition);
    //                GameObject newSquare = Instantiate(square, new Vector3(tilePosition.x,tilePosition.y,0), Quaternion.identity);
    //            }
    //        }

    //        Debug.LogError("Inner tiles:" + room.InnerTiles.Count + " en la habitación: " + room.RoomCenterPos);

    //        // Excluir esquinas de las paredes cercanas
    //        room.NearWallTilesUp.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesDown.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
    //        room.NearWallTilesRight.ExceptWith(room.CornerTiles);
    //    }
    //    AssignPlayerRoomAndSpecialRooms();
    //    Invoke("RunEvent", 0.1f);
    //}

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;

        foreach (Room room in dungeonData.Rooms)
        {
            room.NearWallTilesUp.Clear();
            room.NearWallTilesDown.Clear();
            room.NearWallTilesLeft.Clear();
            room.NearWallTilesRight.Clear();
            room.CornerTiles.Clear();
            room.InnerTiles.Clear();

            foreach (Vector2Int tilePosition in room.FloorTiles)
            {
                HashSet<Vector2Int> missingDirections = new HashSet<Vector2Int>();

                // Verificar las 8 direcciones para detectar paredes cercanas
                foreach (Vector2Int dir in allDirections)
                {
                    if (!room.FloorTiles.Contains(tilePosition + dir))
                        missingDirections.Add(dir);
                }

                // Clasificar tiles cercanos a paredes (cardinales específicos)
                if (missingDirections.Contains(Vector2Int.up)) room.NearWallTilesUp.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.down)) room.NearWallTilesDown.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.left)) room.NearWallTilesLeft.Add(tilePosition);
                if (missingDirections.Contains(Vector2Int.right)) room.NearWallTilesRight.Add(tilePosition);

                // Detección de esquinas: 3 o más direcciones faltantes en radio ampliado
                int missingInRadius = 0;
                foreach (Vector2Int dir in allDirections)
                {
                    if (!room.FloorTiles.Contains(tilePosition + dir)) missingInRadius++;
                }
                if (missingInRadius >= 5) // Ajuste empírico para formas orgánicas
                    room.CornerTiles.Add(tilePosition);

                // Tiles interiores: Todos los vecinos cardinales presentes
                if (CheckCardinalNeighbors(tilePosition, room.FloorTiles))
                    room.InnerTiles.Add(tilePosition);
            }

            // Excluir esquinas de los NearWallTiles
            room.NearWallTilesUp.ExceptWith(room.CornerTiles);
            room.NearWallTilesDown.ExceptWith(room.CornerTiles);
            room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
            room.NearWallTilesRight.ExceptWith(room.CornerTiles);
        }
        AssignPlayerRoomAndSpecialRooms();
        Invoke("RunEvent", 0.1f);
    }

    private bool CheckCardinalNeighbors(Vector2Int position, HashSet<Vector2Int> floorTiles)
    {
        return floorTiles.Contains(position + Vector2Int.up) &&
               floorTiles.Contains(position + Vector2Int.down) &&
               floorTiles.Contains(position + Vector2Int.left) &&
               floorTiles.Contains(position + Vector2Int.right);


        //return floorTiles.Contains(position + Vector2Int.up) &&
        //       floorTiles.Contains(position + Vector2Int.down) &&
        //       floorTiles.Contains(position + Vector2Int.left) &&
        //       floorTiles.Contains(position + Vector2Int.right) &&
        //       floorTiles.Contains(position + new Vector2Int(1, 1)) &&    // NORTHEAST
        //       floorTiles.Contains(position + new Vector2Int(1, -1)) &&   // SOUTHEAST
        //       floorTiles.Contains(position + new Vector2Int(-1, -1)) &&  // SOUTHWEST
        //       floorTiles.Contains(position + new Vector2Int(-1, 1));    // NORTHWEST;
    }

    public void RunEvent() => OnFinishedRoomProcessing?.Invoke();

    private void AssignPlayerRoomAndSpecialRooms()
    {
        // Asignar habitación del jugador
        int index = Mathf.Clamp(playerRoomIndex, 0, dungeonData.Rooms.Count - 1);
        Room playerRoom = dungeonData.Rooms[index];
        Vector2Int playerTile = Vector2Int.RoundToInt(playerRoom.RoomCenterPos);
        dungeonData.PlayerStartTile = playerTile;
        dungeonData.PlayerRoom = playerRoom;

        // Crear grafo y ejecutar Dijkstra
        Graph graph = new Graph(dungeonData.floor);
        Dictionary<Vector2Int, int> distances = DijkstraAlgorithm.Dijkstra(graph, playerTile);

        // Calcular distancias a cada habitación (excluyendo la del jugador)
        List<(Room room, int distance)> roomDistances = new List<(Room, int)>();
        foreach (Room room in dungeonData.Rooms)
        {
            if (room == dungeonData.PlayerRoom) continue;
            Vector2Int centerTile = Vector2Int.RoundToInt(room.RoomCenterPos);
            if (distances.ContainsKey(centerTile))
            {
                roomDistances.Add((room, distances[centerTile]));
            }
            else
            {
                Debug.LogWarning("La habitación no es accesible: " + centerTile);
            }
        }

        // Ordenar habitaciones por distancia (más lejana primero)
        var sortedRooms = roomDistances.OrderByDescending(r => r.distance).ToList();
        if (sortedRooms.Count >= 1)
        {
            sortedRooms[0].room.Type = RoomType.Boss; // Habitación más lejana es la del jefe
        }
        if (sortedRooms.Count >= 2)
        {
            Debug.Log("Lo hice");
            sortedRooms[1].room.Type = RoomType.Chest; // Segunda más lejana es la del cofre
        }
    }

    private void OnDrawGizmosSelected()
    {
        //if (dungeonData == null || !showGizmo) return;

        //foreach (Room room in dungeonData.Rooms)
        //{
        //    // Dibujar tiles interiores (amarillo)
        //    Gizmos.color = Color.yellow;
        //    foreach (Vector2Int pos in room.InnerTiles)
        //        Gizmos.DrawCube(pos + Vector2.one * 0.5f, Vector2.one * 0.9f);

        //    // Dibujar esquinas (magenta)
        //    Gizmos.color = Color.magenta;
        //    foreach (Vector2Int pos in room.CornerTiles)
        //        Gizmos.DrawSphere(pos + Vector2.one * 0.5f, 0.2f);

        //    // Dibujar paredes cercanas (colores direccionales)
        //    Gizmos.color = Color.blue;
        //    foreach (Vector2Int pos in room.NearWallTilesUp)
        //        Gizmos.DrawCube(pos + Vector2.one * 0.5f, new Vector2(0.5f, 0.2f));

        //    Gizmos.color = Color.green;
        //    foreach (Vector2Int pos in room.NearWallTilesDown)
        //        Gizmos.DrawCube(pos + Vector2.one * 0.5f, new Vector2(0.5f, 0.2f));

        //    Gizmos.color = Color.cyan;
        //    foreach (Vector2Int pos in room.NearWallTilesLeft)
        //        Gizmos.DrawCube(pos + Vector2.one * 0.5f, new Vector2(0.2f, 0.5f));

        //    Gizmos.color = Color.white;
        //    foreach (Vector2Int pos in room.NearWallTilesRight)
        //        Gizmos.DrawCube(pos + Vector2.one * 0.5f, new Vector2(0.2f, 0.5f));
        //}
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
            foreach (Vector2Int floorPosition in room.NearWallTilesUp)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles DOWN
            Gizmos.color = Color.green;
            foreach (Vector2Int floorPosition in room.NearWallTilesDown)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles RIGHT
            Gizmos.color = Color.white;
            foreach (Vector2Int floorPosition in room.NearWallTilesRight)
            {
                if (dungeonData.Path.Contains(floorPosition))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles LEFT
            Gizmos.color = Color.cyan;
            foreach (Vector2Int floorPosition in room.NearWallTilesLeft)
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
        }
    }
}
