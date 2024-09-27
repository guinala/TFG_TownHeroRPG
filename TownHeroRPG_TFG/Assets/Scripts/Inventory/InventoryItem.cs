using System.Collections;
using UnityEngine;

public enum ItemType
{
    Weapons,
    Potions,
    Scrolls,
    Ingredients,
    Treasures
}

public class InventoryItem : ScriptableObject
{
    [Header("Parameters")]
    public string id;
    public string name;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Info")]
    public ItemType type;
    public bool Consumible;
    public bool Stackable;
    public int maxAccumulation;

    [HideInInspector] public int Quantity;

    public InventoryItem CopyItem()
    {
        InventoryItem copy = Instantiate(this);

        return copy;
    }

    public virtual bool Use()
    {
        return true;
    }

    public virtual bool Equip()
    {
        return true;
    }

    public virtual bool Remove()
    {
        return true;
    }

    public virtual string DescriptionCraftThing()
    {
        return "";
    }
}
