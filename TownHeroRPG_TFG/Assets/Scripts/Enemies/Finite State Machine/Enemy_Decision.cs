using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Decision : MonoBehaviour
{
    public abstract bool Choose(Enemy_FSM enemy);
}

