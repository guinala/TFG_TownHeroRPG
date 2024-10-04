using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    public static Action DefeatedEvent;
    public bool canRestore => health < maxHealth;

    public bool Defeated { get; private set; }

    private CapsuleCollider2D _capsuleCollider2D;

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
        UpdateHealthBar(health, maxHealth);
        Debug.Log("La salud inicial es: " + health);
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


    protected override void CharacterDefeated()
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

    protected override void UpdateHealthBar(float actualHealth, float maxHealth)
    {
        UIManager.Instance.UpdateHealthBar(actualHealth, maxHealth);
    }

}
