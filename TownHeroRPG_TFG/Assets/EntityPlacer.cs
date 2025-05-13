using UnityEngine;
using System.Collections.Generic;

public class EntityPlacer : MonoBehaviour
{
    public void PlaceEntities(CellAlgorithm[,] grid, List<Entity> entities, int dimension)
    {
        foreach (var entity in entities)
        {
            List<(int, int)> validPositions = new List<(int, int)>();

            // Encontrar todas las posiciones v�lidas
            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    if (IsValidPosition(row, col, entity, grid, dimension))
                    {
                        validPositions.Add((row, col));
                    }
                }
            }

            // Determinar cu�ntas instancias colocar
            int instancesToPlace = Mathf.Min(entity.maxInstances, validPositions.Count);

            // Colocar las instancias en posiciones aleatorias v�lidas
            for (int i = 0; i < instancesToPlace; i++)
            {
                if (validPositions.Count == 0) break;

                int randomIndex = Random.Range(0, validPositions.Count);
                var (selectedRow, selectedCol) = validPositions[randomIndex];
                Vector3 position = new Vector3(selectedCol, selectedRow, 0f); // Ajusta seg�n tu sistema
                InstantiateEntity(entity, position);

                // Remover la posici�n para evitar superposici�n
                validPositions.RemoveAt(randomIndex);
            }

            if (instancesToPlace < entity.maxInstances)
            {
                Debug.LogWarning($"Solo se pudieron colocar {instancesToPlace} de {entity.maxInstances} instancias para la entidad con sockets [{entity.upSocket},{entity.downSocket},{entity.leftSocket},{entity.rightSocket}]");
            }
        }
    }

    private bool IsValidPosition(int row, int col, Entity entity, CellAlgorithm[,] grid, int dimension)
    {
        // L�gica para verificar si la posici�n es v�lida seg�n los sockets (implementaci�n asumida)
        return true; // Reemplaza con tu l�gica real
    }

    private void InstantiateEntity(Entity entity, Vector3 position)
    {
        // Instanciar el prefab en la posici�n dada
        Instantiate(entity.prefab, position, Quaternion.identity);
    }
}

[System.Serializable]
public class Entity
{
    public GameObject prefab; // Prefab a instanciar
    public int upSocket;      // Socket requerido para el tile superior
    public int downSocket;    // Socket requerido para el tile inferior
    public int leftSocket;    // Socket requerido para el tile a la izquierda
    public int rightSocket;   // Socket requerido para el tile a la derecha
    public int maxInstances;  // N�mero m�ximo de veces que se puede instanciar esta entidad
}