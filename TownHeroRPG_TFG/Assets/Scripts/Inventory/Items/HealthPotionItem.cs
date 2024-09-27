using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Items/Health Potion")]
public class HealthPotionItem : InventoryItem
{
    [Header("Effects")]
    public float healthAmount;

    public override bool Use()
    {
        if(InventoryManager.Instance.Player.PlayerHealth.canRestore)
        {
            InventoryManager.Instance.Player.PlayerHealth.RestoreHealth(healthAmount);
            return true;
        }

        return false;
    }

    public override string DescriptionCraftThing()
    {
        string description = $"Restore {healthAmount} health points.";
        return description;
    }
}