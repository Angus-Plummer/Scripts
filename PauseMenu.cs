using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public GameObject pause_menu_ui;
    public GameObject wall_of_death;

    public void Resume()
    {
        GameManager.Resume();
        pause_menu_ui.SetActive(false);
    }

    public void Pause()
    {
        GameManager.Pause();
        pause_menu_ui.SetActive(true);
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
