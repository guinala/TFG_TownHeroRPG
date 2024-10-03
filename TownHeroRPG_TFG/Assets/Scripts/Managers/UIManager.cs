using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Stats")] 
    [SerializeField] private PlayerStats stats;
    
    [Header("Paneles")] 
    [SerializeField] private GameObject panelStats;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject panelCrafting;
    [SerializeField] private GameObject panelCraftingInfo;
    [SerializeField] private GameObject panelInventory;
    [SerializeField] private GameObject panelQuests;
    [SerializeField] private GameObject panelCharacterQuests;
    
    [Header("Barra")]
    [SerializeField] private Image healthPlayer;
    [SerializeField] private Image manaPlayer;
    [SerializeField] private Image expPlayer;
    
    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Stats")] 
    [SerializeField] private TextMeshProUGUI statDamageText;
    [SerializeField] private TextMeshProUGUI statDefenseText;
    [SerializeField] private TextMeshProUGUI statCriticalText;
    [SerializeField] private TextMeshProUGUI statBlockText;
    [SerializeField] private TextMeshProUGUI statSpeedText;
    [SerializeField] private TextMeshProUGUI statLevelText;
    [SerializeField] private TextMeshProUGUI statExpText;
    [SerializeField] private TextMeshProUGUI statExpRequiredText;
    [SerializeField] private TextMeshProUGUI statTotalExpText;
    [SerializeField] private TextMeshProUGUI attributeStrengthText;
    [SerializeField] private TextMeshProUGUI attributeIntelligenceText;
    [SerializeField] private TextMeshProUGUI attributeDexterityText;
    [SerializeField] private TextMeshProUGUI attributeAvailablePointsText;

    private float actualHealth;
    private float maxHealth;
    private float actualMana;
    private float maxMana;
    private float actualExp;
    private float nextLevelExpRequired;

    private void Update()
    {
        UpdateUICharacter();
        UpdateStatsPanel();
    }

    private void UpdateUICharacter()
    {
        if (maxHealth > 0 && actualHealth > 0)
        {
            healthPlayer.fillAmount = Mathf.Lerp(healthPlayer.fillAmount,
                actualHealth / maxHealth, 10f * Time.deltaTime);
        }

        if (maxMana > 0 && actualMana > 0)
        {
            manaPlayer.fillAmount = Mathf.Lerp(manaPlayer.fillAmount,
                actualMana / maxMana, 10f * Time.deltaTime);
        }

        if (nextLevelExpRequired > 0)
        {
            expPlayer.fillAmount = Mathf.Lerp(expPlayer.fillAmount,
                actualExp / nextLevelExpRequired, 10f * Time.deltaTime);
        }

        healthText.text = $"{actualHealth}/{maxHealth}";
        manaText.text = $"{actualMana}/{maxMana}";
        
        if (nextLevelExpRequired > 0)
        {
            expText.text = $"{((actualExp / nextLevelExpRequired) * 100):F2}%";
        }
        
        levelText.text = $"Nivel {stats.Level}";
        // monedasTMP.text = MonedasManager.Instance.MonedasTotales.ToString();
    }

    private void UpdateStatsPanel()
    {
        if (panelStats.activeSelf == false)
        {
            return;
        }

        statDamageText.text = stats.Damage.ToString();
        statDefenseText.text = stats.Defense.ToString();
        statCriticalText.text = $"{stats.Critical}%";
        statBlockText.text = $"{stats.Block}%";
        statSpeedText.text = stats.Speed.ToString();
        statLevelText.text = stats.Level.ToString();
        statExpText.text = stats.Experience.ToString();
        statExpRequiredText.text = stats.ExpRequired.ToString();
        statTotalExpText.text = stats.ExpTotal.ToString();
        
        attributeStrengthText.text = stats.strength.ToString();
        attributeDexterityText.text = stats.dexterity.ToString();
        attributeIntelligenceText.text = stats.intelligence.ToString();
        attributeAvailablePointsText.text = $"Puntos: {stats.availablePoints}";
    }
    
    public void UpdateHealthBar(float pVidaActual, float pVidaMax)
    {
        actualHealth = pVidaActual;
        maxHealth = pVidaMax;
    }
    
    public void UpdateManaBar(float pManaActual, float pManaMax)
    {
        actualMana = pManaActual;
        maxMana = pManaMax;
    }
    
    public void UpdateExpBar(float pExpActual, float pExpRequerida)
    {
        actualExp = pExpActual;
        nextLevelExpRequired = pExpRequerida;
    }

    #region Paneles

    public void ShowPanelStats()
    {
        panelStats.SetActive(!panelStats.activeSelf);
    }

    public void ShowShopPanel()
    {
        panelShop.SetActive(!panelShop.activeSelf);
    }

    public void OpenCraftingPanel()
    {
        panelCrafting.SetActive(true);
    }
    
    public void ClosePanelCrafting()
    {
        panelCrafting.SetActive(false);
        ShowCraftInfo(false);
    }

    public void ShowCraftInfo(bool state)
    {
        panelCraftingInfo.SetActive(state);
    }
    
    public void ShowInventoryPanel()
    {
        panelInventory.SetActive(!panelInventory.activeSelf);
    }

    public void ShowCharacterQuests()
    {
        panelCharacterQuests.SetActive(!panelCharacterQuests.activeSelf);
    }
    
    public void ShowQuestsPanel()
    {
        panelQuests.SetActive(!panelQuests.activeSelf);
    }

    /*
    public void AbrirPanelInteraccion(InteraccionExtraNPC tipoInteraccion)
    {
        switch (tipoInteraccion)
        {
            case InteraccionExtraNPC.Quests:
                AbrirCerrarPanelInspectorQuests();
                break;
            case InteraccionExtraNPC.Tienda:
                AbrirCerrarPanelTienda();
                break;
            case InteraccionExtraNPC.Crafting:
                AbriPanelCrafting();
                break;
        }
    }
    */
    
    #endregion
}
