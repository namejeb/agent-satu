using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public int sceneToLoad;

    public void PlayGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}