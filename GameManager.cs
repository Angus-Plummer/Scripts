﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public ScreenOrientation screen_orientation;
    private static ColorPalette current_color_palette;
    public static bool game_is_paused = false;
    private static float timescale_at_pause = 1f;

	// Use this for initialization
	void Awake ()
    {
        UpdateColorPalette();
        Screen.orientation = screen_orientation;
        // set quality level to same as in prefs if it has been changed from default
        if(PlayerPrefs.HasKey("Quality Level"))
        {
            int quality_level = PlayerPrefs.GetInt("Quality Level");
            QualitySettings.SetQualityLevel(quality_level);
        }

    }

    public static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public static void Resume()
    {
        Time.timeScale = timescale_at_pause;
        game_is_paused = false;
    }

    public static void Pause()
    {
        timescale_at_pause = Time.timeScale;
        Time.timeScale = 0f;
        game_is_paused = true;
    }

    // Coroutine for pausing the game for a number of seconds
    private static IEnumerator PauseForSeconds(float pause_duration)
    {
        float originalTimeScale = Time.timeScale; // store original time scale in case it was not 1
        Time.timeScale = 0; // pause
        yield return new WaitForSecondsRealtime(pause_duration);
        Time.timeScale = originalTimeScale; // restore time scale from before pause
    }

    public static void SetColorPalette(ColorPalette color_palette)
    {
        PlayerPrefs.SetString("Color Palette", color_palette.name);
        UpdateColorPalette();
    }

    public static ColorPalette GetColorPalette()
    {
        return current_color_palette;
    }

    public static void UpdateColorPalette()
    {
        string palette_name;
        if (PlayerPrefs.HasKey("Color Palette"))
        {
            palette_name = PlayerPrefs.GetString("Color Palette");
        }
        else
        {
            palette_name = "Soft Red";
            PlayerPrefs.SetString("Color Palette", palette_name);
        }
        
        string palette_path = "Colors/Color Palettes/" + palette_name;
        current_color_palette = (ColorPalette)Resources.Load(palette_path);
    }
}
