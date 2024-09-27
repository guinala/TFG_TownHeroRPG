using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    [Header("Config")]
    [SerializeField] private ItemShop itemPrefab;
    [SerializeField] private Transform shopPanel;

    [Header("Items")]
    [SerializeField] private ItemSale[] items;

    private void Start()
    {
        LoadItems();
    }

    private void LoadItems()
    {
        for(int i = 0; i < items.Length; i++)
        {
            ItemShop itemShop = Instantiate(itemPrefab, shopPanel);
            itemShop.ConfigureShopPanel(items[i]);
        }
    }
}