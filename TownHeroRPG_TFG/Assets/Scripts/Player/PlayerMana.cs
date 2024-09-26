using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private float initialMana;
    [SerializeField] private float regenerationPerSecond;
    [SerializeField] private float maxMana;

    public float Mana { get; private set; }

    private PlayerHealth _characterHealth;

    public bool CanRestore => Mana < maxMana;

    private void Awake()
    {
        _characterHealth = GetComponent<PlayerHealth>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Mana = initialMana;
        UpdateManaBar();

        //Llamar a regenerar mana una vez cada segundo
        InvokeRepeating(nameof(RegenerateMana), 1, 1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            UseMana(10);
        }
    }


    public void RestoreManaItem(float amount)
    {
        if(Mana > maxMana)
        {
            return;
        }

        Mana += amount;

        if(Mana > maxMana)
        {
            Mana = maxMana;
        }

        //UIManager.Instance.UpdateManaCharacter(Mana, maxMana);
    }

    private void RegenerateMana()
    {
        if(_characterHealth.health > 0 && Mana < maxMana)
        {
            Mana += regenerationPerSecond;
            UpdateManaBar();
        }
    }

    public void UseMana(float mana)
    {
        if(Mana >= mana)
        {
            Mana -= mana;
            UpdateManaBar();
        }
    }

    public void RestoreMana()
    {
        Mana = initialMana;
        UpdateManaBar();
    }

    private void UpdateManaBar()
    {
        //UIManager.Instance.UpdateManaCharacter(Mana, maxMana);
    }

}
