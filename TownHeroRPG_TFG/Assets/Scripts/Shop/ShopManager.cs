using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Temp")]
    [SerializeField] private GameObject shopPanelObject;

    [Header("Config")]
    [SerializeField] private ItemShop itemPrefab;
    [SerializeField] private Transform shopPanel;
    
    private CharacterShopInventorySO items;

    private void OnEnable()
    {
        ShopTrigger.onShopTriggered += LoadItems;
    }

    private void LoadItems(CharacterShopInventorySO itemsToLoad)
    {
        shopPanelObject.SetActive(true);
        items = itemsToLoad;
        
        for(int i = 0; i < items.itemsShop.Length; i++)
        {
            ItemShop itemShop = Instantiate(itemPrefab, shopPanel);
            itemShop.ConfigureShopPanel(items.itemsShop[i]);
        }
    }
}