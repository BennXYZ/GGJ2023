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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Test
        // Camera Up
        if (Input.GetKey(KeyCode.A) &&
            cameraUp.transform.position.x <= 9f && 
            cameraUp.transform.position.x >= -9f)
        {
            movementUp = new Vector3(-1,0f,0f);
            cameraUp.transform.Translate(movementUp * moveSpeed * Time.deltaTime);

            cameraUp.transform.position = new Vector3 (Mathf.Clamp( cameraUp.transform.position.x ,-9,9),cameraUp.transform.position.y, cameraUp.transform.position.z);
        }

        if (Input.GetKey(KeyCode.D) &&
            cameraUp.transform.position.x <= 9f && 
            cameraUp.transform.position.x >= -9f)
        {
            movementUp = new Vector3(1,0f,0f);
            cameraUp.transform.Translate(movementUp * moveSpeed * Time.deltaTime);

            cameraUp.transform.position = new Vector3 (Mathf.Clamp( cameraUp.transform.position.x ,-9,9),cameraUp.transform.position.y, cameraUp.transform.position.z);
        }




        // Camera Down
        if (Input.GetKey(KeyCode.LeftArrow) &&
            cameraDown.transform.position.x <= 9f && 
            cameraDown.transform.position.x >= -9f)
        {
            movementDown = new Vector3(-1,0f,0f);
            cameraDown.transform.Translate(movementDown * moveSpeed * Time.deltaTime);

            cameraDown.transform.position = new Vector3 (Mathf.Clamp( cameraDown.transform.position.x ,-9,9),cameraDown.transform.position.y, cameraDown.transform.position.z);
        }

            if (Input.GetKey(KeyCode.RightArrow) &&
            cameraDown.transform.position.x <= 9f && 
            cameraDown.transform.position.x >= -9f)
        {
            movementDown = new Vector3(1,0f,0f);
            cameraDown.transform.Translate(movementDown * moveSpeed * Time.deltaTime);

            cameraDown.transform.position = new Vector3 (Mathf.Clamp( cameraDown.transform.position.x ,-9,9),cameraDown.transform.position.y, cameraDown.transform.position.z);
        }




    }
}
