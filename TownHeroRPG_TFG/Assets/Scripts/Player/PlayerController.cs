using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    public float speed;

    [Header("Dependencies")]
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private Vector2 _movementInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = _movementInput * speed;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        _movementInput = value.ReadValue<Vector2>();
        Flip(_movementInput);
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

    /*
    private bool PlayerIsLookingLeft()
    {
        return spriteRenderer.flipX;
    }
    */


}