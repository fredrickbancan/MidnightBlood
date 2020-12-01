using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHelper : MonoBehaviour
{
    public static bool restarted = false;

    private void Start()
    {
        GameManagerAlpha.instance.ReloadPlayer();
        if (restarted)
        {
            GameManagerAlpha.instance.GetMenus();
            GameManagerAlpha.instance.mainMenu.SetActive(false);
            GameManagerAlpha.instance.gameOverMenu.SetActive(false);
            GameManagerAlpha.instance.pauseMenu.SetActive(false);
        }
    }

    public void OnStartButton()
    {
        GameManagerAlpha.instance.StartGame();
    }
    public void OnExitButton()
    {
        GameManagerAlpha.instance.ReturnToMenu();
    }
    public void OnResumeButton()
    {
        GameManagerAlpha.instance.ResumeGame();
    }
    public void OnQuitButton()
    {
        GameManagerAlpha.instance.QuitGame();
    }
}