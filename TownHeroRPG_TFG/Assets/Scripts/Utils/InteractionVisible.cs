using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionVisible : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();    
    }
    
    private void OnEnable()
    {
        PlayerInteraction.OnInteract += EnableImage;
    }

    private void EnableImage(bool enable)
    {
        image.enabled = enable;
    }

}
