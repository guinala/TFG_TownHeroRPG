using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public Sprite Sprite;
    public Vector2Int Size = Vector2Int.one;
    public bool Collider = true;

    [Header("Placement type:")]
    public bool DeadEnd = true;
    public bool Corner = true;
    public bool NearUpWall = true;
    public bool NearDownWall = true;
    public bool NearRightWall = true;
    public bool NearLeftWall = true;
    public bool Inner = true;
    public bool Corridor = true;

    [Min(1)]
    public int PlacementQuantityMin = 1;
    [Min(1)]
    public int PlacementQuantityMax = 1;

    [Header("Group placement:")]
    public bool PlaceAsGroup = false;
    [Min(1)]
    public int GroupMinCount = 1;
    [Min(1)]
    public int GroupMaxCount = 1;
}
