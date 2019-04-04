using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameralook : MonoBehaviour {

    [Tooltip("Is the cursor hidden?")]
    public bool showCursor = false;
    public bool IsInverted = false;
    public Vector2 speed = new Vector2(120f, 120f);
    public float yMinLimit = -80f, yMaxlimit = 80f;

    public float x, y; // Current x and y degree of rotation
	// Use this for initialization
	void Start () {
        //Hide the cursor
        Cursor.visible = showCursor;
        //Lock the cursor
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        //Is the cursor supposed to be hidden?
        Vector3 angles = transform.eulerAngles;
        // Set X and Y degrees to current camera rotation
        x = angles.y; // Pitch (x) Yaw (y) Roll (z)
        y = angles.x;
        //Euler = Quaternion
	}
	
	// Update is called once per frame
	void Update () {
        //Get mouse offsets
        float mouseX = Input.GetAxis("Mouse X") * speed.x * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * speed.y * Time.deltaTime;
        // Check for inversion
        mouseY = IsInverted ? -mouseY : mouseY;
        //Rotate camera based on Mouse X and Y
        x += mouseX;
        y -= mouseY;
        //Clamp the angle of the pitch
        y = Mathf.Clamp(y, yMinLimit, yMaxlimit);
        //Rotate parent on Y aXis(yaw)
        transform.parent.rotation = Quaternion.Euler(0, x, 0);
        //Rotate local on X Axis(Pitch)
        transform.localRotation = Quaternion.Euler(y, 0, 0);
    }
}
