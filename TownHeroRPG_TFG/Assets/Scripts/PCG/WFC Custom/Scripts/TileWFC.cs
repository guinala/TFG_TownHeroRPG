using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileWFC : MonoBehaviour
{
    [Header("General config")]
    public bool CanBeUsedOnTheEdge = true;

    [Header("Compatibility (Referencias directas)")]
    public List<TileWFC> UpCompatibleTiles = new List<TileWFC>();
    public List<TileWFC> DownCompatibleTiles = new List<TileWFC>();
    public List<TileWFC> LeftCompatibleTiles = new List<TileWFC>();
    public List<TileWFC> RightCompatibleTiles = new List<TileWFC>();

    [Header("Frequency"), Range(1, 1000)]
    public int ReferenceWeight;

    [HideInInspector]
    public int Weight;


    public Sprite GetSprite()
    {
        Sprite sprite = null;

        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            return renderer.sprite;
        }

        return sprite;
    }
}
