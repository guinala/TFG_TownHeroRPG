using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private TextMeshProUGUI itemPrice;

    public ItemSale itemSale { get; set; }

    private int amount;
    private int initialPrice;
    private int actualPrice; //initialPrice * amount

    public void ConfigureShopPanel(ItemSale item)
    { 
        itemSale = item;
        itemImage.sprite = item.Item.icon;
        itemName.text = item.Name;
        itemPrice.text = item.Price.ToString();
        actualPrice = item.Price;
        initialPrice = item.Price;
        amount = 1;
    }

    private void Update()
    {
        itemAmount.text = amount.ToString();
        itemPrice.text = actualPrice.ToString();
    }

    public void Buy()
    {
        if(CoinManager.Instance.totalCoins >= actualPrice)
        {
            InventoryManager.Instance.AddItem(itemSale.Item, amount);
            CoinManager.Instance.RemoveCoins(actualPrice);
            amount = 1;
            actualPrice = initialPrice;
            Debug.Log("no funcionasdqweqweo");
        }
    }

    public void AddItemToBuy()
    {
        int totalPrice = initialPrice * (amount + 1);
        if(CoinManager.Instance.totalCoins >= totalPrice)
        {
            amount++;
            actualPrice = initialPrice * amount;
        }
    }


    public void RemoveItemToBuy()
    {
        if(amount == 1)
        {
            return;
        }
        amount--;
        actualPrice = initialPrice * amount;
    }
}