using System;
using UnityEngine;

[Serializable]
public class CellAlgorithm
{
    [Header("Tiles")]
    public TileAlgorithm[] tileOptions;
    public TileAlgorithm selectedTile;

    [Header("Position")]
    public int row;
    public int col;

    [Header("State")]
    public bool collapsed;
    public bool instantiated;
}
