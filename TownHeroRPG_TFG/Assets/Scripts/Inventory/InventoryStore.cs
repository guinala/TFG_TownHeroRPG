using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryStore", menuName = "Inventory/InventoryStore")]
public class InventoryStore : ScriptableObject
{
    public InventoryItem[] Items;
}