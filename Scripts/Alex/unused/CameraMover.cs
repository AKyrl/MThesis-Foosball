using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMover : MonoBehaviour
{
    // By Simon Willcock
    // This is a basic C# Keyboard & Mouse control script for UNITY (tested on 2020.3.25f)

    // To use simply add it to your camera asset.
    // You can then set the amount of movement performed using the following variables -

    // 	keyboardMoveSpeed = 25.0f;
    // 	keyboardTurnSpeed = 5.0f;

    // 	mouseMoveSpeed = 50.0f;
    // 	mouseScrollSpeed = 20.0f;
    // 	mouseTurnSpeed = 4.0f;

    // 	minTurnAngle = -90.0f;
    // 	maxTurnAngle = 90.0f;

    // For keyboard support, it implements the following
    // 1) WASD keys with the up and down arrow keys for movement in the X,Y,Z-axis 
    // 2) Left and right arrows rotate in the Y (or left to right)

    // For the mouse support, it implements the following 
    // 1) When no mouse button is pressed it will DO NOTHING.
    // 2) Left mouse button pressed and the mouse moved for will rotate the camera.
    // 3) Right button pressed and the mouse moved will move the camera in X and Y-axis.
    // 4) Both buttons pressed causes movement in the Z-axis (or zooming in and out), the amount of movement is based on the Y movement of the mouse
    // 5) Scroll wheel pressed causes also causes zooming in and out on the Z-axis


    public float keyboardMoveSpeed = 50.0f;
    public float keyboardTurnSpeed = 5.0f;

    public float mouseMoveSpeed = 50.0f;
    public float mouseScrollSpeed = 20.0f;
    public float mouseTurnSpeed = 4.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    
    private float rotX;


    bool rightMouseBtnPress = false;
    bool leftMouseBtnPress = false;
  
    void Update()
    {
        MouseAiming();
        KeyboardMovement();
    }


    void MouseAiming()
    {
        // Left mouse button 
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button down");
            leftMouseBtnPress = true; // User is wanting to pan round 
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse button up");
            leftMouseBtnPress = false;
        }


        // Right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Mouse button down");
            rightMouseBtnPress = true; // The user is wanting to move 
        }
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Right Mouse button up");
            rightMouseBtnPress = false;
        }


        // If BOTH buttons pressed then move the camera forward/backward based on the Y mouse movement
        if (leftMouseBtnPress && rightMouseBtnPress )
        {
            // move forward
            Debug.Log($"Zoomng with left and right mouse buttons pressed ");
            Vector3 dir = new Vector3(-Input.GetAxis("Mouse X"), 0, mouseScrollSpeed * Input.GetAxis("Mouse Y"));
            transform.Translate(dir);
        } 
        else if (leftMouseBtnPress)
        {
            // Rotate the camera round 
            Debug.Log($"Rotating camera using mouse");
            float y = Input.GetAxis("Mouse X") * mouseTurnSpeed;
            rotX += Input.GetAxis("Mouse Y") * mouseTurnSpeed;
            // clamp the vertical rotation
            rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);
            // rotate the camera
            transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
        }
        else if (rightMouseBtnPress)
        {
            // Move the camera 
            Debug.Log($"Moving camera using mouse");
            Vector3 dir = new Vector3(0, 0, 0);
            dir.x = -Input.GetAxis("Mouse X");
            dir.y = Input.GetAxis("Mouse Y");
            transform.Translate(dir * mouseMoveSpeed * Time.deltaTime);
        }
        else
        {
            // Handle tthe scrollwheel
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Debug.Log($"Zoom in {Input.GetAxis("Mouse ScrollWheel")}");
                Vector3 dir = new Vector3(0, 0, mouseScrollSpeed * (Input.GetAxis("Mouse ScrollWheel")));
                transform.Translate(dir);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Debug.Log($"Zoom out {Input.GetAxis("Mouse ScrollWheel")}");
                Vector3 dir = new Vector3(0, 0, mouseScrollSpeed * (Input.GetAxis("Mouse ScrollWheel")));
                transform.Translate(dir);
            }
        }
    }
    void KeyboardMovement()
    {

        // -------------------------------------------------------------------------------------------------
        // Handle movement using the WASD keys
       
        // Detect any key presses 
        Vector3 pos = transform.position;
        if (Input.GetKey("w"))
        {
            pos.z += keyboardMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.z -= keyboardMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += keyboardMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= keyboardMoveSpeed * Time.deltaTime;
        }

        // This is an alternative way of handling the WASD keys

        //  Vector3 dir = new Vector3(0, 0, 0);
        //  dir.x = Input.GetAxis("Horizontal");
        //  dir.z = Input.GetAxis("Vertical");
        //  transform.Translate(dir * keyboardMoveSpeed * Time.deltaTime);

        // Handle up and down arrows
        // Comment these lines out if you want to use the up and down arrow keys for rotation instead 
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pos.y += keyboardMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pos.y -= keyboardMoveSpeed * Time.deltaTime;
        }

        // Set the position
        transform.position = pos;

        // -------------------------------------------------------------------------------------------------


        // -------------------------------------------------------------------------------------------------
        // Handle the  arrow keys for rotation

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, keyboardTurnSpeed, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, -keyboardTurnSpeed, 0);
        }

        // if you want to use the up and down arrow keys for rotation instead  then uncomment these lines and
        // comment out the up and down arrow keys above
        // 
        //  if (Input.GetKey(KeyCode.UpArrow))
        //  {
        //        transform.Rotate(keyboardTurnSpeed, 0, 0);
        //  }

        //  if (Input.GetKey(KeyCode.DownArrow))
        //  {
        //        transform.Rotate(-keyboardTurnSpeed, 0, 0);
        //  }

        // -------------------------------------------------------------------------------------------------

    }
}