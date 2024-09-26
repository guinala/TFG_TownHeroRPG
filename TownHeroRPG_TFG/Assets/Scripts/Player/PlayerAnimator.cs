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
    private PlayerControls _playerControls;
    private bool Attacking;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _animator = GetComponent<Animator>();
        _playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void Update()
    {
        UpdateLayer();
    }

    private void Start()
    {
        _playerControls.Enable();
        _playerControls.PlayerActions.Attack.started += _ => OnAttack();
    }
    
    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();

        _animator.SetFloat(directionX, movementInput.x);
        _animator.SetFloat(directionY, movementInput.y);
    }

    private void OnAttack()
    {
        StartCoroutine(AttackRoutine());
    }
    
    private IEnumerator AttackRoutine()
    {
        Attacking = true;
        ActiveLayer(LayerAttacking);
        yield return new WaitForSeconds(0.6f);
        ActiveLayer(LayerIdle);
        Attacking = false;
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
        /*
        if(_playerWeapon.Attacking)
        {
            Debug.Log("Activo esto");
            ActiveLayer(LayerAttacking);
        }
        */
        
        if((movementInput.x != 0 || movementInput.y != 0) && Attacking == false) 
        {
            ActiveLayer(LayerWalking);
        }

        else if(Attacking == false)
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
