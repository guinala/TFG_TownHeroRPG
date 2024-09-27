using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    [SerializeField] private int testCoins = 100;

    public int totalCoins { get; set; }

    private string coinKey = "COINS";


    private void Start()
    {
        //Para testar o sistema de moedas
        PlayerPrefs.DeleteKey(coinKey);
        LoadCoins();
    }
    
    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt(coinKey, testCoins);
    }

    public void AddCoins(int coins)
    {
        totalCoins += coins;
        PlayerPrefs.SetInt(coinKey, totalCoins);
        PlayerPrefs.Save();
    }

    public void RemoveCoins(int coins)
    {
        if(totalCoins < coins) return;
        totalCoins -= coins;
        PlayerPrefs.SetInt(coinKey, totalCoins);
        PlayerPrefs.Save();
    }
}