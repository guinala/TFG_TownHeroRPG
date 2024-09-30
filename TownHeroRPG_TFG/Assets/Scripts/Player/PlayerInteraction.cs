using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public static event Action<bool> OnInteract;
    
    [Header("Config")]
    [SerializeField] private string interactTag = "Interactable";

    private Interactable currentInteractable;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    
    private void Start()
    {
        playerControls.Enable();
        playerControls.PlayerActions.Interact.performed += ctx => EnableInteract();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(interactTag))
        {
            var interactable = other.GetComponent<Interactable>();
            this.currentInteractable = interactable;
            Debug.Log("Lo tengo");
        }
        
        OnInteract?.Invoke(currentInteractable != null);
        Debug.Log("Ultra cosas");
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(interactTag))
        {
            this.currentInteractable = null;
        }

        OnInteract?.Invoke(currentInteractable != null);
    }
    
    private void EnableInteract()
    {
        Debug.Log("Lo intento");
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            Debug.Log("empanadas");
        }
    }

}
