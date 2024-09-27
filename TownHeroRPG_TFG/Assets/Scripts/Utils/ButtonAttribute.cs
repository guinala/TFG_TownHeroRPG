using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence
}

public class ButtonAttribute : MonoBehaviour
{
    public static Action<AttributeType> attributeEvent;
    [SerializeField] private AttributeType attributeType;

    public void OnClick()
    {
        attributeEvent?.Invoke(attributeType);
        Debug.Log("Evento enviado");
    }
}