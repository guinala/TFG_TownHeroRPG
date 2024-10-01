using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    [Header("Temp")]
    [SerializeField] private GameObject shopPanelObject;
    
    [Header("Open And Close Events")]
    public UnityEvent onShopOpened;
    public UnityEvent onShopClosed;

    [Header("Config")]
    [SerializeField] private ItemShop itemPrefab;
    [SerializeField] private Transform shopPanel;
    
    private CharacterShopInventorySO items;

    private void OnEnable()
    {
        ShopTrigger.onShopTriggered += ShopOpen;
    }

    public void ShopClose()
    {
        items = null;
        if (this.onShopClosed != null)
            onShopClosed?.Invoke();
    }

    
    public void ShopOpen(CharacterShopInventorySO itemsToLoad)
    {
        if (this.items != null)
            return;
        //shopPanelObject.SetActive(true);
        items = itemsToLoad;
        LoadItems();
        if (this.onShopOpened != null)
            onShopOpened?.Invoke();
    }

    private void LoadItems()
    {
        for(int i = 0; i < items.itemsShop.Length; i++)
        {
            ItemShop itemShop = Instantiate(itemPrefab, shopPanel);
            itemShop.ConfigureShopPanel(items.itemsShop[i]);
        }
    }
}