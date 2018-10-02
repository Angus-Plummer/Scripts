using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenus : MonoBehaviour {

    // the GameManager functions that need to be called are static so require these wrapper functions for buttons to work properly
    public void Quit()
    {
        GameManager.QuitGame();
    }
    public void SelectLevel(int index)
    {
        GameManager.LoadScene(index);
    }
    public void SetColorScheme(ColorPalette color_palette)
    {
        GameManager.SetColorPalette(color_palette);
    }
    public void SetQuality(int quality_index)
    {
        QualitySettings.SetQualityLevel(quality_index);
    }
}
