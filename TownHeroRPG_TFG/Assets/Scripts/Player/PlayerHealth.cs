using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] protected int initialHealth;
    [SerializeField] protected int maxHealth;

    public float health { get; protected set; }
    
    public static Action DefeatedEvent;
    public bool canRestore => health < maxHealth;

    public bool Defeated { get; private set; }

    private CapsuleCollider2D _capsuleCollider2D;

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    
    private void Start()
    {
        health = initialHealth;
        UpdateHealthBar(health, maxHealth);
    }
    
    public void AddDamage(float damage)
    {
        if(damage <= 0)
        {
            return;
        }

        if(health > 0)
        {
            health -= damage;
            Debug.Log("cosazas");
            UpdateHealthBar(health, maxHealth);

            if(health <= 0)
            {
                health = 0;
                
                UpdateHealthBar(health, maxHealth);
                CharacterDefeated();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            RestoreHealth(10);
        }
    }

    
    
    public void RestoreHealth(float cantidad)
    {
        if(Defeated)
        {
            return;
        }

        if (canRestore)
        {
            health += cantidad;
            if(health > maxHealth)
            {
                health = maxHealth;
            }

            UpdateHealthBar(health, maxHealth);
        }
    }

    public void RestoreCharacter()
    {
        _capsuleCollider2D.enabled = true;
        Defeated = false;
        health = initialHealth;
        UpdateHealthBar(health, initialHealth);
    }


    private void CharacterDefeated()
    {
        _capsuleCollider2D.enabled = false;
        Defeated = true;
        DefeatedEvent?.Invoke();

        //Esto es lo mismo que la linea de arriba
        /*
        if(DefeatedEvent != null)
        {
            DefeatedEvent.Invoke();
        }
        */
    }

    private void UpdateHealthBar(float actualHealth, float maxHealth)
    {
        UIManager.Instance.UpdateHealthBar(actualHealth, maxHealth);
    }

}
