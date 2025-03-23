using System;
using UnityEngine;

[Serializable]
public class CellAlgorithm
{
    public int Row;
    public int Col;

    public  TileAlgorithm[] PossibleTiles;
    public TileAlgorithm Tile;
    public bool Collapsed;
    public bool Instantiated;
}
