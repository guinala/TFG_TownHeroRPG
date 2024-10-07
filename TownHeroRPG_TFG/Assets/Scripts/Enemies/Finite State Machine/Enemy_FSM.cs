using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FSM : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string initialStateID;

    [Header("Estados")]
    public List<Enemy_State> states; 

    public Enemy_State ActualState { get; private set; }
    //public Room RoomParent { get; set; }
    //public Transform Player { get; set; }
    
    private void Start()
    {
        ChangeState(initialStateID);
    }

    private void Update()
    {
        if (ActualState == null) return;
        ActualState.ExecuteState(this); 
    }

    public void ChangeState(string newID)
    {
        if (ActualState == null)
        {
            ActualState = SearchState(newID);
        }
        
        if (ActualState.stateID == newID) return;
        ActualState = SearchState(newID);
    }

    private Enemy_State SearchState(string wantedID)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].stateID == wantedID)
            {
                return states[i];
            }
        }

        return null;
    }

}
