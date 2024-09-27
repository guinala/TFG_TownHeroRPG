using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReceipeList", menuName = "ReceipeList")]
public class ReceipeList : ScriptableObject
{
    public List<Receipe> receipes;
}