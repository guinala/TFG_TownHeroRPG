using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExp : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;


    [Header("Config")]
    [SerializeField] private int maxLevel;
    [SerializeField] private int expBase; //Experiencia base requerida para subir de nivel
    [SerializeField] private int expIncrement;

    private float actualExp;
    private float expRequired;

    // Start is called before the first frame update
    void Start()
    {
        expRequired = expBase;
        stats.ExpRequired = expRequired;

        UpdateExpBar();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            AddExp(2);
        }
    }

    public void AddExp(float expObtained)
    {
        if (expObtained <= 0) return;
        actualExp += expObtained;
        stats.Experience = actualExp;

        if(actualExp == expRequired)
        {
            UpdateLevel();
        }
        else if (actualExp > expRequired)
        {
            float diference = actualExp - expRequired;
            UpdateLevel();
            AddExp(diference);
        }

        stats.ExpTotal += actualExp;
        UpdateExpBar();
    }


    private void UpdateLevel()
    {
        
        if(stats.Level < maxLevel)
        {
            stats.Level++;
            stats.Experience = 0;
            actualExp = 0;
            expRequired *= expIncrement;
            stats.ExpRequired = expRequired;
            stats.availablePoints += 3;
        }
        
    }

    private void UpdateExpBar()
    {
        UIManager.Instance.UpdateExpBar(actualExp, expRequired);
    }


    private void OnEnable()
    {
        //EnemyHealth.EnemyLooted += AddExpEnemy;
    }

    private void OnDisable()
    {
        //EnemyHealth.EnemyLooted -= AddExpEnemy;
    }

    private void AddExpEnemy(float exp)
    {
        AddExp(exp);
    }

}
