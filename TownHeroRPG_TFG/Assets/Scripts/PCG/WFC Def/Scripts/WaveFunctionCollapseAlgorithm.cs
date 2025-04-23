using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class WaveFunctionCollapseAlgorithm : MonoBehaviour
{
    [SerializeField] private bool showGrid;

    [Header("Grid config"), SerializeField] private int dimension = 4;

    private TileAlgorithm[] tiles;
    private CellAlgorithm[,] cells;

    public TileAlgorithm groundTile;

    [Header("Tiles")]
    public List<TileAlgorithm> AvailableTiles = new List<TileAlgorithm>();

    [HideInInspector] public bool Generating;
    [HideInInspector] public int UsedSeed;
    [HideInInspector] public int IterationNumber;
    [HideInInspector] public int MaxIterationNumber;
    [HideInInspector] public double IterationPerSecond;
    [HideInInspector] public double ElapsedTime;

    private double executionStartup;

    private void Start()
    {
        Generate();
    }

    private void OnDrawGizmos()
    {
        if (showGrid)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    Gizmos.DrawWireCube(new Vector3(j + 0.5f, i + 0.5f, 0f), new Vector3(1f, 1f, 0.1f));
                }
            }
        }
    }

    private void Awake()
    {
        Generating = false;
        // TilePatternList = new List<TilePattern>();
    }

    void EditorUpdate()
    {
        if (Generating && IterationNumber < MaxIterationNumber)
        {
            Debug.Log("Hola");
            WaveFunction();
            //ElapsedTime = EditorApplication.timeSinceStartup - executionStartup;
            //IterationPerSecond = IterationNumber / ElapsedTime;
        }
    }

    void Update()
    {
        if (Generating && IterationNumber < MaxIterationNumber)
        {
            Debug.Log("Hola");
            WaveFunction();
            //ElapsedTime = EditorApplication.timeSinceStartup - executionStartup;
            //IterationPerSecond = IterationNumber / ElapsedTime;
        }
    }

    void InitCells()
    {
        //ConfigTiles();

        var tilesList = AvailableTiles.Where(t => t != null).ToList();
        tilesList.Add(groundTile);
        //Debug.Log(BaseTile + ", " + tilesList.Count);

        tiles = tilesList.OrderBy(t => t.Weight).ToArray();


        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i, j] = new CellAlgorithm
                {
                    row = i,
                    col = j,
                    collapsed = false
                };
                //Debug.Log("hecho");

                if (i == 0 || j == 0 || i == dimension - 1 || j == dimension - 1)
                {
                    cells[i, j].tileOptions = tiles.Where(t => t.UseOnEdges).ToArray();
                }
                else
                {
                    cells[i, j].tileOptions = tiles;
                }
            }
        }
    }

    // private void ConfigTiles()
    // {
    //     var iteraction = 0;
    //
    //     var baseTileNumber = 0;
    //
    //     BaseTile.UpSocket = baseTileNumber;
    //     BaseTile.RightSocket = baseTileNumber;
    //     BaseTile.DownSocket = baseTileNumber;
    //     BaseTile.LeftSocket = baseTileNumber;
    //
    //     foreach (var pattern in TilePatternList)
    //     {
    //         var baseNumber = (iteraction + 1) * 6;
    //         SetBaseOutTileSockets(baseTileNumber, pattern, baseNumber);
    //
    //         if (pattern.EnableInsideOut)
    //         {
    //             SetBaseInTileSockets(baseTileNumber + baseNumber, pattern, baseNumber);
    //         }
    //
    //         iteraction++;
    //     }
    // }
    //
    // private void SetBaseOutTileSockets(int baseTileNumber, TilePattern pattern, int baseNumber)
    // {
    //     pattern.BaseOutTopLeft.UpSocket = baseTileNumber;
    //     pattern.BaseOutTopLeft.RightSocket = baseNumber + 1;
    //     pattern.BaseOutTopLeft.DownSocket = baseNumber + 3;
    //     pattern.BaseOutTopLeft.LeftSocket = baseTileNumber;
    //
    //     pattern.BaseOutTop.UpSocket = baseTileNumber;
    //     pattern.BaseOutTop.RightSocket = baseNumber + 1;
    //     pattern.BaseOutTop.DownSocket = baseNumber + 5;
    //     pattern.BaseOutTop.LeftSocket = baseNumber + 1;
    //
    //     pattern.BaseOutTopRight.UpSocket = baseTileNumber;
    //     pattern.BaseOutTopRight.RightSocket = baseTileNumber;
    //     pattern.BaseOutTopRight.DownSocket = baseNumber + 4;
    //     pattern.BaseOutTopRight.LeftSocket = baseNumber + 1;
    //
    //     pattern.BaseOutCenterLeft.UpSocket = baseNumber + 3;
    //     pattern.BaseOutCenterLeft.RightSocket = baseNumber + 5;
    //     pattern.BaseOutCenterLeft.DownSocket = baseNumber + 3;
    //     pattern.BaseOutCenterLeft.LeftSocket = baseTileNumber;
    //
    //     pattern.BaseOutCenter.UpSocket = baseNumber + 5;
    //     pattern.BaseOutCenter.RightSocket = baseNumber + 5;
    //     pattern.BaseOutCenter.DownSocket = baseNumber + 5;
    //     pattern.BaseOutCenter.LeftSocket = baseNumber + 5;
    //
    //     pattern.BaseOutCenterRight.UpSocket = baseNumber + 4;
    //     pattern.BaseOutCenterRight.RightSocket = baseTileNumber;
    //     pattern.BaseOutCenterRight.DownSocket = baseNumber + 4;
    //     pattern.BaseOutCenterRight.LeftSocket = baseNumber + 5;
    //
    //     pattern.BaseOutBottomLeft.UpSocket = baseNumber + 3;
    //     pattern.BaseOutBottomLeft.RightSocket = baseNumber + 2;
    //     pattern.BaseOutBottomLeft.DownSocket = baseTileNumber;
    //     pattern.BaseOutBottomLeft.LeftSocket = baseTileNumber;
    //
    //     pattern.BaseOutBottom.UpSocket = baseNumber + 5;
    //     pattern.BaseOutBottom.RightSocket = baseNumber + 2;
    //     pattern.BaseOutBottom.DownSocket = baseTileNumber;
    //     pattern.BaseOutBottom.LeftSocket = baseNumber + 2;
    //
    //     pattern.BaseOutBottomRight.UpSocket = baseNumber + 4;
    //     pattern.BaseOutBottomRight.RightSocket = baseTileNumber;
    //     pattern.BaseOutBottomRight.DownSocket = baseTileNumber;
    //     pattern.BaseOutBottomRight.LeftSocket = baseNumber + 2;
    // }
    //
    // private void SetBaseInTileSockets(int baseTileNumber, TilePattern pattern, int baseNumber)
    // {
    //     pattern.BaseInTopLeft.UpSocket = baseTileNumber + 5;
    //     pattern.BaseInTopLeft.RightSocket = baseNumber + 2;
    //     pattern.BaseInTopLeft.DownSocket = baseNumber + 4;
    //     pattern.BaseInTopLeft.LeftSocket = baseTileNumber + 5;
    //
    //     pattern.BaseInTop.UpSocket = baseTileNumber + 5;
    //     pattern.BaseInTop.RightSocket = baseNumber + 2;
    //     pattern.BaseInTop.DownSocket = baseTileNumber - baseNumber;
    //     pattern.BaseInTop.LeftSocket = baseNumber + 2;
    //
    //     pattern.BaseInTopRight.UpSocket = baseTileNumber + 5;
    //     pattern.BaseInTopRight.RightSocket = baseTileNumber + 5;
    //     pattern.BaseInTopRight.DownSocket = baseNumber + 3;
    //     pattern.BaseInTopRight.LeftSocket = baseNumber + 2;
    //
    //     pattern.BaseInCenterLeft.UpSocket = baseNumber + 4;
    //     pattern.BaseInCenterLeft.RightSocket = baseTileNumber - baseNumber;
    //     pattern.BaseInCenterLeft.DownSocket = baseNumber + 4;
    //     pattern.BaseInCenterLeft.LeftSocket = baseTileNumber + 5;
    //
    //     pattern.BaseInCenterRight.UpSocket = baseNumber + 3;
    //     pattern.BaseInCenterRight.RightSocket = baseTileNumber + 5;
    //     pattern.BaseInCenterRight.DownSocket = baseNumber + 3;
    //     pattern.BaseInCenterRight.LeftSocket = baseTileNumber - baseNumber;
    //
    //     pattern.BaseInBottomLeft.UpSocket = baseNumber + 4;
    //     pattern.BaseInBottomLeft.RightSocket = baseNumber + 2;
    //     pattern.BaseInBottomLeft.DownSocket = baseTileNumber + 5;
    //     pattern.BaseInBottomLeft.LeftSocket = baseTileNumber + 5;
    //
    //     pattern.BaseInBottom.UpSocket = baseTileNumber - baseNumber;
    //     pattern.BaseInBottom.RightSocket = baseNumber + 1;
    //     pattern.BaseInBottom.DownSocket = baseTileNumber + 5;
    //     pattern.BaseInBottom.LeftSocket = baseNumber + 1;
    //
    //     pattern.BaseInBottomRight.UpSocket = baseNumber + 3;
    //     pattern.BaseInBottomRight.RightSocket = baseTileNumber + 5;
    //     pattern.BaseInBottomRight.DownSocket = baseTileNumber + 5;
    //     pattern.BaseInBottomRight.LeftSocket = baseNumber + 1;
    // }

    private void WaveFunction()
    {
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                var currentCell = cells[i, j];

                if (currentCell.collapsed)
                {
                    // Instantiate the collapsed cell
                    if (!currentCell.instantiated)
                    {
                        Instantiate(currentCell.selectedTile, new Vector3(j, i, 0f), Quaternion.identity, gameObject.transform);
                        currentCell.instantiated = true;
                    }
                }
            }
        }

        CellAlgorithm nextCellToCollapse = GetSmallerEntropy();

        if (nextCellToCollapse == null)
        {
            Generating = false;
            EditorApplication.update -= EditorUpdate;
        }
        else
        {
            CollapseCell(nextCellToCollapse);
        }

        IterationNumber += 1;
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
            topCell.tileOptions = topCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.UpSocketID == t.DownSocketID))?.ToArray();

            if (topCell.tileOptions.Length == 1)
            {
                CollapseCell(topCell);
                //Debug.Log("Voy a colapsar");
            }
            else
            {
                if (possibleTilesLength > topCell.tileOptions.Length)
                {
                    FilterPossibleOptions(topCell);
                }
            }
        }

        if (bottomCell != null && !bottomCell.collapsed)
        {
            var possibleTilesLength = bottomCell.tileOptions.Length;
            bottomCell.tileOptions = bottomCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.DownSocketID == t.UpSocketID))?.ToArray();

            if (bottomCell.tileOptions.Length == 1)
            {
                CollapseCell(bottomCell);
            }
            else
            {
                if (possibleTilesLength > bottomCell.tileOptions.Length)
                {
                    FilterPossibleOptions(bottomCell);
                }
            }
        }

        if (leftCell != null && !leftCell.collapsed)
        {
            var possibleTilesLength = leftCell.tileOptions.Length;
            leftCell.tileOptions = leftCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.LeftSocketID == t.RightSocketID))?.ToArray();

            if (leftCell.tileOptions.Length == 1)
            {
                CollapseCell(leftCell);
            }
            else
            {
                if (possibleTilesLength > leftCell.tileOptions.Length)
                {
                    FilterPossibleOptions(leftCell);
                }
            }
        }

        if (rightCell != null && !rightCell.collapsed)
        {
            var possibleTilesLength = rightCell.tileOptions.Length;
            rightCell.tileOptions = rightCell.tileOptions.Where(t => cell.tileOptions.Any(p => p.RightSocketID == t.LeftSocketID))?.ToArray();

            if (rightCell.tileOptions.Length == 1)
            {
                CollapseCell(rightCell);
            }
            else
            {
                if (possibleTilesLength > rightCell.tileOptions.Length)
                {
                    FilterPossibleOptions(rightCell);
                }
            }
        }
    }

    private void CollapseCell(CellAlgorithm cell)
    {
        TileAlgorithm selectedTile = null;

        if (cell.tileOptions.Length == 1)
        {
            selectedTile = cell.tileOptions[0];
        }
        else
        {
            var sumOfWeights = cell.tileOptions.Sum(c => c.Weight);
            var pickedNumber = UnityEngine.Random.Range(0, sumOfWeights);
            TileAlgorithm pickedTile = null;

            float cumulativeWeight = 0;
            foreach (var item in cell.tileOptions)
            {
                cumulativeWeight += item.Weight;
                if (pickedNumber <= cumulativeWeight)
                {
                    pickedTile = item;
                    break;
                }
            }

            var sameWeightTiles = cell.tileOptions.Where(c => c.Weight == pickedTile.Weight).ToArray();

            if (sameWeightTiles.Length > 1)
            {
                var pickedIndex = UnityEngine.Random.Range(0, sameWeightTiles.Length);

                selectedTile = sameWeightTiles[pickedIndex];
            }
            else
            {
                if (pickedTile is null)
                {
                    selectedTile = cell.tileOptions.OrderByDescending(c => c.Weight).First();
                }
                else
                {
                    selectedTile = pickedTile;
                }
            }
        }

        cell.selectedTile = selectedTile;
        cell.tileOptions = new[] { cell.selectedTile };
        cell.collapsed = true;
        //Debug.Log("Patateuelas");
        FilterPossibleOptions(cell);
    }

    private CellAlgorithm GetTopCell(int row, int col)
    {
        if (row == dimension - 1)
        {
            return null;
        }

        return cells[row + 1, col];
    }

    private CellAlgorithm GetBottomCell(int row, int col)
    {
        if (row == 0)
        {
            return null;
        }

        return cells[row - 1, col];
    }

    private CellAlgorithm GetLeftCell(int row, int col)
    {
        if (col == 0)
        {
            return null;
        }

        return cells[row, col - 1];
    }

    private CellAlgorithm GetRightCell(int row, int col)
    {
        if (col == dimension - 1)
        {
            return null;
        }

        return cells[row, col + 1];
    }

    private CellAlgorithm GetSmallerEntropy()
    {
        CellAlgorithm nextCellToCollapse = null;
        float smallerEntropy = float.MaxValue;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                var candidateCell = cells[i, j];

                if (candidateCell.collapsed)
                {
                    continue;
                }


                //var weightSum = candidateCell.tileOptions.Sum(c => 1 / (float)c.Weight);
                //var weightSumLog = candidateCell.tileOptions.Sum(c => (1 / (float)c.Weight) * math.log(1 / (float)c.Weight));
                var weightSum = candidateCell.tileOptions.Sum(c => (float)c.Weight);
                var weightSumLog = candidateCell.tileOptions.Sum(c => ((float)c.Weight) * math.log((float)c.Weight));

                var entropy = math.log(weightSum) - (weightSumLog / weightSum);

                if (entropy < smallerEntropy)
                {
                    smallerEntropy = entropy;
                    nextCellToCollapse = candidateCell;
                }
            }
        }

        return nextCellToCollapse;
    }

    public void Generate()
    {
        UsedSeed = UnityEngine.Random.Range(1, 2000000);
        UnityEngine.Random.InitState(UsedSeed);

        IterationNumber = 0;
        ClearGridObjects();

        cells = new CellAlgorithm[dimension, dimension];
        InitCells();
        Generating = true;
        CollapseCell(cells[0, 0]);

        MaxIterationNumber = dimension * dimension;
        EditorApplication.update += EditorUpdate;
    }

    public void ClearGridObjects()
    {
        while (gameObject.transform.childCount > 0)
        {
            DestroyImmediate(gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject);
        }
    }
}
