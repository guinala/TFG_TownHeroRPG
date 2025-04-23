using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DungeonAlgorithms
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int start, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(start);
        
        Vector2Int pos = start;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int direction = Directions.GetCardinalDirectionRandomly();
            pos += direction;
            path.Add(pos);
        }
        
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridors(Vector2Int start, int corridorWidth)
    {
        List<Vector2Int> corridorPath = new List<Vector2Int>();
        Vector2Int direction = Directions.GetCardinalDirectionRandomly();
        Vector2Int pos = start;

        corridorPath.Add(pos);

        for (int i = 0; i < corridorWidth; i++)
        {
            pos += direction;
            corridorPath.Add(pos);
        }

        return corridorPath;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            BoundsInt currentRoom = roomsQueue.Dequeue();
            bool meetsMinSize = currentRoom.size.x >= minWidth && currentRoom.size.y >= minHeight;

            if (!meetsMinSize) continue;

            bool splitVerticalFirst = Random.value < 0.5f;
            bool canSplitVertical = currentRoom.size.x >= minWidth * 2;
            bool canSplitHorizontal = currentRoom.size.y >= minHeight * 2;

            if (splitVerticalFirst)
            {
                if (canSplitVertical)
                {
                    SplitVertically(minWidth, roomsQueue, currentRoom);
                }
                else if (canSplitHorizontal)
                {
                    SplitHorizontally(minHeight, roomsQueue, currentRoom);
                }
                else
                {
                    roomsList.Add(currentRoom);
                }
            }
            else
            {
                if (canSplitHorizontal)
                {
                    SplitHorizontally(minHeight, roomsQueue, currentRoom);
                }
                else if (canSplitVertical)
                {
                    SplitVertically(minWidth, roomsQueue, currentRoom);
                }
                else
                {
                    roomsList.Add(currentRoom);
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int splitPointX = Random.Range(1, room.size.x);
        Vector3Int room1Size = new Vector3Int(splitPointX, room.size.y, room.size.z);
        Vector3Int room2Start = new Vector3Int(room.min.x + splitPointX, room.min.y, room.min.z);
        Vector3Int room2Size = new Vector3Int(room.size.x - splitPointX, room.size.y, room.size.z);

        roomsQueue.Enqueue(new BoundsInt(room.min, room1Size));
        roomsQueue.Enqueue(new BoundsInt(room2Start, room2Size));
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        int splitPointY = Random.Range(1, room.size.y);
        Vector3Int room1Size = new Vector3Int(room.size.x, splitPointY, room.size.z);
        Vector3Int room2Start = new Vector3Int(room.min.x, room.min.y + splitPointY, room.min.z);
        Vector3Int room2Size = new Vector3Int(room.size.x, room.size.y - splitPointY, room.size.z);

        roomsQueue.Enqueue(new BoundsInt(room.min, room1Size));
        roomsQueue.Enqueue(new BoundsInt(room2Start, room2Size));
    }
}








