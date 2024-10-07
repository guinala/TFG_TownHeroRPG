using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, ITakeDamage
{
    public static event Action<Transform> onEnemyDefeated; 
    
    [Header("Config")]
    [SerializeField] private float health;

    private SpriteRenderer spriteRenderer;
    private float actualHealth;
    private FlashDamage flashDamage;
    private Animator animator;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashDamage = GetComponent<FlashDamage>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        actualHealth = health;
    }

    private void DamageColor()
    {
        StartCoroutine(flashDamage.Flash());
    }

    public void ITakeDamage(float damage)
    {
        actualHealth -= damage;
        DamageColor();
        animator.SetTrigger("Hit");
        if (actualHealth <= 0)
        {
            onEnemyDefeated?.Invoke(transform);
            Destroy(gameObject);
        }
    }

}
