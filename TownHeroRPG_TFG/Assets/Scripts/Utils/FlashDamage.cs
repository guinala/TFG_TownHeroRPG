using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashDamage : MonoBehaviour
{
    [SerializeField] private Material Material;
    [SerializeField] private float restoreTime = .2f;

    private Material defaultMaterial;
    private SpriteRenderer _spriteRenderer;
    

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = _spriteRenderer.material;
    }

    public float GetRestoreTime()
    {
        return restoreTime;
    }

    public IEnumerator Flash()
    {
        _spriteRenderer.material = Material;
        yield return new WaitForSeconds(restoreTime);
        _spriteRenderer.material = defaultMaterial;
    }
}