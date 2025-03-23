using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public int dimensions;
    public TileGrid[] tileObjects;
    public List<CellGrid> gridComponents;
    public CellGrid cellObj;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<CellGrid>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                CellGrid newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<CellGrid> tempGrid = new List<CellGrid>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<CellGrid> tempGrid)
    {
        if (tempGrid.Count == 0)
        {
            Debug.LogError("tempGrid está vacío. No hay celdas para colapsar.");
            return;
        }
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        CellGrid cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        int index = UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length);
        
        
        Debug.Log("Error en: " + randIndex + " " + index);
        if (cellToCollapse.tileOptions.Length == 0)
        {
            Debug.LogError("cellToCollapse.tileOptions está vacío. No hay opciones de tiles para colapsar.");
            return;
        }
        TileGrid selectedTile = cellToCollapse.tileOptions[index];
        cellToCollapse.tileOptions = new TileGrid[] { selectedTile };

        TileGrid foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<CellGrid> newGenerationCell = new List<CellGrid>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<TileGrid> options = new List<TileGrid>();
                    foreach (TileGrid t in tileObjects)
                    {
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        CellGrid up = gridComponents[x + (y - 1) * dimensions];
                        List<TileGrid> validOptions = new List<TileGrid>();

                        foreach (TileGrid possibleOptions in up.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].upNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < dimensions - 1)
                    {
                        CellGrid right = gridComponents[x + 1 + y * dimensions];
                        List<TileGrid> validOptions = new List<TileGrid>();

                        foreach (TileGrid possibleOptions in right.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < dimensions - 1)
                    {
                        CellGrid down = gridComponents[x + (y + 1) * dimensions];
                        List<TileGrid> validOptions = new List<TileGrid>();

                        foreach (TileGrid possibleOptions in down.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        CellGrid left = gridComponents[x - 1 + y * dimensions];
                        List<TileGrid> validOptions = new List<TileGrid>();

                        foreach (TileGrid possibleOptions in left.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    TileGrid[] newTileList = new TileGrid[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        if(iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    void CheckValidity(List<TileGrid> optionList, List<TileGrid> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
