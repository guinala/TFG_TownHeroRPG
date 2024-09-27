using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Receipe
{
    public string Name;
    [Header("1st Material")]
    public InventoryItem Material1;
    public int amountRequired1;
    [Header("2nd Material")]
    public InventoryItem Material2;
    public int amountRequired2;

    [Header("Result")]
    public InventoryItem Result;
    public int amountResult;
}