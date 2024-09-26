using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Stats")]
    public float Damage = 5f;
    public float Level;
    public float Defense = 2f;
    public float Speed = 5f;
    public float Experience;
    public float ExpRequired;
    public float ExpTotal;
    [Range(0, 100f)] public float Critical;
    [Range(0, 100f)] public float Block;

    [Header("Attributes")]
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;

    [HideInInspector] public int availablePoints = 0;

    public void addStrengthBonusAttribute()
    {
        Damage += 2;
        Defense += 1;
        Block += 0.03f;
    }

    public void addIntelligenceBonusAttribute()
    {
        Defense += 2;
        Block += 0.2f; 
    }

    public void addDexterityBonusAttribute()
    {
        Damage += 2.5f;
        Speed += 2;
        Critical += 0.1f;
    }

    /*
    public void AddBonusWeapon(Weapon weapon)
    {
        Damage += weapon.damage;
        Block += weapon.BlockChance;
        Critical += weapon.CriticalChance;
    }

    public void RemoveBonusWeapon(Weapon weapon)
    {
        Damage -= weapon.damage;
        Block -= weapon.BlockChance;
        Critical -= weapon.CriticalChance;
    }
    */

    public void ResetStats()
    {
        Damage = 5f;
        Level = 1;
        Defense = 2f;
        Speed = 5f;
        Experience = 0f;
        ExpRequired = 0f;
        ExpTotal = 0f;
        Critical = 0f;
        Block = 0f;

        strength = 0;
        dexterity = 0;
        intelligence = 0;

        availablePoints = 0;
    }
}

