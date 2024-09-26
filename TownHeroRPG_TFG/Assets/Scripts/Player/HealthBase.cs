using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [SerializeField] protected int initialHealth;
    [SerializeField] protected int maxHealth;

    public float health { get; protected set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = initialHealth;
    }

    // Update is called once per frame
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


    protected virtual void UpdateHealthBar(float actualHealth, float maxHealth) //Virtual para que pueda ser sobreescrito
    {
        
    }

    protected virtual void CharacterDefeated()
    {
        
    }

}
