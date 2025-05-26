using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public float speed;

    [Header("Dependencies")]
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D collider_AttackRightLeft;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
        rigidbody.velocity = Vector2.zero; // Stop movement when controls are disabled
    }

    private void Start()
    {
        _playerControls.PlayerActions.Movement.performed += OnMovement;
        _playerControls.PlayerActions.Movement.canceled += StopMovement;
        _playerControls.PlayerActions.Combat.started += _ => OnAttack();
    }
    private void OnAttack()
    {
        _animator.SetTrigger("Attack");
    }
    

    private void FixedUpdate()
    {
        rigidbody.velocity = _movementInput * speed;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        _movementInput = value.ReadValue<Vector2>();
        Debug.Log(_movementInput);;
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
            collider_AttackRightLeft.offset = new Vector2(0.37f, 0.41f);
        }
        else if (movementInput.x < 0f) // Moving to the left
        {
            spriteRenderer.flipX = true;
            collider_AttackRightLeft.offset = new Vector2(-0.37f, 0.41f);
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