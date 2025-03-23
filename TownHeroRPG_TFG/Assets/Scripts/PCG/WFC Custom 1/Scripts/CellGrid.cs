using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    public bool collapsed;
    public TileGrid[] tileOptions;

    public void CreateCell(bool collapseState, TileGrid[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    public void RecreateCell(TileGrid[] tiles)
    {
        tileOptions = tiles;
    }
}
