using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public GameObject ball;
    public float pan_time;
    private Vector3 velocity = Vector3.zero;
    public float offset_fraction;
    private Vector3 offset;

    // Use this for initialization
    void Start ()
    {
        Camera cam = GetComponent<Camera>();
        float screen_height = Mathf.Abs(cam.ViewportToWorldPoint(new Vector3(1,1,1)).y);
        offset = new Vector3(0, screen_height * offset_fraction *-1f, -1);
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        PanCamera(ball.transform.position + offset);

	}

    void PanCamera(Vector3 target)
    {
        Vector3 new_pos = Vector3.SmoothDamp(transform.position, target, ref velocity, pan_time);
        transform.position = new_pos;
    }
}
