using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LoadWFCData : ScriptableObject
{
    public bool loadData;

    public void SetNewGame()
    {
        loadData = false;
    }
}
