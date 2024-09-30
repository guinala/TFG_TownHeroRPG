using System;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : Singleton<GameManager>
{
    private GameStateSO currentState;

    [Header("States")] public GameStateSO[] states;
    
    [Header("Broadcasting events")]
    public Action<GameStateSO> gameStateChanged;

    private GameStateSO previousState;

    public void SetGameState(GameStateSO gameState)
    {
        if(this.currentState != null)
        {
            this.previousState = this.currentState;
        }

        this.currentState = gameState;

        if(this.gameStateChanged != null)
        {
            this.gameStateChanged.Invoke(this.currentState);
        }   

    }

    public void RestorePreviousState()
    {
        this.SetGameState(this.previousState);
    }
}