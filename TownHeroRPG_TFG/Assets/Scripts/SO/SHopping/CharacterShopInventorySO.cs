using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterShopInventory", menuName = "ScriptableObjects/Shopping/CharacterShopInventory")]
public class CharacterShopInventorySO : ScriptableObject
{
    public ItemSale[] itemsShop;
}
