using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    [Header("Fade For Loading Screen")]
    [SerializeField] private CanvasGroup fadeScreen;
    
    private SceneRequestSO _pendingRequest;
    private void OnEnable()
    {
        SceneLoader.loadSceneEvent += OnLoadLevelRequest;    
    }
    
    // Function that will be called from a listener
    public void OnLoadMenuRequest(SceneRequestSO request)
    {
        if (IsSceneAlreadyLoaded(request.scene) == false)
        {
            // Menus are loaded instantly
            SceneManager.LoadScene(request.scene.sceneName);
        }
    }

    // Function that will be called from a listener
    public void OnLoadLevelRequest(SceneRequestSO request)
    {
        if (IsSceneAlreadyLoaded(request.scene))
        {
           // Debug.Log("Cosazas");
            // Level is already loaded. Activate it
            ActivateLevel(request);
        }
        
        else
        {
            // Level is not loaded
            if (request.loadScene)
            {
                // If a loading screen is requested, then show it and wait
                this._pendingRequest = request;
                //this.loadingScreenUI.ToggleScreen(true);
                StartCoroutine(Fade(fadeScreen, 1, 1f));
            }
            else
            {
                // If no loading screen requested, load it ASAP
                StartCoroutine(ProcessLevelLoading(request));
            }
        }
    }
    
    public IEnumerator Fade(CanvasGroup canvasGroup, float valorDeseado, float tiempoFade)
    {
        float timer = 0;
        float valorInicial = canvasGroup.alpha;
        while (timer < tiempoFade)
        {
            canvasGroup.alpha = Mathf.Lerp(valorInicial, valorDeseado, timer / tiempoFade);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = valorDeseado;
        OnLoadingScreenToggled(true);
    }

    // Function that will be called from a listener
    public void OnLoadingScreenToggled(bool enabled)
    {
        if (enabled && this._pendingRequest != null)
        {
            // When loading screen is shown, we receive the event and can load the new level
            StartCoroutine(ProcessLevelLoading(this._pendingRequest));
        }
    }

    private bool IsSceneAlreadyLoaded(SceneSO scene)
    {
        Scene loadedScene = SceneManager.GetSceneByName(scene.name);

        if (loadedScene != null && loadedScene.isLoaded == true)
            return true;
        else
            return false;
    }

    private IEnumerator ProcessLevelLoading(SceneRequestSO request)
    {
        if (request.scene != null)
        {
            var currentLoadedLevel = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(currentLoadedLevel);

            AsyncOperation loadSceneProcess = SceneManager.LoadSceneAsync(request.scene.name, LoadSceneMode.Additive);

            // Level is being loaded, it could take some seconds (or not). Waiting until is fully loaded
            while (!loadSceneProcess.isDone)
            {
                yield return null;
            }

            // Once the level is ready, activate it!
            ActivateLevel(request);
        }
    }

    private void ActivateLevel(SceneRequestSO request)
    {
        // Set active
        var loadedLevel = SceneManager.GetSceneByName(request.scene.name);
        SceneManager.SetActiveScene(loadedLevel);

        // Hide black loading screen
        if (request.loadScene)
        {
            StartCoroutine(Helper.Fade(fadeScreen, 0, 1f));
        }

        // Clean status
        this._pendingRequest = null;
    }
}
