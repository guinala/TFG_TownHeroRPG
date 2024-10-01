using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component

{
    private static bool applicationIsQuitting = false;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();   
                
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
    
    public void OnDestroy()
    {
        Debug.Log("Gets destroyed");
        applicationIsQuitting = true;
    }

}
