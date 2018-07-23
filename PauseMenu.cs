using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public GameObject PauseMenuUI;

    public void Resume()
    {
        GameManager.Resume();
        PauseMenuUI.SetActive(false);
    }

    public void Pause()
    {
        GameManager.Pause();
        PauseMenuUI.SetActive(true);
    }

    public void Quit()
    {
        GameManager.QuitGame();
    }

    public void Menu()
    {
        Resume();
        GameManager.LoadScene(0);
    }

    public void Restart()
    {
        Resume();
        GameManager.RestartScene();
    }
}
