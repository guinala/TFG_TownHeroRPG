using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomContentGenerator : MonoBehaviour
{
    [SerializeField]
    private RoomGenerator playerRoom, defaultRoom, chestRoom;
    
    [SerializeField] private Transform playerSpawnPointScene;
    
    List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    private GraphTest graphTest;

    public Transform itemParent;

    public UnityEvent RegenerateDungeon;
    
    [SerializeField]
    private CinemachineVirtualCamera cinemachineCamera;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in spawnedObjects)
            {
                Destroy(item);
            }
            RegenerateDungeon?.Invoke();
        }
    }
    public void GenerateRoomContent(DungeonData dungeonData)
    {
        foreach (GameObject item in spawnedObjects)
        {
            DestroyImmediate(item);
        }
        spawnedObjects.Clear();

        SelectPlayerSpawnPoint(dungeonData);
        //Prueba por ahora
        SelectChestSpawnPoints(dungeonData);
        SelectEnemySpawnPoints(dungeonData);
        

        foreach (GameObject item in spawnedObjects)
        {
            if(item != null)
                item.transform.SetParent(itemParent, false);
        }
    }

    private void SelectChestSpawnPoints(DungeonData dungeonData)
    {
        // Calcular el índice intermedio
        int midRoomIndex = dungeonData.roomsDictionary.Count % 2 == 0 
            ? (dungeonData.roomsDictionary.Count / 2) - 1 
            : dungeonData.roomsDictionary.Count / 2;

        // Obtener la posición de la habitación en el índice intermedio
        Vector2Int chestSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(midRoomIndex);

        // Configurar el punto de spawn del cofre en la posición obtenida
        playerSpawnPointScene.position = new Vector3(chestSpawnPoint.x, chestSpawnPoint.y, playerSpawnPointScene.position.z);

        // Usar la habitación en el índice intermedio para colocar el contenido del cofre
        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(midRoomIndex);
        
        List<GameObject> placedPrefabs = chestRoom.ProcessRoom(
            chestSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(midRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
        );
        

        spawnedObjects.AddRange(placedPrefabs);

        // Eliminar la habitación seleccionada del diccionario
        dungeonData.roomsDictionary.Remove(chestSpawnPoint);

        Debug.Log("Dungeon data tiene un diccionario con: " + dungeonData.roomsDictionary.Count + " elementos");
        //var ultimoElemento = dungeonData.roomsDictionary.Last();
        Debug.Log($"Elgida clave: {midRoomIndex}");
    }

    private void SelectPlayerSpawnPoint(DungeonData dungeonData)
    {
        int randomRoomIndex = UnityEngine.Random.Range(0, dungeonData.roomsDictionary.Count);
        Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        playerSpawnPointScene.position = new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, playerSpawnPointScene.position.z);

        //graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

        Dictionary<Vector2Int, int> dijkstraResult = graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);
        
        Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

        List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
            playerSpawnPoint,
            dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
            dungeonData.GetRoomFloorWithoutCorridors(roomIndex)
            );

        FocusCameraOnThePlayer(placedPrefabs[placedPrefabs.Count - 1].transform);

        spawnedObjects.AddRange(placedPrefabs);

        dungeonData.roomsDictionary.Remove(playerSpawnPoint);

        Debug.Log("Dungeon data tiene un diccionario con: " + dungeonData.roomsDictionary.Count + " elementos");
        // Obtén el último par clave-valor
        var ultimoElemento = dungeonData.roomsDictionary.Last();
        Debug.Log($"Última clave: {ultimoElemento.Key}, Último valor: {ultimoElemento.Value}");
    }

    
    private void FocusCameraOnThePlayer(Transform playerTransform)
    {
        cinemachineCamera.LookAt = playerTransform;
        cinemachineCamera.Follow = playerTransform;
    }
    

    private void SelectEnemySpawnPoints(DungeonData dungeonData)
    {
        foreach (KeyValuePair<Vector2Int,HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
        { 
            spawnedObjects.AddRange(
                defaultRoom.ProcessRoom(
                    roomData.Key,
                    roomData.Value, 
                    dungeonData.GetRoomFloorWithoutCorridors(roomData.Key)
                    )
            );

        }
    }

}
