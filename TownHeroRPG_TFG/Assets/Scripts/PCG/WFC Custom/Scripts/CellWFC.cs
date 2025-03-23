using System;
using UnityEngine;

[Serializable]
public class CellWFC
{
    public int Row;
    public int Col;

    public TileWFC[] PossibleTiles;
    public TileWFC Tile;
    public bool Collapsed;
    public bool Instantiated;
}
