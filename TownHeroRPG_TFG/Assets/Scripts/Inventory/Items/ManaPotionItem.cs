using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mana Potion", menuName = "Items/Mana Potion")]
public class ManaPotionItem : InventoryItem
{
    [Header("Effects")]
    public float manaAmount;

    
    public override bool Use()
    {
        if(InventoryManager.Instance.Player.PlayerMana.CanRestore)
        {
            InventoryManager.Instance.Player.PlayerMana.RestoreManaItem(manaAmount);
            return true;
        }

        return false;
    }
    
}