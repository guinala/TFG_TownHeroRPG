using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private string LayerIdle;
    [SerializeField] private string LayerWalking;
    [SerializeField] private string LayerAttacking; 

    private Animator _animator;
    //private CharacterAttack _characterAttack;
    private readonly int directionX = Animator.StringToHash("MovementX");
    private readonly int directionY = Animator.StringToHash("MovementY");
    private readonly int defeated = Animator.StringToHash("Defeated");

    private Vector2 movementInput;
    private PlayerWeapon _playerWeapon;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void Update()
    {
        UpdateLayer();
    }
    
    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();

        _animator.SetFloat(directionX, movementInput.x);
        _animator.SetFloat(directionY, movementInput.y);
    }

    private void ActiveLayer(string layerName)
    {
        for (int i = 0; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0); //Peso 0 significa desactivar
        }

        _animator.SetLayerWeight(_animator.GetLayerIndex(layerName), 1); //Peso 1 significa activar
    }

    private void UpdateLayer()
    {
        
        if(_playerWeapon.Attacking)
        {
            Debug.Log("Activo esto");
            ActiveLayer(LayerAttacking);
        }
        
        else if(movementInput.x != 0 || movementInput.y != 0)
        {
            ActiveLayer(LayerWalking);
        }

        else
        {
            ActiveLayer(LayerIdle);
        }
    }

    public void ReviveCharacter()
    {
        ActiveLayer(LayerIdle);
        _animator.SetBool(defeated, false);
    }

    private void Defeated()
    {
        if(_animator.GetLayerWeight(_animator.GetLayerIndex(LayerIdle)) == 1)
             _animator.SetBool(defeated, true);

        else
        {
            ActiveLayer(LayerIdle);
            _animator.SetBool(defeated, true);
        }
    }

    private void OnEnable()
    {
        //CharacterHealth.DefeatedEvent += Defeated;
    }

    private void OnDisable()
    {
        //CharacterHealth.DefeatedEvent -= Defeated;
    }

}