using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
    public float damage = 5f;
    [SerializeField] private PlayerDamageSource playerDamageSource;
    /*
    public static Action<float, EnemyHealth> OnEnemyDamaged;
    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;

    public Weapon equipedWeapon { get; private set; }
    public EnemyInteraction enemySelectionated { get; private set; }
    
    private int indexShootPosition;
    private PlayerMana characterMana;
    private float timeToNextShot;

    [Header("Object Pooler")]
    [SerializeField] private ObjectPool objectPooler;

    [Header("Shooting Positions")]
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private Transform[] shootingPositions;

    public bool Attacking { get; set; }

    private void Awake()
    {
        characterMana = GetComponent<CharacterMana>();
    }

    private void Update()
    {
        ObtainMovementDirection();

        if(Time.time > timeToNextShot)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
               if(enemySelectionated == null || equipedWeapon == null)
                {
                    return;
                }

               UseWeapon();
               timeToNextShot = Time.time + timeBetweenShots;
               StartCoroutine(IESetAttackCondition());
            }
        }
    }

    public void EquipWeapon(WeaponItem item)
    {
        equipedWeapon = item.weapon;

        if (equipedWeapon.weaponType == WeaponType.Magic)
        {
            objectPooler.CreatePool(equipedWeapon.projectilePrefab.gameObject);
        }

        characterStats.AddBonusWeapon(equipedWeapon);
    }

    public void RemoveWeapon()
    {
        if (equipedWeapon == null) return;

        if (equipedWeapon.weaponType == WeaponType.Magic)
        {
            objectPooler.DestroyPool();
        }

        characterStats.RemoveBonusWeapon(equipedWeapon);

        equipedWeapon = null;
    }

    public float ObtainDamage()
    {
        float amount = characterStats.Damage;
        if(Random.value < characterStats.Critical)
        {
            amount *= 2;
        }

        return amount;
    }

    private void MeleeDetected(EnemyInteraction enemy)
    {
        if (equipedWeapon == null) return;
        if (equipedWeapon.weaponType != WeaponType.Melee) return;

        enemySelectionated = enemy;
        enemy.DisplaySelectedEnemy(true, DetectionType.Melee);
    }

    private void ObtainMovementDirection()
    {
        Vector2 input= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(input.x > 0.1f)
        {
            indexShootPosition = 1;
        }
        else if(input.x < 0f)
        {
            indexShootPosition = 3;
        }
        else if(input.y > 0.1f)
        {
            indexShootPosition = 0;
        }
        else if(input.y < 0f)
        {
            indexShootPosition = 2;
        }

    }

    private void UseWeapon()
    {
        Debug.Log("patata");
        if(equipedWeapon.weaponType == WeaponType.Magic)
        {
            if(characterMana.Mana < equipedWeapon.manaCost)
            {
                return;
            }

            GameObject projectile = objectPooler.ObtainInstance();
            projectile.transform.localPosition = shootingPositions[indexShootPosition].position;

            Projectile newProjectile = projectile.GetComponent<Projectile>();
            newProjectile.Initialize(this);

            projectile.SetActive(true);
            characterMana.UseMana(equipedWeapon.manaCost);
        }
        else
        {
            float damage = ObtainDamage();
            EnemyHealth health = enemySelectionated.GetComponent<EnemyHealth>();
            health.AddDamage(damage);
            OnEnemyDamaged?.Invoke(damage, health);

        }
    }

    private IEnumerator IESetAttackCondition()
    {
        Attacking = true;
        yield return new WaitForSeconds(0.5f);
        Attacking = false;
    }

    private void MeleeLost()
    {
        if (equipedWeapon == null) return;
        if (equipedWeapon.weaponType != WeaponType.Melee) return;
        if (enemySelectionated == null) return;

        enemySelectionated.DisplaySelectedEnemy(false, DetectionType.Melee);
        enemySelectionated = null;
    }

    private void OnEnable()
    {
        SelectionManager.EnemySeleccionated += AttackRangeSelect;
        SelectionManager.ObjectNotSeleccionated += NotSelectionated;
        CharacterDetector.EnemySeleccionated += MeleeDetected;
        CharacterDetector.EnemyLost += MeleeLost;
    }

    private void OnDisable()
    {
        SelectionManager.EnemySeleccionated -= AttackRangeSelect;
        SelectionManager.ObjectNotSeleccionated -= NotSelectionated;
        CharacterDetector.EnemySeleccionated -= MeleeDetected;
        CharacterDetector.EnemyLost -= MeleeLost;
    }

    private void AttackRangeSelect(EnemyInteraction enemy)
    {
        Debug.Log("si");
       if(equipedWeapon == null) return;
       if (equipedWeapon.weaponType != WeaponType.Magic) return;
       if (enemy == enemySelectionated) return;

       enemySelectionated = enemy;
       enemy.DisplaySelectedEnemy(true, DetectionType.Magic);
    }

    private void NotSelectionated()
    {
        if (enemySelectionated == null) return;

        enemySelectionated.DisplaySelectedEnemy(false, DetectionType.Magic);
        enemySelectionated = null;
    }
    */
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        ITakeDamage takeDamage = other.gameObject.GetComponent<ITakeDamage>();
        
        if(takeDamage != null)
        {
            takeDamage.ITakeDamage(damage);
            Debug.Log("Colision con: " + other.gameObject.name);
        }
    }
}
