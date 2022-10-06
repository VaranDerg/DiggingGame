using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtoSceneSwap : MonoBehaviour
{
    private int _currentScene = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_currentScene == 0)
            {
                _currentScene++;
                SceneManager.LoadScene(_currentScene);
            }
            else if (_currentScene == 1)
            {
                _currentScene--;
                SceneManager.LoadScene(_currentScene);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(_currentScene);
        }
    }
}
