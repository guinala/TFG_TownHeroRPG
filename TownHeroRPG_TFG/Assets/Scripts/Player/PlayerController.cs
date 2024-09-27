using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public float speed;

    [Header("Dependencies")]
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    private PlayerControls _playerControls;

    //private CharacterAttack _characterAttack;
    private readonly int directionX = Animator.StringToHash("MovementX");
    private readonly int directionY = Animator.StringToHash("MovementY");
    private readonly int defeated = Animator.StringToHash("Defeated");
    private readonly int walking = Animator.StringToHash("Walking");
    
    private Vector2 _movementInput;
    private bool Walking;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _playerControls.Enable();
        _playerControls.PlayerActions.Movement.performed += OnMovement;
        _playerControls.PlayerActions.Movement.canceled += StopMovement;
        _playerControls.PlayerActions.Attack.started += _ => OnAttack();
    }
    private void OnAttack()
    {
        _animator.SetTrigger("Attack");
    }
    

    private void FixedUpdate()
    {
        rigidbody.velocity = _movementInput * speed;
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        _movementInput = value.ReadValue<Vector2>();
        Flip(_movementInput);
        _animator.SetFloat(directionX, _movementInput.x);
        _animator.SetFloat(directionY, _movementInput.y);
        Walking = true;
        _animator.SetBool(walking, true);
    }

    private void StopMovement(InputAction.CallbackContext value)
    {
        _movementInput = Vector2.zero;
        Walking = false;
        _animator.SetBool(walking, false);
    }
    
    private void Flip(Vector2 movementInput)
    {
        if (movementInput.x > 0f) // Moving to the right
        {
            spriteRenderer.flipX = false;
        }
        else if (movementInput.x < 0f) // Moving to the left
        {
            spriteRenderer.flipX = true;
        }
    }
    
    public void ReviveCharacter()
    {
        //ActiveLayer(LayerIdle);
        _animator.SetBool(defeated, false);
    }

    private void Defeated()
    {
        //if(_animator.GetLayerWeight(_animator.GetLayerIndex(LayerIdle)) == 1)
        _animator.SetBool(defeated, true);
        /*
        else
        {
            ActiveLayer(LayerIdle);
            _animator.SetBool(defeated, true);
        }
        */
    }

    /*
    private bool PlayerIsLookingLeft()
    {
        return spriteRenderer.flipX;
    }
    */


}