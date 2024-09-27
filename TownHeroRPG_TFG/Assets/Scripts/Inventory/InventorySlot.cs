using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType
{
    Click,
    Use,
    Equip,
    Remove
}

public class InventorySlot : MonoBehaviour
{
    public static Action <InteractionType, int> interaction;

    [SerializeField] private Image icon;
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI quantityText;

    public int Index { get; set; }

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void UpdateSlot(InventoryItem item, int quantity)
    {
        icon.sprite = item.icon;
        quantityText.text = quantity.ToString();
    }

    public void EnableSlot(bool state)
    {
        icon.gameObject.SetActive(state);
        background.SetActive(state);
    }

    public void ClickSlot()
    {
        interaction?.Invoke(InteractionType.Click, Index);

        //Move Item
        if(InventoryUI.Instance.initialMoveSlot != -1)
        {
            if(InventoryUI.Instance.initialMoveSlot != Index)
            {
                InventoryManager.Instance.MoveItem(InventoryUI.Instance.initialMoveSlot, Index);
            }
        }
    }

    public void UseSlotItem()
    {
        if (InventoryManager.Instance.itemsInventory[Index] != null)
        {
            interaction?.Invoke(InteractionType.Use, Index);
        }
    }

    public void EquipSlotItem()
    {
        if (InventoryManager.Instance.itemsInventory[Index] != null)
        {
            Debug.Log("invasdoa");
            interaction?.Invoke(InteractionType.Equip, Index);
        }
    }

    public void RemoveSlotItem()
    {
        if (InventoryManager.Instance.itemsInventory[Index] != null)
        {
            interaction?.Invoke(InteractionType.Remove, Index);
        }
    }

    public void SelectSlot()
    {
        button.Select();
    }
    
}