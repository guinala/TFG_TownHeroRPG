using System;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
    [Header("General config")]
    public bool CanBeUsedOnTheEdge = true;

    [Header("Socket config")]
    public int UpSocket;
    public int DownSocket;
    public int LeftSocket;
    public int RightSocket;

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
