using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dungeon : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;

    [SerializeField] protected TilemapPainter painter = null;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        painter.ClearTiles();
        RunAlgorithm();
    }

    protected abstract void RunAlgorithm();
}
