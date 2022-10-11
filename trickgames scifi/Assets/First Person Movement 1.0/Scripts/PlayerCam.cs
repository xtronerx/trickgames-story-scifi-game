using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;
    public float multiplier;

    public float maxLookUpRotation = 80;
    public float maxLookDownRotation = 80;

    public Transform playerOrientation;
    public Transform camHolder;

 

    float xRotation;
    float yRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        yRotation += mouseX * multiplier;

        xRotation -= mouseY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -maxLookUpRotation, maxLookDownRotation);

        // rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

}