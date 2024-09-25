using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public bool Attacking {
        get;
        private set;
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        StartCoroutine(AttackAnim());
    }

    private IEnumerator AttackAnim()
    {
        Attacking = true;
        yield return new WaitForSeconds(1f);
        Attacking = false;
    }
}
