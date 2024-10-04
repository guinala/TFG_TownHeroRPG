using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : Singleton<CraftingManager>
{
    [Header("Config")]
    [SerializeField] ReceipeCard cardPrefab;
    [SerializeField] Transform cardParent;

    [Header("Receipe Info")]
    [SerializeField] private Image iconMaterial1;
    [SerializeField] private Image iconMaterial2;
    [SerializeField] private TextMeshProUGUI nameMaterial1;
    [SerializeField] private TextMeshProUGUI nameMaterial2;
    [SerializeField] private TextMeshProUGUI amountMaterial1;
    [SerializeField] private TextMeshProUGUI amountMaterial2;
    [SerializeField] private TextMeshProUGUI messageResult;

    [SerializeField] private Button button;

    [Header("Receipe Result")] 
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Image iconResult;
    [SerializeField] private TextMeshProUGUI nameResult;
    [SerializeField] private TextMeshProUGUI descriptionResult;



    [Header("Receipes")]
    [SerializeField] ReceipeList list;

    public Receipe selectedReceipe { get; set; }

    private void Start()
    {
        LoadReceipes();
    }

    private void LoadReceipes()
    {
        for(int i = 0; i < list.receipes.Count; i++)
        {
            ReceipeCard card = Instantiate(cardPrefab, cardParent);
            card.LoadReceipe(list.receipes[i]);
        }
    }


    public void Display(Receipe receipe)
    {
        selectedReceipe = receipe;
        
        if(iconMaterial1.enabled == false) iconMaterial1.enabled = true;
        if(iconMaterial2.enabled == false) iconMaterial2.enabled = true;
        if(nameMaterial1.enabled == false) nameMaterial1.enabled = true;
        if(nameMaterial2.enabled == false) nameMaterial2.enabled = true;
        if(amountMaterial1.enabled == false) amountMaterial1.enabled = true;
        if(amountMaterial2.enabled == false) amountMaterial2.enabled = true;
        
        iconMaterial1.sprite = receipe.Material1.icon;
        iconMaterial2.sprite = receipe.Material2.icon;
        nameMaterial1.text = receipe.Material1.name;
        nameMaterial2.text = receipe.Material2.name;
        amountMaterial1.text = $"{InventoryManager.Instance.ObtainItemsAmount(receipe.Material1.id)} / {receipe.amountRequired1}";
        amountMaterial2.text = $"{InventoryManager.Instance.ObtainItemsAmount(receipe.Material2.id)} / {receipe.amountRequired2}";
        
        if(iconResult.enabled == false) iconResult.enabled = true;
        if(nameResult.enabled == false) nameResult.enabled = true;
        if(descriptionResult.enabled == false) descriptionResult.enabled = true;

        iconResult.sprite = receipe.Result.icon;
        nameResult.text = receipe.Result.name;
        descriptionResult.text = receipe.Result.DescriptionCraftThing();
        
        button.gameObject.SetActive(true);
    }

    public bool CanCraft(Receipe receipe)
    {
        if(InventoryManager.Instance.ObtainItemsAmount(receipe.Material1.id) >= receipe.amountRequired1 &&
                       InventoryManager.Instance.ObtainItemsAmount(receipe.Material2.id) >= receipe.amountRequired2)
        {
            Debug.Log("Es posible?!!!!asqd");
            return true;
        }

        return false;
    }

    public void Craft()
    {
        if(CanCraft(selectedReceipe))
        {
            messageResult.text = "Material \n crafted!";
            //button.interactable = true;
            for(int i = 0; i < selectedReceipe.amountRequired1; i++)
            {
                InventoryManager.Instance.ConsumeItem(selectedReceipe.Material1.id);
            }

            for(int i = 0; i < selectedReceipe.amountRequired2; i++)
            {
                InventoryManager.Instance.ConsumeItem(selectedReceipe.Material2.id);
            }

            InventoryManager.Instance.AddItem(selectedReceipe.Result, selectedReceipe.amountResult);
            messagePanel.SetActive(true);
            Display(selectedReceipe);
        }

        else
        {
            messageResult.text = "You don't have enough materials!";
            messagePanel.SetActive(true);
            //button.interactable = false;
        }
        
    }
}
