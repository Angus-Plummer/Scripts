using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// gives the attached object a velocity v = v_max ( 1 - e^(-t/k) )
public class ExponentialApproachVelocity : MonoBehaviour {

    public float v_start; // speed at start
    public float v_end; // final speed that is being approached
    public float time_const; // time for speed to apprach maximum by factor 1/e

    private float v_delta; // difference between start and end speeds

    public Vector2 direction;
    private Vector2 current_velocity = Vector2.zero;

    private float start_time; // time that the script was intialised

	// Use this for initialization
	void Start() {
        start_time = Time.time;
        v_delta = v_end - v_start;
	}
	
	// Update is called once per physics update
	void FixedUpdate () {
        float total_time_elapsed = Time.time - start_time;
        current_velocity = direction.normalized * (v_start + v_delta * (1 - Mathf.Exp(- total_time_elapsed / time_const)));
        GetComponent<Rigidbody2D>().velocity = current_velocity;
    }
}
