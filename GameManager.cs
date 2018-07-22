using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        StartCoroutine(PauseForSeconds(1f));
    }

    // Triggers the proccesses that occur on player death
    public void TriggerDeath()
    {
        // TEMPORARY SOLUTION TO DEATH AND RESPAWNING. MUST FIX.
        Scene current_scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current_scene.name);

    }

    // Coroutine for pausing the game for a number of seconds
    private IEnumerator PauseForSeconds(float pause_duration)
    {
        float originalTimeScale = Time.timeScale; // store original time scale in case it was not 1
        Time.timeScale = 0; // pause
        yield return new WaitForSecondsRealtime(pause_duration);
        Time.timeScale = originalTimeScale; // restore time scale from before pause
    }
}
