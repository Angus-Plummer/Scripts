using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script adjusts the camera so that the width of the camera view is the same for all aspect ratios (9:18 is default)
public class AspectRatioFixer : MonoBehaviour {

    Camera cam;
    float default_aspect = 9f / 18f;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        cam.orthographicSize = cam.orthographicSize * default_aspect / cam.aspect;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
