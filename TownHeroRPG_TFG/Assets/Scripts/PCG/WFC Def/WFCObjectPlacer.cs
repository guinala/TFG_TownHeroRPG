using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class WFCObjectPlacer : MonoBehaviour
{
    [Header("Objects to Place")]
    public List<WFCObject> objectsToPlace;
    public UnityEvent OnObjectsPlaced;

    public List<WFCObjectInstance> PlaceObjects(CellAlgorithm[,] grid, int dimension) 
    {
        List<WFCObjectInstance> placedWfcObjects = new List<WFCObjectInstance>(); 

        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            WFCObject wfcObject = objectsToPlace[i]; 
            List<(int, int)> validPositions = new List<(int, int)>();

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    if (IsValidPosition(row, col, wfcObject, grid, dimension))
                    {
                        validPositions.Add((row, col));
                    }
                }
            }

            Debug.Log("Para el objeto " + wfcObject.Prefab.name + " hay " + validPositions.Count + " posiciones válidas para colocar instancias.");
            int instancesToPlace = Mathf.Min(wfcObject.maxInstances, validPositions.Count); 

            for (int j = 0; j < instancesToPlace; j++)
            {
                if (validPositions.Count == 0)
                {
                    Debug.Log("No hay más posiciones válidas para colocar el objeto " + wfcObject.Prefab.name);
                    break;
                }

                int randomIndex = Random.Range(0, validPositions.Count);
                var (selectedRow, selectedCol) = validPositions[randomIndex];
                Vector3 position = new Vector3(selectedCol, selectedRow, 0f);
                InstantiateEntity(wfcObject, position); 

                placedWfcObjects.Add(new WFCObjectInstance
                {
                    Index = i,
                    Position = position
                });

                validPositions.RemoveAt(randomIndex);
            }

            if (instancesToPlace < wfcObject.maxInstances) 
            {
                Debug.LogWarning($"Solo se pudieron colocar {instancesToPlace} de {wfcObject.maxInstances} instancias para el objeto con sockets [{wfcObject.UpSocketID},{wfcObject.DownSocketID},{wfcObject.LeftSocketID},{wfcObject.RightSocketID}]");
            }
        }

        return placedWfcObjects;
    }

    private bool IsValidPosition(int row, int col, WFCObject wfcObject, CellAlgorithm[,] grid, int dimension) 
    {
        CellAlgorithm currentCell = grid[row, col];

        bool valid =
        currentCell.selectedTile.UpSocketID == wfcObject.UpSocketID &&
        currentCell.selectedTile.DownSocketID == wfcObject.DownSocketID &&
        currentCell.selectedTile.LeftSocketID == wfcObject.LeftSocketID &&
        currentCell.selectedTile.RightSocketID == wfcObject.RightSocketID &&
        !currentCell.selectedTile.UseOnEdges;

        return valid;
    }

    private void InstantiateEntity(WFCObject entity, Vector3 position)
    {
        if (entity.Prefab != null)
        {
            Instantiate(entity.Prefab, position, Quaternion.identity, transform);
        }
        else
        {
            Debug.LogError("El prefab de la entidad no est� asignado.");
        }
    }

    public void InstantiateObjects(WFCObjectInstance[] entityInstances)
    {
        foreach (var instance in entityInstances)
        {
            if (instance.Index >= 0 && instance.Index < objectsToPlace.Count)
            {
                WFCObject entity = objectsToPlace[instance.Index];
                InstantiateEntity(entity, instance.Position);
            }
            else
            {
                Debug.LogWarning($"�ndice de entidad inv�lido: {instance.Index}");
            }
        }
    }
}