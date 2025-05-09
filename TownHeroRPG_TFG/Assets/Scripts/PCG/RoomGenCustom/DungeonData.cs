using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Stores all the data about our dungeon.
/// Useful when creating a save / load system
/// </summary>
public class DungeonData : MonoBehaviour
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public HashSet<Vector2Int> Path { get; set; } = new HashSet<Vector2Int>();
    // Added properties for Dijkstra and room assignment
    public Vector2Int PlayerStartTile { get; set; }
    public Room PlayerRoom { get; set; }
    public HashSet<Vector2Int> floor { get; set; }
    public HashSet<Vector2Int> corridors { get; set; }
    public Dictionary<Vector2Int,HashSet<Vector2Int>> rooms = new Dictionary<Vector2Int, HashSet<Vector2Int>>();


    public GameObject PlayerReference { get; set; }
    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach (var item in room.PropObjectReferences)
            {
                Destroy(item);
            }
            foreach (var item in room.EnemiesInTheRoom)
            {
                Destroy(item);
            }
        }
        Rooms = new();
        Path = new();
        //Destroy(PlayerReference);
        DestroyImmediate(PlayerReference);
    }

    public IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }

    public void InitializeRoomDictionary(Dictionary<Vector2Int, HashSet<Vector2Int>> roomDictionary, HashSet<Vector2Int> Corridors, HashSet<Vector2Int> Floor, List<Room> roomsList)
    {
        rooms = roomDictionary;
        corridors = Corridors;
        floor = Floor;
        Path = new HashSet<Vector2Int>(corridors);
        Rooms = roomsList;
        Debug.Log("Odio mi vida");
    }

    public HashSet<Vector2Int> GetRoomFloorWithoutCorridors(Vector2Int dictionaryKey)
    {
        HashSet<Vector2Int> roomFloorNoCorridors = new HashSet<Vector2Int>(rooms[dictionaryKey]);
        roomFloorNoCorridors.ExceptWith(Path);
        return roomFloorNoCorridors;
    }
}

public enum RoomType { Normal, Boss, Chest }
/// <summary>
/// Holds all the data about the room
/// </summary>
public class Room
{
    public Vector2 RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();
   
    public RoomType Type { get; set; } = RoomType.Normal;

    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();

    public List<Vector2Int> PositionsAccessibleFromPath { get; set; } = new List<Vector2Int>();

    public List<GameObject> EnemiesInTheRoom { get; set; } = new List<GameObject>();

    public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}