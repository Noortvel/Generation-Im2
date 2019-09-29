using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneScript : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
