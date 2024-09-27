using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [Header("Stats GameObject")]
    [SerializeField] private PlayerStats stats;

    [Header("Panels")]
    [SerializeField] private GameObject panelStats;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject questsPanel;
    [SerializeField] private GameObject questsCharacterPanel;
    [SerializeField] private GameObject panelCraft;
    [SerializeField] private GameObject panelCraftInfo;

    [Header("Bars")]
    [SerializeField] private Image healthImage;
    [SerializeField] private Image manaImage;
    [SerializeField] private Image expImage;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coins;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI damageTextStat;
    [SerializeField] private TextMeshProUGUI defenseTextStat;
    [SerializeField] private TextMeshProUGUI expTextStat;
    [SerializeField] private TextMeshProUGUI levelTextStat;
    [SerializeField] private TextMeshProUGUI speedTextStat;
    [SerializeField] private TextMeshProUGUI criticalTextStat;
    [SerializeField] private TextMeshProUGUI blockTextStat;
    [SerializeField] private TextMeshProUGUI expRequiredTextStat;
    [SerializeField] private TextMeshProUGUI expTotalTextStat;
    [SerializeField] private TextMeshProUGUI availablePointsTextStat;
    [SerializeField] private TextMeshProUGUI strengthAttribute;
    [SerializeField] private TextMeshProUGUI dexterityAttribute;
    [SerializeField] private TextMeshProUGUI intelligenceAttribute;
    

    private float actualHealth;
    private float actualMana;
    private float actualExp;
    private float maxHealth;
    private float maxMana;
    private float expNextlevel;

    // Update is called once per frame
    void Update()
    {
        UpdateUICharacter();    
        UpdateStatsPanel();
    }

    private void UpdateUICharacter()
    {
        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, actualHealth / maxHealth, Time.deltaTime * 10);

        //String interpolation
        healthText.text = $"{actualHealth} / {maxHealth}";

        manaImage.fillAmount = Mathf.Lerp(manaImage.fillAmount, actualMana / maxMana, Time.deltaTime * 10);
        manaText.text = $"{actualMana} / {maxMana}";

        expImage.fillAmount = Mathf.Lerp(expImage.fillAmount, actualExp / expNextlevel, Time.deltaTime * 10);
        expText.text = $"{((actualExp/expNextlevel)*100):F2}%";
        levelText.text = $"Nivel {stats.Level}";
        coins.text = CoinManager.Instance.totalCoins.ToString();
    }

    private void UpdateStatsPanel()
    {
        if(panelStats.activeSelf == false)
        {
            return;
        }

        damageTextStat.text = stats.Damage.ToString();
        defenseTextStat.text = stats.Defense.ToString();
        expTextStat.text = stats.Experience.ToString();
        levelTextStat.text = stats.Level.ToString();
        speedTextStat.text = stats.Speed.ToString();
        criticalTextStat.text = $"{stats.Critical}%";
        blockTextStat.text = $"{stats.Block}%";
        expRequiredTextStat.text = stats.ExpRequired.ToString();
        expTotalTextStat.text = stats.ExpTotal.ToString();

        availablePointsTextStat.text = $"Available Points: {stats.availablePoints}";
        strengthAttribute.text = stats.strength.ToString();
        dexterityAttribute.text = stats.dexterity.ToString();
        intelligenceAttribute.text = stats.intelligence.ToString();
    }

    public void UpdateHealthCharacter(float pActualHealth, float pMaxHealth)
    {
        actualHealth = pActualHealth;
        maxHealth = pMaxHealth;

        //healthImage.fillAmount = actualHealth / maxHealth;
        //healthText.text = actualHealth + " / " + maxHealth;
    }

    public void UpdateManaCharacter(float pActualMana, float pMaxMana)
    {
        actualMana = pActualMana;
        maxMana = pMaxMana;

        //healthImage.fillAmount = actualHealth / maxHealth;
        //healthText.text = actualHealth + " / " + maxHealth;
    }

    public void UpdateExpCharacter(float pActualExp, float pExpRequired)
    {
        actualExp = pActualExp;
        expNextlevel = pExpRequired;

        //healthImage.fillAmount = actualHealth / maxHealth;
        //healthText.text = actualHealth + " / " + maxHealth;
    }

    #region Panels

        public void ShowStatsPanel()
        {
            panelStats.SetActive(!panelStats.activeSelf);
        }

        public void ShowInventoryPanel()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
        
        
        public void ShowShopPanel()
        {
            panelShop.SetActive(!panelShop.activeSelf);
        }

        public void ShowCraftanel()
        {
            panelCraft.SetActive(!panelCraft.activeSelf);
            ShowCraftInfoPanel(false);
        }


        public void ShowQuestCharacterPanel()
        {
            questsCharacterPanel.SetActive(!questsCharacterPanel.activeSelf);
        }

        public void ShowCraftInfoPanel(bool state)
        {
            panelCraftInfo.SetActive(state);
        }

        public void ShowQuestsPanel()
        {
        Debug.Log("abnrete sesamo");
            questsPanel.SetActive(!questsPanel.activeSelf);
        }

        /*
        public void ShowExtraInteraction(ExtraInteraction extra)
        {
            switch(extra)
            {
                case ExtraInteraction.Quest:
                    ShowQuestsPanel();
                    break;
                case ExtraInteraction.Shop:
                    ShowShopPanel();
                    break;
                case ExtraInteraction.Craft:
                    ShowCraftanel();
                    break;
               

            }
        }
        */

    #endregion 
}
