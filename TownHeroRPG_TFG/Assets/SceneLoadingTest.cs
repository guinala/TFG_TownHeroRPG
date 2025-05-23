using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingTest : MonoBehaviour
{
    public void LoadWFCScene()
    {
        SceneManager.LoadScene("WaveFunctionCollapse");
    }

    public void LoadInitScene()
    {
        SceneManager.LoadScene("SenseiHouse");
    }
}
