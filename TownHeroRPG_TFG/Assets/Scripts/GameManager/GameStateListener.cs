using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameStateListener : MonoBehaviour
{
    [Header("Enabled & Disabled Shortcuts")]
    public MonoBehaviour[] components;
    public List<GameStateSO> enabledStates;
    public List<GameStateSO> disabledStates;

    [Header("Actions")]
    public UnityEvent onMainMenuState;
    public UnityEvent onLoadingState;
    public UnityEvent onPlayingState;
    public UnityEvent onPauseState;
    public UnityEvent onDialogueState;
    public UnityEvent onCutsceneState;
    public UnityEvent onShoppingState;
    public UnityEvent onMissionState;
    public UnityEvent onStatsState;
    public UnityEvent onInventoryState;

    private void OnEnable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.gameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.gameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameStateSO newGameState)
    {
        InvokeShortcuts(newGameState);
        InvokeActions(newGameState);
    }

    private void InvokeShortcuts(GameStateSO newGameState)
    {
        foreach (var component in this.components)
        {
            if (this.enabledStates.Contains(newGameState))
            {
                component.enabled = true;
            }

            if (this.disabledStates.Contains(newGameState))
            {
                component.enabled = false;
            }
        }
    }

    private void InvokeActions(GameStateSO newGameState)
    {
        if (newGameState.stateName == "MainMenu" && this.onMainMenuState != null)
            this.onMainMenuState.Invoke();

        if (newGameState.stateName == "Loading" && this.onLoadingState != null)
            this.onLoadingState.Invoke();

        if (newGameState.stateName == "Playing" && this.onPlayingState != null)
            this.onPlayingState.Invoke();

        if (newGameState.stateName == "Paused" && this.onPauseState != null)
            this.onPauseState.Invoke();

        if (newGameState.stateName == "Dialogue" && this.onDialogueState != null)
            this.onDialogueState.Invoke();

        if (newGameState.stateName == "Cutscene" && this.onCutsceneState != null)
            this.onCutsceneState.Invoke();

        if (newGameState.stateName == "Shopping" && this.onShoppingState != null)
            this.onShoppingState.Invoke();

        if (newGameState.stateName == "OnMission" && this.onMissionState != null)
            this.onMissionState.Invoke();
        
        if (newGameState.stateName == "Stats" && this.onStatsState != null)
            this.onStatsState.Invoke();
        
        if (newGameState.stateName == "Inventory" && this.onInventoryState != null)
            this.onInventoryState.Invoke();
    }
}
