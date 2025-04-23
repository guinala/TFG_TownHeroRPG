using System;
using UnityEngine;

[Serializable]
public class TileAlgorithm : MonoBehaviour
{
    [Header("Sockets")]
    public int UpSocketID;
    public int DownSocketID;
    public int LeftSocketID;
    public int RightSocketID;

    [Header("Weight (Frequency)")] [Range(1, 100)]
    public int Weight;

    public bool UseOnEdges = false;
}
