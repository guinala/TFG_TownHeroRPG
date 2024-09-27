using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : Singleton<InventoryUI>
{
    [Header("Inventory Panel Description")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private InventorySlot slot;
    [SerializeField] private Transform container;

    public int initialMoveSlot { get; private set; }
    public InventorySlot slotSelection { get; private set; }
    private List<InventorySlot> slotList = new List<InventorySlot>();

   

    // Start is called before the first frame update
    void Start()
    {
        InitializeInventory();
        initialMoveSlot = -1;
    }

    private void Update()
    {
        UpdateSlotSelection();
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(slotSelection != null)
            {
                initialMoveSlot = slotSelection.Index;
            }
        }
    }
    

    public void InitializeInventory()
    {
        for (int i = 0; i < InventoryManager.Instance.Slots; i++)
        {
            InventorySlot newSlot = Instantiate(slot, container);
            newSlot.Index = i;
            slotList.Add(newSlot);
        }
    }

    public void drawItem(InventoryItem item, int quantity, int index)
    {
        InventorySlot slot = slotList[index];

        if(item != null)
        {
            slot.EnableSlot(true);
            slot.UpdateSlot(item, quantity);
        }
        else
        {
            slot.EnableSlot(false);
        }
    }

    private void UpdateSlotSelection()
    {
        GameObject selection = EventSystem.current.currentSelectedGameObject;

        if (selection == null)
        {
            return;
        }

        InventorySlot slot = selection.GetComponent<InventorySlot>();

        if (slot != null)
        {
            slotSelection = slot;
        }
    }

    private void UpdateDescriptionPanel(int index)
    {
        if (InventoryManager.Instance.itemsInventory[index] != null)
        {
            InventoryItem item = InventoryManager.Instance.itemsInventory[index];
            icon.sprite = item.icon;
            nameText.text = item.name;
            descriptionText.text = item.description;
            descriptionPanel.SetActive(true);
        }

        else
        {
            descriptionPanel.SetActive(false);
        }
    }

    public void UseItem()
    {
        if (slotSelection != null)
        {
            slotSelection.UseSlotItem();
            slotSelection.SelectSlot();
        }
    }

    public void EquipItem()
    {
        if (slotSelection != null)
        {
            slotSelection.EquipSlotItem();
            slotSelection.SelectSlot();
        }
    }

    public void RemoveItem()
    {
        if (slotSelection != null)
        {
            slotSelection.RemoveSlotItem();
            slotSelection.SelectSlot();
        }
    }

    #region Event
    private void OnSlotInteraction(InteractionType type, int index)
        {
            if (type == InteractionType.Click)
            {
                UpdateDescriptionPanel(index);
            }
        }

        private void OnEnable()
        {
            InventorySlot.interaction += OnSlotInteraction;
        }

        private void OnDisable()
        {
            InventorySlot.interaction -= OnSlotInteraction;
        }
    #endregion

    
   
}
