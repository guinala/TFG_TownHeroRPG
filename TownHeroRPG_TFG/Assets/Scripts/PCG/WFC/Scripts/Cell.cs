using System;
using UnityEngine;

[Serializable]
public class Cell
{
    public int Row;
    public int Col;

    public Tile[] PossibleTiles;
    public Tile Tile;
    public bool Collapsed;
    public bool Instantiated;
}
