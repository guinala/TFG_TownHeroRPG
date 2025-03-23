using System;
using UnityEngine;

[Serializable]
public class TileModule
{
    // public Tile BaseTile;
    public bool Enabled;
    public bool ShowOnInspector;
    public bool EnableInsideOut;

    [Header("Frequency"), Range(1, 100)]
    public int Frequency;

    public Tile BaseOutTopLeft;
    public Tile BaseOutTop;
    public Tile BaseOutTopRight;
    public Tile BaseOutCenterLeft;
    public Tile BaseOutCenter;
    public Tile BaseOutCenterRight;
    public Tile BaseOutBottomLeft;
    public Tile BaseOutBottom;
    public Tile BaseOutBottomRight;

    public Tile BaseInTopLeft;
    public Tile BaseInTop;
    public Tile BaseInTopRight;
    public Tile BaseInCenterLeft;
    public Tile BaseInCenterRight;
    public Tile BaseInBottomLeft;
    public Tile BaseInBottom;
    public Tile BaseInBottomRight;

    public Tile[] GetTiles()
    {
        if (!Enabled)
        {
            return new Tile[] { };
        }

        return new Tile[] { BaseOutTopLeft, BaseOutTop,
            BaseOutTopRight, BaseOutCenterLeft, BaseOutCenter,
            BaseOutCenterRight, BaseOutBottomLeft, BaseOutBottom,
            BaseOutBottomRight, BaseInTopLeft, BaseInTop,
            BaseInTopRight, BaseInCenterLeft,
            BaseInCenterRight, BaseInBottomLeft, BaseInBottom,
            BaseInBottomRight };
    }
}