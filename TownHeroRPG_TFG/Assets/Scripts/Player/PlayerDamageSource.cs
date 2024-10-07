using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageSource : MonoBehaviour
{
    public float damage = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ITakeDamage takeDamage = other.gameObject.GetComponent<ITakeDamage>();
        
        if(takeDamage != null)
        {
            takeDamage.ITakeDamage(damage);
            Debug.Log("ODIO MI VIDA");
        }
    }
}
