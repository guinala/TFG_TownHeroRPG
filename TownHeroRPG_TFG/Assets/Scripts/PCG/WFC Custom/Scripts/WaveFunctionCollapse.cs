using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode, Serializable]
public class WaveFunctionCollapse : MonoBehaviour
{
    [SerializeField] private bool showGrid;

    [Header("Grid config"), SerializeField] private int dimension = 4;
    [SerializeField, Tooltip("Leave the number at 0 to have random generated maps")] private int mapSeed = 0;

    private TileWFC[] tilesList;
    [Header("Tiles disponibles")]
    [SerializeField] public List<TileWFC> tiles = new List<TileWFC>();
    private CellWFC[,] gridCells;

    [HideInInspector] public TileWFC BaseTile;
    //[HideInInspector, SerializeField] public List<TilePatternWFC> TilePatternList;
    [HideInInspector] public bool Generate;
    [HideInInspector] public int UsedSeed;
    [HideInInspector] public int IterationNumber;
    public int MaxIterationNumber;
    [HideInInspector] public double IterationPerSecond;
    [HideInInspector] public double ElapsedTime;

    private double executionStartup;
    
    [Header("Aprendizaje automático de reglas")]
    public bool LearnFromImage = false; // Activar para aprender reglas desde una imagen
    public GameObject ReferenceImage;

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
        Generate = false;
        // TilePatternList = new List<TilePattern>();
    }

    void EditorUpdate()
    {
        if (Generate && IterationNumber < MaxIterationNumber)
        {
            WaveFunction();
            ElapsedTime = EditorApplication.timeSinceStartup - executionStartup;
            IterationPerSecond = IterationNumber / ElapsedTime;
        }
    }

    /*
    ██╗    ██╗███████╗ ██████╗    ███╗   ███╗███████╗████████╗██╗  ██╗ ██████╗ ██████╗ ███████╗
    ██║    ██║██╔════╝██╔════╝    ████╗ ████║██╔════╝╚══██╔══╝██║  ██║██╔═══██╗██╔══██╗██╔════╝
    ██║ █╗ ██║█████╗  ██║         ██╔████╔██║█████╗     ██║   ███████║██║   ██║██║  ██║███████╗
    ██║███╗██║██╔══╝  ██║         ██║╚██╔╝██║██╔══╝     ██║   ██╔══██║██║   ██║██║  ██║╚════██║
    ╚███╔███╔╝██║     ╚██████╗    ██║ ╚═╝ ██║███████╗   ██║   ██║  ██║╚██████╔╝██████╔╝███████║
     ╚══╝╚══╝ ╚═╝      ╚═════╝    ╚═╝     ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═════╝ ╚══════╝
    */

    void InitCells()
    {
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                gridCells[i, j] = new CellWFC
                {
                    Row = i,
                    Col = j,
                    Collapsed = false,
                    PossibleTiles = (i == 0 || j == 0 || i == dimension - 1 || j == dimension - 1) 
                        ? tiles.Where(t => t.CanBeUsedOnTheEdge).ToArray() 
                        : tiles.ToArray()
                };
            }
        }
    }
    
    public void LearnAdjacencyRulesFromImage()
    {
        if (ReferenceImage == null) return;

        // Reiniciar todas las compatibilidades
        foreach (TileWFC tile in tiles)
        {
            tile.UpCompatibleTiles.Clear();
            tile.DownCompatibleTiles.Clear();
            tile.LeftCompatibleTiles.Clear();
            tile.RightCompatibleTiles.Clear();
        }

        // Obtener tiles y posiciones
        TileWFC[] imageTiles = ReferenceImage.GetComponentsInChildren<TileWFC>();
        Dictionary<Vector2Int, TileWFC> positionToTile = new Dictionary<Vector2Int, TileWFC>();

        foreach (TileWFC tile in imageTiles)
        {
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(tile.transform.localPosition.x),
                Mathf.FloorToInt(tile.transform.localPosition.y)
            );
            positionToTile[gridPos] = tile;
        }

        // Aprender reglas
        foreach (var entry in positionToTile)
        {
            Vector2Int pos = entry.Key;
            TileWFC currentTile = entry.Value;

            CheckAndAddNeighbor(pos + Vector2Int.up, currentTile, Direction.Down, positionToTile);
            CheckAndAddNeighbor(pos + Vector2Int.down, currentTile, Direction.Up, positionToTile);
            CheckAndAddNeighbor(pos + Vector2Int.left, currentTile, Direction.Right, positionToTile);
            CheckAndAddNeighbor(pos + Vector2Int.right, currentTile, Direction.Left, positionToTile);
        }

        Debug.Log("Reglas aprendidas correctamente.");
    }

    private void CheckAndAddNeighbor(Vector2Int neighborPos, TileWFC currentTile, Direction direction, Dictionary<Vector2Int, TileWFC> positionToTile)
    {
        Debug.Log("Holaaa");
        if (positionToTile.TryGetValue(neighborPos, out TileWFC neighborTile))
        {
            // Añadir vecino a la dirección correspondiente del tile actual
            switch (direction)
            {
                case Direction.Up:
                    SafeAdd(currentTile.UpCompatibleTiles, neighborTile);
                    SafeAdd(neighborTile.DownCompatibleTiles, currentTile);
                    break;
                case Direction.Down:
                    SafeAdd(currentTile.DownCompatibleTiles, neighborTile);
                    SafeAdd(neighborTile.UpCompatibleTiles, currentTile);
                    break;
                case Direction.Left:
                    SafeAdd(currentTile.LeftCompatibleTiles, neighborTile);
                    SafeAdd(neighborTile.RightCompatibleTiles, currentTile);
                    break;
                case Direction.Right:
                    SafeAdd(currentTile.RightCompatibleTiles, neighborTile);
                    SafeAdd(neighborTile.LeftCompatibleTiles, currentTile);
                    break;
            }
        }
    }
    
    private void SafeAdd(List<TileWFC> list, TileWFC tile)
    {
        if (!list.Contains(tile)) list.Add(tile);
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
    // private void SetBaseOutTileSockets(int baseTileNumber, TilePatternWFC pattern, int baseNumber)
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
    // private void SetBaseInTileSockets(int baseTileNumber, TilePatternWFC pattern, int baseNumber)
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
        for (int i = 0; i < gridCells.GetLength(0); i++)
        {
            for (int j = 0; j < gridCells.GetLength(1); j++)
            {
                var currentCell = gridCells[i, j];

                if (currentCell.Collapsed)
                {
                    // Instantiate the collapsed cell
                    if (!currentCell.Instantiated)
                    {
                        Instantiate(currentCell.Tile, new Vector3(j, i, 0f), Quaternion.identity, gameObject.transform);
                        currentCell.Instantiated = true;
                    }
                }
            }
        }

        CellWFC nextCellToCollapse = GetSmallerEntropy();

        if (nextCellToCollapse == null)
        {
            Generate = false;
            EditorApplication.update -= EditorUpdate;
        }
        else
        {
            CollapseCell(nextCellToCollapse);
        }

        IterationNumber += 1;
    }
    
    public void CalculateWeightsFromImage()
    {
        Dictionary<TileWFC, int> frequency = new Dictionary<TileWFC, int>();
        foreach (TileWFC tile in ReferenceImage.GetComponentsInChildren<TileWFC>())
        {
            if (frequency.ContainsKey(tile)) frequency[tile]++;
            else frequency[tile] = 1;
        }

        foreach (TileWFC tile in tiles)
        {
            if (frequency.TryGetValue(tile, out int count))
                tile.Weight = count; // Asignar frecuencia como peso
            else
                tile.Weight = 1; // Valor por defecto
        }
    }

    private void FilterPossibleOptions(CellWFC cell)
    {
        Debug.Log("Lo intento");
        var topCell = GetTopCell(cell.Row, cell.Col);
        var bottomCell = GetBottomCell(cell.Row, cell.Col);
        var leftCell = GetLeftCell(cell.Row, cell.Col);
        var rightCell = GetRightCell(cell.Row, cell.Col);

        // Filtrado para celda superior (arriba)
        if (topCell != null && !topCell.Collapsed)
        {
            topCell.PossibleTiles = topCell.PossibleTiles
                .Where(t => cell.PossibleTiles.Any(p => p.UpCompatibleTiles.Contains(t)))
                .ToArray();
        }

        // Filtrado para celda inferior (abajo)
        if (bottomCell != null && !bottomCell.Collapsed)
        {
            bottomCell.PossibleTiles = bottomCell.PossibleTiles
                .Where(t => cell.PossibleTiles.Any(p => p.DownCompatibleTiles.Contains(t)))
                .ToArray();
        }
        
        if (leftCell != null && !leftCell.Collapsed)
        {
            leftCell.PossibleTiles = leftCell.PossibleTiles
                .Where(t => cell.PossibleTiles.Any(p => p.LeftCompatibleTiles.Contains(t)))
                .ToArray();
        }
        
        if (rightCell != null && !rightCell.Collapsed)
        {
            rightCell.PossibleTiles = rightCell.PossibleTiles
                .Where(t => cell.PossibleTiles.Any(p => p.RightCompatibleTiles.Contains(t)))
                .ToArray();
        }
    }

    private void CollapseCell(CellWFC cell)
    {
        TileWFC selectedTile = null;

        if (cell.PossibleTiles.Length == 1)
        {
            selectedTile = cell.PossibleTiles[0];
        }
        else
        {
            var sumOfWeights = cell.PossibleTiles.Sum(c => c.Weight);
            var pickedNumber = UnityEngine.Random.Range(0, sumOfWeights);
            TileWFC pickedTile = null;

            float cumulativeWeight = 0;
            foreach (var item in cell.PossibleTiles)
            {
                cumulativeWeight += item.Weight;
                if (pickedNumber <= cumulativeWeight)
                {
                    pickedTile = item;
                    break;
                }
            }

            var sameWeightTiles = cell.PossibleTiles.Where(c => c.Weight == pickedTile.Weight).ToArray();

            if (sameWeightTiles.Length > 1)
            {
                var pickedIndex = UnityEngine.Random.Range(0, sameWeightTiles.Length);

                selectedTile = sameWeightTiles[pickedIndex];
            }
            else
            {
                if (pickedTile is null)
                {
                    selectedTile = cell.PossibleTiles.OrderByDescending(c => c.Weight).First();
                }
                else
                {
                    selectedTile = pickedTile;
                }
            }
        }

        cell.Tile = selectedTile;
        cell.PossibleTiles = new[] { cell.Tile };
        cell.Collapsed = true;

        FilterPossibleOptions(cell);
    }

    private CellWFC GetTopCell(int row, int col)
    {
        if (row == dimension - 1)
        {
            return null;
        }

        return gridCells[row + 1, col];
    }

    private CellWFC GetBottomCell(int row, int col)
    {
        if (row == 0)
        {
            return null;
        }

        return gridCells[row - 1, col];
    }

    private CellWFC GetLeftCell(int row, int col)
    {
        if (col == 0)
        {
            return null;
        }

        return gridCells[row, col - 1];
    }

    private CellWFC GetRightCell(int row, int col)
    {
        if (col == dimension - 1)
        {
            return null;
        }

        return gridCells[row, col + 1];
    }

    private CellWFC GetSmallerEntropy()
    {
        CellWFC nextCellToCollapse = null;
        float smallerEntropy = float.MaxValue;

        for (int i = 0; i < gridCells.GetLength(0); i++)
        {
            for (int j = 0; j < gridCells.GetLength(1); j++)
            {
                var candidateCell = gridCells[i, j];

                if (candidateCell.Collapsed)
                {
                    continue;
                }


                var weightSum = candidateCell.PossibleTiles.Sum(c => 1 / (float)c.Weight);
                var weightSumLog = candidateCell.PossibleTiles.Sum(c => (1 / (float)c.Weight) * math.log(1 / (float)c.Weight));

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

    /*
    ███████╗██████╗ ██╗████████╗ ██████╗ ██████╗     ███╗   ███╗███████╗████████╗██╗  ██╗ ██████╗ ██████╗ ███████╗
    ██╔════╝██╔══██╗██║╚══██╔══╝██╔═══██╗██╔══██╗    ████╗ ████║██╔════╝╚══██╔══╝██║  ██║██╔═══██╗██╔══██╗██╔════╝
    █████╗  ██║  ██║██║   ██║   ██║   ██║██████╔╝    ██╔████╔██║█████╗     ██║   ███████║██║   ██║██║  ██║███████╗
    ██╔══╝  ██║  ██║██║   ██║   ██║   ██║██╔══██╗    ██║╚██╔╝██║██╔══╝     ██║   ██╔══██║██║   ██║██║  ██║╚════██║
    ███████╗██████╔╝██║   ██║   ╚██████╔╝██║  ██║    ██║ ╚═╝ ██║███████╗   ██║   ██║  ██║╚██████╔╝██████╔╝███████║
    ╚══════╝╚═════╝ ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝    ╚═╝     ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═════╝ ╚══════╝
    */

    public void GenerateMap()
    {
        if (LearnFromImage)
        {
            LearnAdjacencyRulesFromImage();
            CalculateWeightsFromImage();
        }
        
        if (mapSeed > 0)
        {
            UsedSeed = mapSeed;
            UnityEngine.Random.InitState(mapSeed);
        }
        else
        {
            UsedSeed = UnityEngine.Random.Range(1, 2000000);
            UnityEngine.Random.InitState(UsedSeed);
        }

        IterationNumber = 0;
        ClearGridObjects();

        gridCells = new CellWFC[dimension, dimension];
        InitCells();
        Generate = true;
        CollapseCell(gridCells[0, 0]);

        executionStartup = EditorApplication.timeSinceStartup;
        MaxIterationNumber = dimension * dimension;
        EditorApplication.update += EditorUpdate;
    }

    // public void AddTilePattern()
    // {
    //     TilePatternList.Add(new TilePatternWFC() { Enabled = true });
    //
    //     // tilePattern = TilePatternList.ToArray();
    // }
    //
    // public void RemoveTilePattern(int index)
    // {
    //     TilePatternList.RemoveAt(index);
    //
    //     // tilePattern = TilePatternList.ToArray();
    // }

    public void ClearGridObjects()
    {
        while (gameObject.transform.childCount > 0)
        {
            DestroyImmediate(gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject);
        }
    }
}

public enum Direction { Up, Down, Left, Right }
