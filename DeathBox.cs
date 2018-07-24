using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour {

    public bool wall_of_death;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (wall_of_death)
            {
                GameManager.RestartScene();
            }
            else
            {
                other.transform.GetComponent<BallHandler>().Respawn();
            }
        }
    }
}
