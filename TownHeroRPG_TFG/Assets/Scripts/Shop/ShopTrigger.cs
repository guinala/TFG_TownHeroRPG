using System;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("Configuration")]
    public CharacterShopInventorySO shopInventory;

    public static Action<CharacterShopInventorySO> onShopTriggered;
    
    public void TriggerShop()
    {
        onShopTriggered?.Invoke(shopInventory);
    }
}