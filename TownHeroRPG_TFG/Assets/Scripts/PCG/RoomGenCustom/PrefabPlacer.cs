using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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

    public void ProcessRooms(DungeonData data)
    {
        if (data == null)
            return;

        dungeonData = data;
        foreach (Room room in dungeonData.Rooms)
        {
            List<Prop> propsToPlace = parameters.Props; // Props genéricos por defecto

            if (room is BossRoom bossRoom)
            {
                propsToPlace = bossRoom.BossRoomParameters.Props; // Props exclusivos de BossRoom
            }
            else if (room is ChestRoom chestRoom)
            {
                propsToPlace = chestRoom.ChestRoomParameters.Props; // Props exclusivos de ChestRoom
                Vector2Int center = Vector2Int.RoundToInt(room.RoomCenterPos);
            }
            else if (room is PlayerRoom playerRoom)
            {
                propsToPlace = playerRoom.PlayerRoomParameters.Props; // Props exclusivos de PlayerRoom
            }

            List<Prop> deadEndProps = propsToPlace.Where(x => x.DeadEnd).ToList();
            if(deadEndProps.Count > 0)
                PlaceDeadEndProps(room, deadEndProps);

            //Place props place props in the corners
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            if(cornerProps.Count > 0)
                PlaceCornerProps(room, cornerProps);

            //Place props in the corridors
            List<Prop> corridorProps = propsToPlace.Where(x => x.Corridor).ToList();
            if(corridorProps.Count > 0)
                PlaceCorridorProps(room, corridorProps);

            //Place props near LEFT wall
            List<Prop> leftWallProps = propsToPlace
            .Where(x => x.NearLeftWall)
            .OrderByDescending(x => x.Size.x * x.Size.y)
            .ToList();

            if(leftWallProps.Count > 0)
                PlaceProps(room, leftWallProps, room.NearWallLeftTiles, PlacementOriginCorner.BottomLeft);

            //Place props near RIGHT wall
            List <Prop> rightWallProps = propsToPlace
            .Where(x => x.NearRightWall)
            .OrderByDescending(x => x.Size.x * x.Size.y)
            .ToList();

            if(rightWallProps.Count > 0)
                PlaceProps(room, rightWallProps, room.NearWallRightTiles, PlacementOriginCorner.TopRight);

            //Place props near UP wall
            List <Prop> topWallProps = propsToPlace
            .Where(x => x.NearUpWall)
            .OrderByDescending(x => x.Size.x * x.Size.y)
            .ToList();

            if (topWallProps.Count > 0)
                PlaceProps(room, topWallProps, room.NearWallUpTiles, PlacementOriginCorner.TopLeft);

            //Place props near DOWN wall
            List <Prop> downWallProps = propsToPlace
            .Where(x => x.NearDownWall)
            .OrderByDescending(x => x.Size.x * x.Size.y)
            .ToList();

            if(downWallProps.Count > 0)
                PlaceProps(room, downWallProps, room.NearWallDownTiles, PlacementOriginCorner.BottomLeft); 

            //Place inner props
            List <Prop> innerProps = propsToPlace
                .Where(x => x.Inner)
                .OrderByDescending(x => x.Size.x * x.Size.y)
                .ToList();
            if(innerProps.Count > 0)
                PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        //OnFinished?.Invoke();
        //Invoke("RunEvent", 1);
        PlaceSpecialObjects();

    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //Remove path positions from the initial nearWallTiles to ensure the clear path to traverse dungeon
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(dungeonData.Path);

        //We will try to place all the props
        foreach (Prop propToPlace in wallProps)
        {
            //We want to place only certain quantity of each prop
            int quantity = Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //remove taken positions
                tempPositons.ExceptWith(room.PropPositions);
                //shuffel the positions
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                //If placement has failed there is no point in trying to place the same prop again
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }

        }
    }

    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //try placing the objects starting from the corner specified by the placement parameter
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //select the specified position (but it can be already taken after placing the corner props as a group)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //check if there is enough space around to fit the prop
            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position, placement);

            //If we have enough spaces place the prop
            if (freePositionsAround.Count == propToPlace.Size.x * propToPlace.Size.y)
            {
                //Place the gameobject
                PlacePropGameObjectAt(room, position, propToPlace);
                //Lock all the positions recquired by the prop (based on its size)
                foreach (Vector2Int pos in freePositionsAround)
                {
                    //Hashest will ignore duplicate positions
                    room.PropPositions.Add(pos);
                }

                //Deal with groups
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> TryToFitProp(
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        //Perform the specific loop depending on the PlacementOriginCorner
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

    private void PlaceCornerProps(Room room, List<Prop> cornerProps)
    {
        float tempChance = cornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (Random.value < tempChance)
            {
                Prop propToPlace
                    = cornerProps[Random.Range(0, cornerProps.Count)];

                if (!room.PropPositions.Contains(cornerTile) && !dungeonData.Path.Contains(cornerTile))
                {
                    PlacePropGameObjectAt(room, cornerTile, propToPlace);
                }
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 1);
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    private void PlaceDeadEndProps(Room room, List<Prop> deadEndProps)
    {
        float tempChance = deadPropPlacementChance;

        foreach (Vector2Int deadEndTile in room.DeadEndTiles)
        {
            if (Random.value < tempChance)
            {
                Prop propToPlace
                    = deadEndProps[Random.Range(0, deadEndProps.Count)];

                if (!room.PropPositions.Contains(deadEndTile) && !dungeonData.Path.Contains(deadEndTile))
                {
                    PlacePropGameObjectAt(room, deadEndTile, propToPlace);
                }
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, deadEndTile, propToPlace, 1);
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    private void PlaceCorridorProps(Room room, List<Prop> corridorProps)
    {
        float tempChance = corridorPropPlacementChance;

        foreach (Vector2Int corridorTile in room.CorridorTiles)
        {
            if (Random.value < tempChance)
            {
                Prop propToPlace
                    = corridorProps[Random.Range(0, corridorProps.Count)];

                if (!room.PropPositions.Contains(corridorTile) && !dungeonData.Path.Contains(corridorTile))
                {
                    PlacePropGameObjectAt(room, corridorTile, propToPlace);
                }
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, corridorTile, propToPlace, 1);
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {
        //*Can work poorely when placing bigger props as groups

        //calculate how many elements are in the group -1 that we have placed in the center
        int count = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        //find the available spaces around the center point.
        //we use searchOffset to limit the distance between those points and the center point
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

        //shuffle the list
        availableSpaces.OrderBy(x => Guid.NewGuid());

        //place the props (as many as we want or if there is less space fill all the available spaces)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
        }

    }


    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, Prop propToPlace)
    {
        //Instantiat the prop at this positon
        GameObject prop = Instantiate(propPrefab);
        if (propToPlace.name == "Chest")
        {
            prop.gameObject.name = "Chest";
            Debug.Log("Se supone que se instancia el cofreasdasdasdasdasdasdas");
        }
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        if (propSpriteRenderer == null)
        {
            Debug.LogError("Prop prefab does not have a SpriteRenderer component." + prop.name);
            return null;
        }
        else
        {
            //set the sprite
            propSpriteRenderer.sprite = propToPlace.Sprite;
        }

        if (propToPlace.Collider)
        {
            CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
            collider.offset = Vector2.zero;

            if (propToPlace.Size.x > propToPlace.Size.y)
            {
                collider.direction = CapsuleDirection2D.Horizontal;
            }

            collider.size = new Vector2(propToPlace.Size.x * 0.8f, propToPlace.Size.y * 0.8f);
        }
        //collider.size = size;

        prop.transform.localPosition = (Vector2)placementPostion;
        //adjust the position to the sprite
        propSpriteRenderer.transform.localPosition
            = (Vector2)propToPlace.Size * 0.5f;

        //Save the prop in the room data (so in the dunbgeon data)
        room.PropPositions.Add(placementPostion);
        room.PropObjects.Add(prop);
        return prop;
    }

    #endregion

    #region SpecialPrefabs

    public void PlaceSpecialObjects()
    {
        if (dungeonData == null || dungeonData.Rooms.Count == 0) return;

        // Calcular posiciones accesibles para cada habitación
        CalculateAccessiblePositionsForRooms();

        Room playerRoom = dungeonData.PlayerRoom;
        
        if(playerRoom.AvailablePositionsFromPath.Count > 0)
        {
            Vector2Int playerTile = playerRoom.AvailablePositionsFromPath[0];
            GameObject player = Instantiate(dungeonData.PlayerRoom.PlayerRoomParameters.PlayerPrefab);
            player.transform.localPosition = (Vector2)playerTile + Vector2.one * 0.5f;
        }

        // Colocar jefe y enemigos
        foreach (Room room in dungeonData.Rooms)
        {
            if (room is BossRoom bossRoom)
            {
                // Colocar solo al jefe en la habitación del jefe
                if (room.AvailablePositionsFromPath.Count > 0)
                {
                    Vector2Int bossPosition = room.AvailablePositionsFromPath[0];
                    GameObject boss = Instantiate(bossRoom.BossRoomParameters.BossPrefab);
                    boss.transform.localPosition = (Vector2)bossPosition + Vector2.one * 0.5f;
                    room.EnemiesInRoom.Add(boss);
                }
            }
            else if (room is ChestRoom chestRoom)
            {
                // Colocar solo al jefe en la habitación del jefe
                if (room.AvailablePositionsFromPath.Count > 0)
                {
                    Vector2Int chestPosition = room.AvailablePositionsFromPath[0];
                    GameObject chest = Instantiate(chestRoom.ChestRoomParameters.ChestPrefab);
                    chest.transform.localPosition = (Vector2)chestPosition + Vector2.one * 0.5f;
                    room.SpecialItemsInRoom.Add(chest);
                }
            }
            else if (room is EnemyRoom enemyRoom)
            {
                // Colocar enemigos regulares en habitaciones no especiales (excluyendo la del jugador)
                int enemyCount = Random.Range(enemyRoom.EnemyRoomParameters.EnemyCountMin, enemyRoom.EnemyRoomParameters.EnemyCountMax); // Ajustar según sea necesario
                PlaceEnemies(enemyRoom, enemyCount);
            }
        }
    }

    private void PlaceEnemies(EnemyRoom room, int count)
    {
        List<Vector2Int> accessiblePositions = new List<Vector2Int>(room.AvailablePositionsFromPath);
        count = Mathf.Min(count, accessiblePositions.Count);

        for (int i = 0; i < count; i++)
        {
            if (accessiblePositions.Count == 0) break;
            Vector2Int pos = accessiblePositions[Random.Range(0, accessiblePositions.Count)];
            GameObject enemy = Instantiate(room.EnemyRoomParameters.EnemyPrefabs[Random.Range(0, room.EnemyRoomParameters.EnemyPrefabs.Length)]);
            enemy.transform.localPosition = (Vector2)pos + Vector2.one * 0.5f;
            room.EnemiesInRoom.Add(enemy);
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