using UnityEngine;
using UnityEngine.Events;

public class TriggerChecker : MonoBehaviour
{
    [Header("Extra config")]
    public string validTag;

    private bool invoked = false;

    [Header("Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invoked) return; // Prevent multiple invocations
        if (collision.CompareTag(validTag))
        {
            Debug.Log("Detectado");
            if (onTriggerEnter != null)
            {
                onTriggerEnter.Invoke();
                invoked = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(validTag))
        {
            if (onTriggerStay != null)
                onTriggerStay.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(validTag))
        {
            if (onTriggerExit != null)
                onTriggerExit.Invoke();
        }
    }
}