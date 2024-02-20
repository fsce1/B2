using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class MovementController : MonoBehaviour
{
    private DefaultInput defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;

    [Header("References")]
    public Transform camHolder;
    [Header("SettingsTemp")]
    public float sensitivity = 1;

    [Header("Camera")]
    public Vector2 cameraAngles;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultInput = new DefaultInput();
        defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Enable();


    }

    // Update is called once per frame
    void Update()
    {
        CalculateView();
    }

    private void CalculateView()
    {
        
        inputView *= sensitivity / 10;
        cameraAngles += inputView;

        cameraAngles.y = Mathf.Clamp(cameraAngles.x, -90, 90);

        //Vector3 newCameraRotation = camHolder.localRotation.eulerAngles;

        //newCameraRotation += (Vector3.right * -inputView.y);
        //newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, 1, 90);
        //newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, 270, 90);
        //camHolder.localEulerAngles = newCameraRotation;

        
        camHolder.Rotate(Vector3.right * -inputView.y);
        transform.Rotate(Vector3.up, inputView.x);

        Debug.Log(camHolder.localEulerAngles.x);


        
    }
    private void CalculateMovement()
    {

    }
}
