using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera cameraUp;
    public Camera cameraDown;


    float moveSpeed = 4f;

    Vector3 movementUp;
    Vector3 movementDown;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.AssignButton("B", 1, BButtonPressed);
    }

    void BButtonPressed()
    {

    }

    void UpdateCameraMovement(Camera camera, Vector2 movement)
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Test
        // Camera Up
        movementUp = new Vector3(InputManager.Instance.GetRightJoystick(1).x, 0f);
        cameraUp.transform.Translate(movementUp * moveSpeed * Time.deltaTime);
        cameraUp.transform.position = new Vector3(Mathf.Clamp(cameraUp.transform.position.x, -9, 9), cameraUp.transform.position.y, cameraUp.transform.position.z);

        // Camera Down
        movementDown = new Vector3(InputManager.Instance.GetRightJoystick(2).x, 0f);
        cameraDown.transform.Translate(movementDown * moveSpeed * Time.deltaTime);
        cameraDown.transform.position = new Vector3(Mathf.Clamp(cameraDown.transform.position.x, -9, 9), cameraDown.transform.position.y, cameraDown.transform.position.z);
    }
}
