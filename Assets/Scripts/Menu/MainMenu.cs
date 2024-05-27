using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button LoadGameButton;

    private void Start()
    {

    }
    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");

    }

    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();

    }
}
