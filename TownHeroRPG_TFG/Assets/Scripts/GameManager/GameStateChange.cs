using UnityEngine;

public class GameStateChange : MonoBehaviour
{

    public void SetGameState(GameStateSO gameState)
    {
        GameManager.Instance.SetGameState(gameState);
    }
    
    public void RestorePreviousState()
    {
        GameManager.Instance.RestorePreviousState();
    }
}