using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters", menuName = "Dungeon/RandomWalkParameters")]
public class RandomWalkParameters : ScriptableObject
{
    [Header("Parameters")]
    public bool randomIterations = true;
    public int iterations = 5;
    public int walkLength = 5;
}
