using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class AdvancedWFC : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private bool progressive = true;
    //[SerializeField] private bool showGrid;
    [SerializeField] private int dimension = 25;

    [Header("Tiles")]
    public TileAlgorithm BaseTile;
    public List<TileAlgorithm> AvailableTiles = new List<TileAlgorithm>();
    private TileAlgorithm[] allTiles;
    private CellAlgorithm[,] grid;
    
    private int Iteration;
    private int MaxIteration;

    private bool IsGenerating;

    // private void OnDrawGizmos()
    // {
    //     if (showGrid)
    //     {
    //         Gizmos.color = Color.green;
    //         for (int row = 0; row < dimension; row++)
    //         {
    //             for (int col = 0; col < dimension; col++)
    //             {
    //                 Gizmos.DrawWireCube(new Vector3(col + 0.5f, row + 0.5f, 0f), new Vector3(1f, 1f, 0.1f));
    //             }
    //         }
    //     }
    // }

    private void Awake()
    {
        IsGenerating = false;
    }

    void Update() {
        if (IsGenerating)
        {
            Debug.Log("cosazas");
            GenerateStepByStep();
        } 
        else
        {
            Debug.Log("No se esta generando");
        }
    }
    
    void EditorUpdate() {
        if(IsGenerating)
        {
            Debug.Log("cosazas");
            GenerateStepByStep();
        }
        else
        {
            Debug.Log("No se esta generando");
        }
    }
    

    // #if UNITY_EDITOR
    // private void EditorUpdate()
    // {
    //     EditorApplication.update += Update;
    // }
    // #endif

    private void InitializeGrid()
    {
        var tileList = AvailableTiles.Where(t => t != null).ToList();
        tileList.Add(BaseTile);
        allTiles = tileList.OrderBy(t => t.Weight).ToArray();

        grid = new CellAlgorithm[dimension, dimension];

        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                grid[row, col] = new CellAlgorithm
                {
                    row = row,
                    col = col,
                    collapsed = false,
                    tileOptions = (row == 0 || col == 0 || row == dimension - 1 || col == dimension - 1)
                        ? allTiles.Where(t => t.UseOnEdges).ToArray()
                        : allTiles
                };
            }
        }
    }

    public void GenerateMap()
    {
        int seed = Random.Range(1, 2000000);
        Random.InitState(seed);

        Iteration = 0;
        ClearGridObjects();

        InitializeGrid();
        IsGenerating = true;

        CellAlgorithm initialCell = grid[0, 0];
        CollapseCell(initialCell);

        EditorApplication.update += EditorUpdate;
        MaxIteration = dimension * dimension;
    }

    private void GenerateStepByStep()
    {
        if (IsGenerating && Iteration < MaxIteration)
        {
            PerformWaveFunctionStep();
            Iteration++;
        }
        else
        {
            Debug.Log("Generation complete");
            if (!progressive)
            {
               InstantiateAllCells();
            }
            EditorApplication.update -= EditorUpdate;
        }
    }

    private void PerformWaveFunctionStep()
    {
        if(progressive)
        {
            InstantiateCollapsedCells();
        }

        CellAlgorithm nextCell = FindCellWithLowestEntropy();
        if (nextCell != null)
        {
            CollapseCell(nextCell);
        }
        else
        {
            IsGenerating = false;
            Debug.Log("Generation complete");
            if (!progressive)
            {
                InstantiateAllCells();
            }
            EditorApplication.update -= EditorUpdate;
        }
    }

    private void InstantiateCollapsedCells()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                var cell = grid[row, col];
                if (cell.collapsed && !cell.instantiated)
                {
                    Instantiate(cell.selectedTile, new Vector3(col, row, 0f), Quaternion.identity, transform);
                    cell.instantiated = true;
                }
            }
        }
    }

    private void InstantiateAllCells()
    {
        Debug.Log("Se viene");
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                var cell = grid[row, col];
                if (cell.collapsed) //&& !cell.instantiated && cell.selectedTile != null)
                {
                    Instantiate(cell.selectedTile, new Vector3(col, row, 0f), Quaternion.identity, transform);
                    cell.instantiated = true;
                }
            }
        }
    }


    private void CollapseCell(CellAlgorithm cell)
    {
        if (cell.tileOptions.Length == 0)
        {
            Debug.LogError($"No possible tiles for cell at [{cell.row}, {cell.col}]");
            IsGenerating = false;
            return;
        }

        TileAlgorithm selectedTile = SelectTileBasedOnWeight(cell.tileOptions);
        cell.selectedTile = selectedTile;
        cell.tileOptions = new[] { selectedTile };
        cell.collapsed = true;

        FilterPossibleOptions(cell);
    }

    private TileAlgorithm SelectTileBasedOnWeight(TileAlgorithm[] possibleTiles)
    {
        if (possibleTiles.Length == 1)
            return possibleTiles[0];

        var totalWeight = possibleTiles.Sum(t => t.Weight);
        var randomValue = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var tile in possibleTiles)
        {
            cumulative += tile.Weight;
            if (randomValue <= cumulative)
                return tile;
        }

        return possibleTiles.OrderByDescending(t => t.Weight).First();
    }

    private void FilterPossibleOptions(CellAlgorithm cell)
    {
        var topCell = GetTopCell(cell.row, cell.col);
        var bottomCell = GetBottomCell(cell.row, cell.col);
        var leftCell = GetLeftCell(cell.row, cell.col);
        var rightCell = GetRightCell(cell.row, cell.col);

        if (topCell != null && !topCell.collapsed)
        {
            var possibleTilesLength = topCell.tileOptions.Length;
            topCell.tileOptions = topCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.UpSocketID == t.DownSocketID)).ToArray();

            if (topCell.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for top neighbor at [{topCell.row}, {topCell.col}]");
                IsGenerating = false;
            }
            else if (topCell.tileOptions.Length == 1)
            {
                CollapseCell(topCell);
            }
            else if (possibleTilesLength > topCell.tileOptions.Length)
            {
                FilterPossibleOptions(topCell);
            }
        }

        if (bottomCell != null && !bottomCell.collapsed)
        {
            var possibleTilesLength = bottomCell.tileOptions.Length;
            bottomCell.tileOptions = bottomCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.DownSocketID == t.UpSocketID)).ToArray();

            if (bottomCell.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for bottom neighbor at [{bottomCell.row}, {bottomCell.col}]");
                IsGenerating = false;
            }
            else if (bottomCell.tileOptions.Length == 1)
            {
                CollapseCell(bottomCell);
            }
            else if (possibleTilesLength > bottomCell.tileOptions.Length)
            {
                FilterPossibleOptions(bottomCell);
            }
        }

        if (leftCell != null && !leftCell.collapsed)
        {
            var possibleTilesLength = leftCell.tileOptions.Length;
            leftCell.tileOptions = leftCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.LeftSocketID == t.RightSocketID)).ToArray();

            if (leftCell.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for left neighbor at [{leftCell.row}, {leftCell.col}]");
                IsGenerating = false;
            }
            else if (leftCell.tileOptions.Length == 1)
            {
                CollapseCell(leftCell);
            }
            else if (possibleTilesLength > leftCell.tileOptions.Length)
            {
                FilterPossibleOptions(leftCell);
            }
        }

        if (rightCell != null && !rightCell.collapsed)
        {
            var possibleTilesLength = rightCell.tileOptions.Length;
            rightCell.tileOptions = rightCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.RightSocketID == t.LeftSocketID)).ToArray();

            if (rightCell.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for right neighbor at [{rightCell.row}, {rightCell.col}]");
                IsGenerating = false;
            }
            else if (rightCell.tileOptions.Length == 1)
            {
                CollapseCell(rightCell);
            }
            else if (possibleTilesLength > rightCell.tileOptions.Length)
            {
                FilterPossibleOptions(rightCell);
            }
        }
    }

    private CellAlgorithm GetTopCell(int row, int col)
    {
        if (row == dimension - 1)
            return null;
        return grid[row + 1, col];
    }

    private CellAlgorithm GetBottomCell(int row, int col)
    {
        if (row == 0)
            return null;
        return grid[row - 1, col];
    }

    private CellAlgorithm GetLeftCell(int row, int col)
    {
        if (col == 0)
            return null;
        return grid[row, col - 1];
    }

    private CellAlgorithm GetRightCell(int row, int col)
    {
        if (col == dimension - 1)
            return null;
        return grid[row, col + 1];
    }

    private CellAlgorithm FindCellWithLowestEntropy()
    {
        CellAlgorithm cellWithLowestEntropy = null;
        float lowestEntropy = float.MaxValue;

        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                var cell = grid[row, col];
                if (cell.collapsed) continue;

                var entropy = CalculateEntropy(cell.tileOptions);
                if (entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;
                    cellWithLowestEntropy = cell;
                }
            }
        }

        return cellWithLowestEntropy;
    }

    private float CalculateEntropy(TileAlgorithm[] possibleTiles)
    {
        // var weightSum = possibleTiles.Sum(t => 1f / t.Weight);
        // var weightSumLog = possibleTiles.Sum(t => (1f / t.Weight) * math.log(1f / t.Weight));
        var weightSum = possibleTiles.Sum(t => t.Weight);
        var weightSumLog = possibleTiles.Sum(t => (t.Weight) * math.log(t.Weight));
        return math.log(weightSum) - (weightSumLog / weightSum);
    }

    private void ClearGridObjects()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }

    [ContextMenu("Generate Map")]
    public void StartGeneration()
    {
        Debug.Log("Comenzando iteraci�n");
        GenerateMap();
    }
}