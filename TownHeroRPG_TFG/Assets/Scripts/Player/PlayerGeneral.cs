using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    public PlayerWeapon PlayerWeapon { get; set; }
    public PlayerExp PlayerExp { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public PlayerMana PlayerMana { get; private set; }


    private void Awake()
    {
        PlayerWeapon = GetComponent<PlayerWeapon>();
        PlayerHealth = GetComponent<PlayerHealth>();
        PlayerController = GetComponent<PlayerController>();
        PlayerMana = GetComponent<PlayerMana>();
        PlayerExp = GetComponent<PlayerExp>();
    }

    public void RestoreCharacter()
    {
        PlayerHealth.RestoreCharacter();
        PlayerController.ReviveCharacter();
        PlayerMana.RestoreMana();
    }


    public void OnEnable()
    {
        ButtonAttribute.attributeEvent += AddAttribute;
    }

    public void OnDisable()
    {
        ButtonAttribute.attributeEvent -= AddAttribute;
    }
    

    private void AddAttribute(AttributeType attributeType)
    {
        if(stats.availablePoints <= 0)
        {
            return;
        }
        

        switch (attributeType)
        {
            
            case AttributeType.Strength:
                //CharacterHealth.CharacterStats.strength++;
                stats.strength++;
                stats.addStrengthBonusAttribute();
                
                break;
            case AttributeType.Dexterity:
                //CharacterHealth.CharacterStats.dexterity++;
                stats.dexterity++;
                stats.addDexterityBonusAttribute();
                break;
            case AttributeType.Intelligence:
                //CharacterHealth.CharacterStats.intelligence++;
                stats.intelligence++;
                stats.addIntelligenceBonusAttribute();
                break;
        }
        stats.availablePoints--;
    }
}
