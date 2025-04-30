using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class WaveFunctionCollapseAlgorithm : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private bool progressive = true;
    [SerializeField] private int dimension = 25;

    [Header("Tiles")]
    public List<TileAlgorithm> AvailableTiles = new List<TileAlgorithm>();
    private TileAlgorithm[] allTiles;
    private CellAlgorithm[,] grid;

    private int Iteration;
    private int MaxIteration;

    private bool IsGenerating;

    private void Awake()
    {
        IsGenerating = false;
    }

    public void StartGeneration()
    {
        Debug.Log("Comenzando iteracion");
        Init();
    }

    void Update()
    {
        if (IsGenerating)
        {
            Wave();
        }
    }

    void EditorUpdate()
    {
        if (IsGenerating)
        {
            Wave();
        }
    }


    private void InitializeGrid()
    {
        List<TileAlgorithm> tileList = AvailableTiles.Where(t => t != null).ToList();
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

    public void Init()
    {
        int seed = Random.Range(1, 2000000);
        Random.InitState(seed);
        EditorApplication.update += EditorUpdate;

        Iteration = 0;
        MaxIteration = dimension * dimension;
        ClearGridObjects();
        IsGenerating = true;

        InitializeGrid();


        CellAlgorithm initialCell = grid[0, 0];
        Collapse(initialCell);
    }


    private void Wave()
    {
        if (Iteration < MaxIteration)
        {
            Iteration++;

            if (progressive)
            {
                InstantiateCollapsedCells();
            }

            CellAlgorithm nextCell = FindLowestEntropy();

            if (nextCell != null)
            {
                Collapse(nextCell);
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
    }

    private void InstantiateCollapsedCells()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                CellAlgorithm cell = grid[row, col];
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
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                CellAlgorithm cell = grid[row, col];
                if (cell.collapsed)
                {
                    Instantiate(cell.selectedTile, new Vector3(col, row, 0f), Quaternion.identity, transform);
                    cell.instantiated = true;
                }
            }
        }
    }

    private CellAlgorithm FindLowestEntropy()
    {
        CellAlgorithm cellWithLowestEntropy = null;
        float lowestEntropy = float.PositiveInfinity;

        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                CellAlgorithm cell = grid[row, col];
                if (!cell.collapsed)
                {
                    float entropy = CalculateEntropy(cell.tileOptions);
                    if (entropy < lowestEntropy)
                    {
                        lowestEntropy = entropy;
                        cellWithLowestEntropy = cell;
                    }
                }
            }
        }

        return cellWithLowestEntropy;
    }

    private float CalculateEntropy(TileAlgorithm[] tiles)
    {
        int sumOfWeights = tiles.Sum(t => t.Weight);
        float logSumOfWeights = tiles.Sum(t => (t.Weight) * math.log(t.Weight));
        return math.log(sumOfWeights) - (logSumOfWeights / sumOfWeights);
    }


    private void Collapse(CellAlgorithm cell)
    {
        if (cell.tileOptions.Length == 0)
        {
            Debug.LogError($"No possible tiles for cell at [{cell.row}, {cell.col}]");
            IsGenerating = false;
            return;
        }

        TileAlgorithm selectedTile = SelectTile(cell.tileOptions);
        cell.selectedTile = selectedTile;
        cell.tileOptions = new[] { selectedTile };
        cell.collapsed = true;

        Propagate(cell);
    }

    private TileAlgorithm SelectTile(TileAlgorithm[] tiles)
    {
        if (tiles.Length == 1)
            return tiles[0];

        float acumulative = 0;
        int totalWeight = tiles.Sum(t => t.Weight);
        int random = Random.Range(0, totalWeight);
        
        foreach (var tile in tiles)
        {
            acumulative += tile.Weight;
            if (random <= acumulative)
                return tile;
        }

        return tiles.OrderByDescending(t => t.Weight).First();
    }

    private void Propagate(CellAlgorithm cell)
    {
        CellAlgorithm cellLeft = GetCellLeft(cell.row, cell.col);
        CellAlgorithm cellRight = GetCellRight(cell.row, cell.col);
        CellAlgorithm cellTop = GetCellTop(cell.row, cell.col);
        CellAlgorithm cellBottom = GetCellBottom(cell.row, cell.col);
        

        if (cellTop != null && !cellTop.collapsed)
        {
            int tilesLength = cellTop.tileOptions.Length;

            List<TileAlgorithm> tilesSameSocket = new List<TileAlgorithm>();

            foreach (var t in cellTop.tileOptions)
            {
                bool sameSocket = false;
                foreach (var n in cell.tileOptions)
                {
                    if (n.UpSocketID == t.DownSocketID)
                    {
                        sameSocket = true;
                        break; 
                    }
                }
                
                if (sameSocket)
                {
                    tilesSameSocket.Add(t);
                }
            }

            cellTop.tileOptions = tilesSameSocket.ToArray();

            if (cellTop.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for top neighbor at [{cellTop.row}, {cellTop.col}]");
                IsGenerating = false;
            }
            else if (cellTop.tileOptions.Length == 1)
            {
                Collapse(cellTop);
            }
            else if (tilesLength > cellTop.tileOptions.Length)
            {
                Propagate(cellTop);
            }
        }

        if (cellBottom != null && !cellBottom.collapsed)
        {
            int tilesLength = cellBottom.tileOptions.Length;

            List<TileAlgorithm> tilesSameSocket = new List<TileAlgorithm>();

            foreach (var t in cellBottom.tileOptions)
            {
                bool sameSocket = false;
                foreach (var n in cell.tileOptions)
                {
                    if (n.DownSocketID == t.UpSocketID)
                    {
                        sameSocket = true;
                        break; 
                    }
                }
                
                if (sameSocket)
                {
                    tilesSameSocket.Add(t);
                }
            }

            cellBottom.tileOptions = tilesSameSocket.ToArray();

            if (cellBottom.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for bottom neighbor at [{cellBottom.row}, {cellBottom.col}]");
                IsGenerating = false;
            }
            else if (cellBottom.tileOptions.Length == 1)
            {
                Collapse(cellBottom);
            }
            else if (tilesLength > cellBottom.tileOptions.Length)
            {
                Propagate(cellBottom);
            }
        }

        if (cellLeft != null && !cellLeft.collapsed)
        {
            int tilesLength = cellLeft.tileOptions.Length;

            List<TileAlgorithm> tilesSameSocket = new List<TileAlgorithm>();

            foreach (var t in cellLeft.tileOptions)
            {
                bool sameSocket = false;
                foreach (var n in cell.tileOptions)
                {
                    if (n.LeftSocketID == t.RightSocketID)
                    {
                        sameSocket = true;
                        break; 
                    }
                }
                
                if (sameSocket)
                {
                    tilesSameSocket.Add(t);
                }
            }

            cellLeft.tileOptions = tilesSameSocket.ToArray();
            
            if (cellLeft.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for left neighbor at [{cellLeft.row}, {cellLeft.col}]");
                IsGenerating = false;
            }
            else if (cellLeft.tileOptions.Length == 1)
            {
                Collapse(cellLeft);
            }
            else if (tilesLength > cellLeft.tileOptions.Length)
            {
                Propagate(cellLeft);
            }
        }

        if (cellRight != null && !cellRight.collapsed)
        {
            int tilesLength = cellRight.tileOptions.Length;

            List<TileAlgorithm> tilesSameSocket = new List<TileAlgorithm>();

            foreach (var t in cellRight.tileOptions)
            {
                bool sameSocket = false;
                foreach (var n in cell.tileOptions)
                {
                    if (n.RightSocketID == t.LeftSocketID)
                    {
                        sameSocket = true;
                        break; 
                    }
                }
                
                if (sameSocket)
                {
                    tilesSameSocket.Add(t);
                }
            }

            cellRight.tileOptions = tilesSameSocket.ToArray();

            if (cellRight.tileOptions.Length == 0)
            {
                Debug.LogError($"No possible tiles left for right neighbor at [{cellRight.row}, {cellRight.col}]");
                IsGenerating = false;
            }
            else if (cellRight.tileOptions.Length == 1)
            {
                Collapse(cellRight);
            }
            else if (tilesLength > cellRight.tileOptions.Length)
            {
                Propagate(cellRight);
            }
        }
    }

    private CellAlgorithm GetCellTop(int row, int col)
    {
        if (row == dimension - 1)
            return null;
        return grid[row + 1, col];
    }

    private CellAlgorithm GetCellBottom(int row, int col)
    {
        if (row == 0)
            return null;
        return grid[row - 1, col];
    }

    private CellAlgorithm GetCellLeft(int row, int col)
    {
        if (col == 0)
            return null;
        return grid[row, col - 1];
    }

    private CellAlgorithm GetCellRight(int row, int col)
    {
        if (col == dimension - 1)
            return null;
        return grid[row, col + 1];
    }

    private void ClearGridObjects()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }
}