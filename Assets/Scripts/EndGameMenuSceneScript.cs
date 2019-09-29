using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameMenuSceneScript : MonoBehaviour
{
    private static EndGameMenuSceneScript _script;
    [SerializeField]
    private GameField gameField;
    [SerializeField]
    private UnityEngine.UI.Text messageBar;
    public void Awake()
    {
        _script = this;
        gameObject.SetActive(false);
    }
    public static void Show(string text)
    {
        _script.messageBar.text = text;
        _script.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        _script.messageBar.text = "";
        _script.gameObject.SetActive(false);
    }
    public void ToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void PlayAgain()
    {
        gameField.ReGeneratField();
    }
}
