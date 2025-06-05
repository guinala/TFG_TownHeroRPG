using System;
using UnityEngine;

[Serializable]
public class TileWFC : MonoBehaviour
{
    [Header("Sockets")]
    public int UpSocketID;
    public int DownSocketID;
    public int LeftSocketID;
    public int RightSocketID;

    [Header("Weight (Frequency)")] [Range(1, 100)]
    public int Weight;

    public bool UseOnEdges = false;

    public bool walkable = true;

    public int GetSocket(string direction)
    {
        switch (direction)
        {
            case "Up": return UpSocketID;
            case "Down": return DownSocketID;
            case "Left": return LeftSocketID;
            case "Right": return RightSocketID;
            default: throw new ArgumentException("Dirección inválida");
        }
    }
}
