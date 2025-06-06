using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, ITakeDamage
{
    public static event Action<Transform> onEnemyDefeated;

    [Header("Item Death")]
    [SerializeField] private GameObject itemDeathPrefab;

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
            animator.SetTrigger("Death");
            //Destroy(gameObject);
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false; // Disable collider to prevent further interactions
            }
            if (itemDeathPrefab != null)
            {
                Invoke("InstantiateItemDeath", 1.5f); // Delay to allow death animation to play
            }
        }
    }

    private void InstantiateItemDeath()
    {
        if (itemDeathPrefab != null)
        {
            Instantiate(itemDeathPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // Destroy the enemy after instantiating item death
    }

}
