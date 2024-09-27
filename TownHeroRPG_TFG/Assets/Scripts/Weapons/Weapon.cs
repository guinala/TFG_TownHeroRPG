using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Magic,
    Melee
}

[CreateAssetMenu(menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Config")]
    public Sprite iconWeapon;
    public Sprite skillIcon;
    public WeaponType weaponType;
    public float damage;

    [Header("Magic Weapon")]
    public Projectile projectilePrefab;
    public float manaCost;

    [Header("Stats")]
    public float BlockChance;
    public float CriticalChance;

}