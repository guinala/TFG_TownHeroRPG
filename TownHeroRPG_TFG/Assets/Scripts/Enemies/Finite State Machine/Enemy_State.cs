using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Enemy_State 
{
    public string stateID;
    public List<Enemy_Action> Actions;
    public List<Enemy_Transition> Transitions;

    public void ExecuteState(Enemy_FSM enemyFSM)
    {
        ExecuteActions();
        ExecuteTransitions(enemyFSM);
    }

    private void ExecuteActions()
    {
        if (Actions.Count <= 0) return;
        for (int i = 0; i < Actions.Count; i++)
        {
            Actions[i].ExecuteAction();
        }
    }

    private void ExecuteTransitions(Enemy_FSM enemyFSM)
    {
        if (Transitions.Count <= 0) return;
        for (int i = 0; i < Transitions.Count; i++)
        {
            bool respuesta = Transitions[i].Decision.Choose(enemyFSM);
            if (respuesta)
            {
                if (string.IsNullOrEmpty(Transitions[i].State_True) == false)
                {
                    enemyFSM.ChangeState(Transitions[i].State_True);
                    break;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Transitions[i].State_False) == false)
                {
                    enemyFSM.ChangeState(Transitions[i].State_False);
                    break;
                }
            }
        }
    }

}
