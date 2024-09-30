using System;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [Header("Configuration")]
    public SceneSO sceneToLoad;
    public LevelEntranceSO levelEntrance;
    public bool loadingScreen;
    [SerializeField] private SceneRequestSO sceneRequest;

    [Header("Player Path")]
    public PlayerPathSO playerPath;

    public static Action<SceneRequestSO> loadSceneEvent;
    
    //[SerializeField] private MusicArea musicArea;

    public void LoadScene()
    {
        if (this.levelEntrance != null && this.playerPath != null)
            this.playerPath.levelEntrance = this.levelEntrance;

        sceneRequest.SetRequest(
            scene: this.sceneToLoad,
            loadScene: this.loadingScreen
        );

        Debug.Log("He enviado el evento");
        loadSceneEvent?.Invoke(sceneRequest);
        //AudioManager.instance.setMusicArea(musicArea);
        
    }
}