using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
using Random = UnityEngine.Random;

public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

public class PrefabPlacer : MonoBehaviour
{
    private DungeonData dungeonData;
    private RandomWalkRoomData randomWalkData;

    [SerializeField]
    private EnemyRoomParameters parameters;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f;
    private float deadPropPlacementChance = 0.75f;
    private float corridorPropPlacementChance = 0.4f;

    [SerializeField]
    private GameObject propPrefab;
    public UnityEvent OnFinished;

    #region PropPlacement

    public void ProcessRoomProps(DungeonData data)
    {
        if (data == null)
            return;

        dungeonData = data;
        foreach (Room room in dungeonData.Rooms)
        {
            ExtractProps(room);
        }

        PlaceSpecialObjects();
    }

    private void ExtractProps(Room room)
    {
        List<Prop> propsToPlace = parameters.Props;

        if (room is RandomWalkDungeonRoom rwRoom)
        {
            propsToPlace = rwRoom.RandomWalkDungeonParameters.Props;
        }

        if (room is BossRoom bossRoom)
        {
            propsToPlace = bossRoom.BossRoomParameters.Props;
        }
        else if (room is ChestRoom chestRoom)
        {
            propsToPlace = chestRoom.ChestRoomParameters.Props;
            Vector2Int center = Vector2Int.RoundToInt(room.RoomCenterPos);
        }
        else if (room is PlayerRoom playerRoom)
        {
            propsToPlace = playerRoom.PlayerRoomParameters.Props;
        }

        List<Prop> deadEndProps = propsToPlace.Where(x => x.DeadEnd).ToList();
        if (deadEndProps.Count > 0)
            PlaceDeadEndProps(room, deadEndProps);

        List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
        if (cornerProps.Count > 0)
            PlaceCornerProps(room, cornerProps);

        List<Prop> corridorProps = propsToPlace.Where(x => x.Corridor).ToList();
        if (corridorProps.Count > 0)
            PlaceCorridorProps(room, corridorProps);

        List<Prop> leftWallProps = propsToPlace
        .Where(x => x.NearLeftWall)
        .OrderByDescending(x => x.Size.x * x.Size.y)
        .ToList();

        if (leftWallProps.Count > 0)
            PlaceProps(room, leftWallProps, room.NearWallLeftTiles, PlacementOriginCorner.BottomLeft);

        List<Prop> rightWallProps = propsToPlace
        .Where(x => x.NearRightWall)
        .OrderByDescending(x => x.Size.x * x.Size.y)
        .ToList();

        if (rightWallProps.Count > 0)
            PlaceProps(room, rightWallProps, room.NearWallRightTiles, PlacementOriginCorner.TopRight);

        List<Prop> topWallProps = propsToPlace
        .Where(x => x.NearUpWall)
        .OrderByDescending(x => x.Size.x * x.Size.y)
        .ToList();

        if (topWallProps.Count > 0)
            PlaceProps(room, topWallProps, room.NearWallUpTiles, PlacementOriginCorner.TopLeft);

        List<Prop> downWallProps = propsToPlace
        .Where(x => x.NearDownWall)
        .OrderByDescending(x => x.Size.x * x.Size.y)
        .ToList();

        if (downWallProps.Count > 0)
            PlaceProps(room, downWallProps, room.NearWallDownTiles, PlacementOriginCorner.BottomLeft);

        List<Prop> innerProps = propsToPlace
            .Where(x => x.Inner)
            .OrderByDescending(x => x.Size.x * x.Size.y)
            .ToList();
        if (innerProps.Count > 0)
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
    }

    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);

        if (dungeonData != null)
            tempPositons.ExceptWith(dungeonData.Path);

        foreach (Prop propToPlace in wallProps)
        {
            int quantity = Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                tempPositons.ExceptWith(room.PropPositions);
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                if (TryToPlacePropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }

        }
    }

    private bool TryToPlacePropBruteForce(Room room, Prop prop, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            List<Vector2Int> freePositionsAround = TryToFitProp(prop, availablePositions, position, placement);

            if (freePositionsAround.Count == prop.Size.x * prop.Size.y)
            {
                PlacePropGameObject(room, position, prop);
                foreach (Vector2Int pos in freePositionsAround)
                {
                    room.PropPositions.Add(pos);
                }

                if (prop.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, prop, 1);
                }
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> TryToFitProp(Prop prop, List<Vector2Int> availablePositions, Vector2Int originPosition, PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        if (placement == PlacementOriginCorner.BottomLeft)
        {
            for (int xOffset = 0; xOffset < prop.Size.x; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.Size.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -prop.Size.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.Size.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.TopLeft)
        {
            for (int xOffset = 0; xOffset < prop.Size.x; xOffset++)
            {
                for (int yOffset = -prop.Size.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else
        {
            for (int xOffset = -prop.Size.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = -prop.Size.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }

        return freePositions;
    }

    private void PlaceCornerProps(Room room, List<Prop> props)
    {
        float chance = cornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (Random.value < chance)
            {
                Prop propToPlace = props[Random.Range(0, props.Count)];

                if (!room.PropPositions.Contains(cornerTile))
                {
                    if (dungeonData != null)
                    {
                        if (!dungeonData.Path.Contains(cornerTile))
                        {
                            PlacePropGameObject(room, cornerTile, propToPlace);
                        }
                    }
                    else
                    {
                        PlacePropGameObject(room, cornerTile, propToPlace);
                    }
                }

                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 1);
                }
            }
            else
            {
                chance = Mathf.Clamp01(chance + 0.1f);
            }
        }
    }

    private void PlaceDeadEndProps(Room room, List<Prop> props)
    {
        float chance = deadPropPlacementChance;

        foreach (Vector2Int deadEndTile in room.DeadEndTiles)
        {
            if (Random.value < chance)
            {
                Prop propToPlace = props[Random.Range(0, props.Count)];

                if (!room.PropPositions.Contains(deadEndTile))
                {
                    if (dungeonData != null)
                    {
                        if (!dungeonData.Path.Contains(deadEndTile))
                        {
                            PlacePropGameObject(room, deadEndTile, propToPlace);
                        }
                    }
                    else
                    {
                        PlacePropGameObject(room, deadEndTile, propToPlace);
                    }
                }
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, deadEndTile, propToPlace, 1);
                }
            }
            else
            {
                chance = Mathf.Clamp01(chance + 0.1f);
            }
        }
    }

    private void PlaceCorridorProps(Room room, List<Prop> props)
    {
        float chance = corridorPropPlacementChance;

        foreach (Vector2Int corridorTile in room.CorridorTiles)
        {
            if (Random.value < chance)
            {
                Prop propToPlace = props[Random.Range(0, props.Count)];

                if (!room.PropPositions.Contains(corridorTile))
                {
                    if (dungeonData != null)
                    {
                        if (!dungeonData.Path.Contains(corridorTile))
                        {
                            PlacePropGameObject(room, corridorTile, propToPlace);
                        }
                    }
                    else
                    {
                        PlacePropGameObject(room, corridorTile, propToPlace);
                    }
                }
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, corridorTile, propToPlace, 1);
                }
            }
            else
            {
                chance = Mathf.Clamp01(chance + 0.1f);
            }
        }
    }

    private void PlaceGroupObject(Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {
        int count = Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        List<Vector2Int> availableSpaces = new List<Vector2Int>();
        for (int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tempPos = groupOriginPosition + new Vector2Int(xOffset, yOffset);
                if (room.FloorTiles.Contains(tempPos) &&
                    !dungeonData.Path.Contains(tempPos) &&
                    !room.PropPositions.Contains(tempPos))
                {
                    availableSpaces.Add(tempPos);
                }
            }
        }

        availableSpaces.OrderBy(x => Guid.NewGuid());

        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObject(room, availableSpaces[i], propToPlace);
        }

    }


    private GameObject PlacePropGameObject(Room room, Vector2Int placementPostion, Prop propToPlace)
    {
        GameObject prop = Instantiate(propPrefab);
        
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        Vector2Int Size = Vector2Int.one;

        if (propSpriteRenderer == null)
        {
            Debug.LogError("Prop prefab does not have a SpriteRenderer component." + prop.name);
            return null;
        }
        else
        {
            propSpriteRenderer.sprite = propToPlace.Sprite;
        }

        if(propToPlace.Corner == false && propToPlace.DeadEnd == false && propToPlace.Corridor == false)
        {
            Size = propToPlace.Size;
        }

        if (propToPlace.Collider)
        {
            CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
            collider.offset = Vector2.zero;

            if (Size.x > Size.y)
            {
                collider.direction = CapsuleDirection2D.Horizontal;
            }

            collider.size = new Vector2(Size.x * 0.8f, Size.y * 0.8f);
        }

        prop.transform.localPosition = (Vector2)placementPostion;
        prop.transform.parent = this.transform;
        propSpriteRenderer.transform.localPosition = (Vector2)Size * 0.5f;

        room.PropPositions.Add(placementPostion);
        room.PropObjects.Add(prop);
        return prop;
    }

    public void ProcessSingleRoom(RandomWalkRoomData data)
    {
        if (data == null)
            return;

        randomWalkData = data;

        RandomWalkDungeonRoom room = randomWalkData.Room;
        ExtractProps(room);

        PlaceRandomWalkRoomSpecialObjects(room);
    }

    #endregion

    #region SpecialPrefabs

    public void PlaceSpecialObjects()
    {
        if (dungeonData == null || dungeonData.Rooms.Count == 0) return;

        CalculateAccessiblePositionsForRooms();

        Room playerRoom = dungeonData.PlayerRoom;
        
        if(playerRoom.AvailablePositionsFromPath.Count > 0)
        {
            Vector2Int playerTile;
            if (playerRoom.AvailablePositionsFromPath.Contains(playerRoom.RoomCenterPos))
                playerTile = playerRoom.RoomCenterPos;
            else
            {
                playerRoom.AvailablePositionsFromPath.OrderBy(x => Guid.NewGuid()).ToList();
                playerTile = playerRoom.AvailablePositionsFromPath[0];
            }
            GameObject player = Instantiate(dungeonData.PlayerRoom.PlayerRoomParameters.PlayerPrefab);
            player.transform.localPosition = (Vector2)playerTile + Vector2.one * 0.5f;
        }

        foreach (Room room in dungeonData.Rooms)
        {
            if (room is BossRoom bossRoom)
            {
                if (room.AvailablePositionsFromPath.Count > 0)
                {
                    Vector2Int bossPosition;
                    if (room.AvailablePositionsFromPath.Contains(room.RoomCenterPos))
                        bossPosition = room.RoomCenterPos;
                    else
                    {
                        room.AvailablePositionsFromPath.OrderBy(x => Guid.NewGuid()).ToList();
                        bossPosition = room.AvailablePositionsFromPath[0];
                    }
                    GameObject boss = Instantiate(bossRoom.BossRoomParameters.BossPrefab);
                    boss.transform.localPosition = (Vector2)bossPosition + Vector2.one * 0.5f;
                    room.EnemiesInRoom.Add(boss);
                    boss.transform.parent = this.transform;
                }
            }
            else if (room is ChestRoom chestRoom)
            {
                if (room.AvailablePositionsFromPath.Count > 0)
                {
                    Vector2Int chestPosition;
                    if(room.AvailablePositionsFromPath.Contains(room.RoomCenterPos))
                        chestPosition = room.RoomCenterPos;
                    else
                    {
                        room.AvailablePositionsFromPath.OrderBy(x => Guid.NewGuid()).ToList();
                        chestPosition = room.AvailablePositionsFromPath[0];
                    }
                    GameObject chest = Instantiate(chestRoom.ChestRoomParameters.ChestPrefab);
                    chest.transform.localPosition = (Vector2)chestPosition + Vector2.one * 0.5f;
                    chest.transform.parent = this.transform;
                    room.SpecialItemsInRoom.Add(chest);
                }
            }
            else if (room is EnemyRoom enemyRoom)
            {
                int enemyCount = Random.Range(enemyRoom.EnemyRoomParameters.EnemyCountMin, enemyRoom.EnemyRoomParameters.EnemyCountMax);
                PlaceEnemies(enemyRoom, enemyCount);
            }
        }
        OnFinished?.Invoke();
    }

    private void PlaceEnemies(EnemyRoom room, int count)
    {
        List<Vector2Int> accessiblePositions = new List<Vector2Int>(room.AvailablePositionsFromPath);

        accessiblePositions = accessiblePositions.OrderBy(x => Guid.NewGuid()).ToList(); 
        count = Mathf.Min(count, accessiblePositions.Count);

        for (int i = 0; i < count; i++)
        {
            if (accessiblePositions.Count == 0) break;
            Vector2Int pos = accessiblePositions[Random.Range(0, accessiblePositions.Count)];
            GameObject enemy = Instantiate(room.EnemyRoomParameters.EnemyPrefabs[Random.Range(0, room.EnemyRoomParameters.EnemyPrefabs.Length)]);
            enemy.transform.localPosition = (Vector2)pos + Vector2.one * 0.5f;
            enemy.transform.parent = this.transform;
            room.EnemiesInRoom.Add(enemy);
            accessiblePositions.Remove(pos);
        }
    }

    private void PlaceRandomWalkRoomSpecialObjects(RandomWalkDungeonRoom room)
    {
        CalculateAccessiblePositionsForRooms();
        room.AvailablePositionsFromPath.OrderBy(x => Guid.NewGuid()).ToList();
        

        if (room.AvailablePositionsFromPath.Count > 0)
        {
            Vector2Int playerTile;
            if (room.AvailablePositionsFromPath.Contains(room.RoomCenterPos))
            {
                playerTile = room.RoomCenterPos;
            }
                
            else
            {
                playerTile = room.AvailablePositionsFromPath[0];
            }

            room.AvailablePositionsFromPath.Remove(playerTile);

            GameObject player = Instantiate(randomWalkData.Room.RandomWalkDungeonParameters.PlayerPrefab);
            player.transform.localPosition = (Vector2)playerTile + Vector2.one * 0.5f;


            Vector2Int chestTile = room.AvailablePositionsFromPath[Random.Range(0, room.AvailablePositionsFromPath.Count)];
            GameObject chest = Instantiate(randomWalkData.Room.RandomWalkDungeonParameters.ChestPrefab);
            chest.transform.localPosition = (Vector2)chestTile + Vector2.one * 0.5f;
            chest.transform.parent = this.transform;
            room.AvailablePositionsFromPath.Remove(chestTile);

            Vector2Int bossTile = room.AvailablePositionsFromPath[Random.Range(0, room.AvailablePositionsFromPath.Count)];
            GameObject boss = Instantiate(randomWalkData.Room.RandomWalkDungeonParameters.BossPrefab);
            boss.transform.localPosition = (Vector2)bossTile + Vector2.one * 0.5f;
            boss.transform.parent = this.transform;
            room.AvailablePositionsFromPath.Remove(bossTile);

            room.AvailablePositionsFromPath.OrderBy(x => Guid.NewGuid()).ToList();
            int count = room.RandomWalkDungeonParameters.EnemyCount;
            count = Mathf.Min(count, room.AvailablePositionsFromPath.Count);

            for (int i = 0; i < count; i++)
            {
                if (room.AvailablePositionsFromPath.Count == 0) break;
                Vector2Int pos = room.AvailablePositionsFromPath[Random.Range(0, room.AvailablePositionsFromPath.Count)];
                GameObject enemy = Instantiate(room.RandomWalkDungeonParameters.EnemyPrefabs[Random.Range(0, room.RandomWalkDungeonParameters.EnemyPrefabs.Length)]);
                enemy.transform.localPosition = (Vector2)pos + Vector2.one * 0.5f;
                enemy.transform.parent = this.transform;
                room.EnemiesInRoom.Add(enemy);
                room.AvailablePositionsFromPath.Remove(pos);
            }
        }

        OnFinished?.Invoke();

    }

    private void CalculateAccessiblePositionsForRooms()
    {
        if(dungeonData != null)
        {
            foreach (Room room in dungeonData.Rooms)
            {
                RoomGraph roomGraph = new RoomGraph(room.FloorTiles);
                HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
                roomFloor.IntersectWith(dungeonData.Path);
                if (roomFloor.Count > 0)
                {
                    Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);
                    room.AvailablePositionsFromPath = roomMap.Keys.ToList();
                }
                else
                {
                    Debug.LogWarning("No se encontró un tile de camino en la habitación: " + room.RoomCenterPos);
                    room.AvailablePositionsFromPath = new List<Vector2Int>();
                }
            }
        }
        else if(randomWalkData != null)
        {
            Room room = randomWalkData.Room;
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
            //roomFloor.IntersectWith(dungeonData.Path);
            if (roomFloor.Count > 0)
            {
                Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);
                room.AvailablePositionsFromPath = roomMap.Keys.ToList();
            }
            else
            {
                Debug.LogWarning("No se encontró un tile de camino en la habitación: " + room.RoomCenterPos);
                room.AvailablePositionsFromPath = new List<Vector2Int>();
            }
        }
    }

    #endregion
}