using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRequest", menuName = "ScriptableObjects/Scenes/SceneRequest")]
public class SceneRequestSO : ScriptableObject
{
    public SceneSO scene;
    public bool loadScene;
    
    public void SetRequest (SceneSO scene, bool loadScene)
    {
        this.scene = scene;
        this.loadScene = loadScene;
    }
}
