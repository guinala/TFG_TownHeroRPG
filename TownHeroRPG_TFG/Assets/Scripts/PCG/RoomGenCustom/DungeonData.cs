using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DungeonData : MonoBehaviour
{
    public Dictionary<Vector2Int, HashSet<Vector2Int>> rooms = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    public List<Room> Rooms { get; set; }
    public HashSet<Vector2Int> Path { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> floor { get; set; }
    public PlayerRoom PlayerRoom { get; set; }
    public GameObject PlayerReference { get; set; }

    private void Awake()
    {
        Rooms = new List<Room>();
    }

    public void ClearData()
    {
        foreach (Room room in Rooms)
        {
            room.PropObjects.ForEach(go => DestroyImmediate(go));
            room.EnemiesInRoom.ForEach(go => DestroyImmediate(go));
            room.SpecialItemsInRoom.ForEach(go => DestroyImmediate(go));
        }

        Rooms = new();
        Path = new();
        DestroyImmediate(PlayerReference);
    }

    public void InitializeRoomDictionary(Dictionary<Vector2Int, HashSet<Vector2Int>> roomDictionary, HashSet<Vector2Int> Corridors, HashSet<Vector2Int> Floor)
    {
        Rooms = new();
        rooms = roomDictionary;
        floor = Floor;
        Path = new HashSet<Vector2Int>(Corridors);
    }
}
