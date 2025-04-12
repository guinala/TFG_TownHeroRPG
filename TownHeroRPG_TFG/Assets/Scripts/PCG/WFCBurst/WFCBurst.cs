// CellAlgorithm.cs
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[System.Serializable]
public struct CellAlgorithmBurst
{
    public int Row;
    public int Col;
    public bool Collapsed;
    public bool Instantiated;
    public NativeList<int> PossibleTileIndices; // Índices de los tiles posibles
    public int TileIndex; // Índice del tile seleccionado
}

// TileData.cs
[System.Serializable]
public struct TileData
{
    public int UpSocket;
    public int DownSocket;
    public int LeftSocket;
    public int RightSocket;
    public int Weight;
    public bool CanBeUsedOnEdge;
}

public class WFCBurst : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int dimension = 4;
    [SerializeField] private int mapSeed = 0;
    [SerializeField] private TileAlgorithm[] tilePrefabs;

    private NativeArray<CellAlgorithmBurst> cells;
    private NativeArray<TileData> tilesData;
    private Random random;
    private int currentIteration;

    private void Start()
    {
        InitializeSystem();
        ScheduleEntropyJob();
    }

    private void InitializeSystem()
    {
        // Inicializar semilla
        random = new Random((uint)(mapSeed > 0 ? mapSeed : UnityEngine.Random.Range(1, 2000000)));

        // Convertir tiles a datos compatibles con Burst
        tilesData = new NativeArray<TileData>(tilePrefabs.Length, Allocator.Persistent);
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            tilesData[i] = new TileData
            {
                UpSocket = tilePrefabs[i].UpSocket,
                DownSocket = tilePrefabs[i].DownSocket,
                LeftSocket = tilePrefabs[i].LeftSocket,
                RightSocket = tilePrefabs[i].RightSocket,
                Weight = tilePrefabs[i].ReferenceWeight,
                CanBeUsedOnEdge = tilePrefabs[i].CanBeUsedOnTheEdge
            };
        }

        // Inicializar celdas
        cells = new NativeArray<CellAlgorithmBurst>(dimension * dimension, Allocator.Persistent);
        InitializeCellsJob initializeJob = new InitializeCellsJob
        {
            Cells = cells,
            TilesData = tilesData,
            Dimension = dimension
        };
        JobHandle handle = initializeJob.Schedule(dimension * dimension, 64);
        handle.Complete();
    }

    [BurstCompile]
    private struct InitializeCellsJob : IJobParallelFor
    {
        public NativeArray<CellAlgorithmBurst> Cells;
        [ReadOnly] public NativeArray<TileData> TilesData;
        public int Dimension;

        public void Execute(int index)
        {
            int row = index / Dimension;
            int col = index % Dimension;

            CellAlgorithmBurst cell = new CellAlgorithmBurst
            {
                Row = row,
                Col = col,
                Collapsed = false,
                Instantiated = false,
                PossibleTileIndices = new NativeList<int>(Allocator.Persistent),
                TileIndex = -1
            };

            // Filtrar tiles para bordes
            for (int i = 0; i < TilesData.Length; i++)
            {
                if (IsEdge(row, col) && !TilesData[i].CanBeUsedOnEdge) continue;
                cell.PossibleTileIndices.Add(i);
            }

            Cells[index] = cell;
        }

        private bool IsEdge(int row, int col)
        {
            return row == 0 || col == 0 || row == Dimension - 1 || col == Dimension - 1;
        }
    }

    private void ScheduleEntropyJob()
    {
        FindMinEntropyJob entropyJob = new FindMinEntropyJob
        {
            Cells = cells,
            TilesData = tilesData,
            RandomSeed = random.NextUInt(),
            Dimension = dimension
        };

        JobHandle handle = entropyJob.Schedule();
        handle.Complete();

        if (entropyJob.NextCellIndex >= 0)
        {
            ScheduleCollapseJob(entropyJob.NextCellIndex);
        }
    }

    [BurstCompile]
    private struct FindMinEntropyJob : IJob
    {
        public NativeArray<CellAlgorithmBurst> Cells;
        [ReadOnly] public NativeArray<TileData> TilesData;
        public uint RandomSeed;
        public int Dimension;

        public int NextCellIndex;

        public void Execute()
        {
            float minEntropy = float.MaxValue;
            NativeList<int> candidates = new NativeList<int>(Allocator.Temp);

            for (int i = 0; i < Cells.Length; i++)
            {
                if (Cells[i].Collapsed) continue;

                float entropy = CalculateEntropy(i);
                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    candidates.Clear();
                    candidates.Add(i);
                }
                else if (math.abs(entropy - minEntropy) < 0.001f)
                {
                    candidates.Add(i);
                }
            }

            // Selección aleatoria entre celdas con misma entropía
            Random rand = new Random(RandomSeed);
            NextCellIndex = candidates.Length > 0 ? candidates[rand.NextInt(candidates.Length)] : -1;
        }

        private float CalculateEntropy(int cellIndex)
        {
            CellAlgorithmBurst cell = Cells[cellIndex];
            float sumWeights = 0;
            float sumLogWeights = 0;

            foreach (int tileIndex in cell.PossibleTileIndices)
            {
                float weight = TilesData[tileIndex].Weight;
                sumWeights += weight;
                sumLogWeights += weight * math.log(weight);
            }

            return math.log(sumWeights) - (sumLogWeights / sumWeights);
        }
    }

    private void ScheduleCollapseJob(int cellIndex)
    {
        CollapseCellJob collapseJob = new CollapseCellJob
        {
            Cells = cells,
            TilesData = tilesData,
            CellIndex = cellIndex,
            RandomSeed = random.NextUInt()
        };

        JobHandle handle = collapseJob.Schedule();
        handle.Complete();

        currentIteration++;
        if (currentIteration < dimension * dimension)
        {
            SchedulePropagationJob(cellIndex);
        }
    }

    [BurstCompile]
    private struct CollapseCellJob : IJob
    {
        public NativeArray<CellAlgorithmBurst> Cells;
        [ReadOnly] public NativeArray<TileData> TilesData;
        public int CellIndex;
        public uint RandomSeed;

        public void Execute()
        {
            CellAlgorithmBurst cell = Cells[CellIndex];
            Random rand = new Random(RandomSeed);

            // Selección ponderada
            int totalWeight = 0;
            foreach (int tileIndex in cell.PossibleTileIndices)
            {
                totalWeight += TilesData[tileIndex].Weight;
            }

            int randomPick = rand.NextInt(1, totalWeight + 1);
            int accumulated = 0;
            int selectedIndex = 0;

            for (int i = 0; i < cell.PossibleTileIndices.Length; i++)
            {
                accumulated += TilesData[cell.PossibleTileIndices[i]].Weight;
                if (accumulated >= randomPick)
                {
                    selectedIndex = cell.PossibleTileIndices[i];
                    break;
                }
            }

            // Colapsar celda
            cell.TileIndex = selectedIndex;
            cell.Collapsed = true;
            cell.PossibleTileIndices.Clear();
            cell.PossibleTileIndices.Add(selectedIndex);

            Cells[CellIndex] = cell;
        }
    }

    private void SchedulePropagationJob(int cellIndex)
    {
        // Implementar propagación similar usando Jobs
        // (Requiere lógica adicional para vecinos)
        UpdateVisuals();
        ScheduleEntropyJob();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Collapsed && !cells[i].Instantiated)
            {
                Vector3 position = new Vector3(cells[i].Col, cells[i].Row, 0);
                Instantiate(tilePrefabs[cells[i].TileIndex], position, Quaternion.identity, transform);
                
                CellAlgorithmBurst updatedCell = cells[i];
                updatedCell.Instantiated = true;
                cells[i] = updatedCell;
            }
        }
    }

    private void OnDestroy()
    {
        // Liberar memoria
        foreach (CellAlgorithmBurst cell in cells)
        {
            cell.PossibleTileIndices.Dispose();
        }
        cells.Dispose();
        tilesData.Dispose();
    }
}