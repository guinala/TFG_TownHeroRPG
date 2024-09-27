using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceipeCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;


    public Receipe LoadedReceipe { get; set; }

    public void LoadReceipe(Receipe receipe)
    {
        LoadedReceipe = receipe;
        icon.sprite = receipe.Result.icon;
        nameText.text = receipe.Result.name;
    }

    public void Select()
    {
        Debug.Log("Empanasdas CON ATTUN");
        //UIManager.Instance.ShowCraftInfoPanel(true);
        CraftingManager.Instance.Display(LoadedReceipe);
    }
}