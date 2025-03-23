using System;
using UnityEngine;

[Serializable]
public class TilePatternWFC
{
    public bool Enabled;
    public bool ShowOnInspector;
    public bool EnableInsideOut;

    [Header("Frequency"), Range(1, 100)]
    public int Frequency;

    [Header("Tilemap contenedor")]
    public GameObject TilemapContainer; // Arrastra aquí el GameObject padre con los tiles hijos

    public Tile[] GetTiles()
    {
        if (!Enabled || TilemapContainer == null)
        {
            return new Tile[] { };
        }

        // Obtener todos los tiles hijos del contenedor
        return TilemapContainer.GetComponentsInChildren<Tile>();
    }
}