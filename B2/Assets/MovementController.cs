using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem.Controls;

public class MovementController : MonoBehaviour
{
    DefaultInput defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;

    [Header("References")]
    public Transform camHolder;
    public CharacterController characterController;

    [Header("SettingsTemp")]
    public float sensitivity = 1;

    [Header("Camera")]
    public Vector2 cameraAngles;

    [Header("Movement")]
    public Vector3 velocity;
    public float friction;
    public float accel;
    public float maxSpeed = 0.7f;

    [Header("Jump")]
    public bool onGround;
    public Vector3 airVelocity;
    public float jumpHeight;
    public bool isJumping;
    public float jumpVelocity = 1;
    public float jumpStartY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultInput = new DefaultInput();
        defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => DoJump();
        defaultInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateView();
        CalculateGroundMovement();
        DoGroundCheck();
        if (!onGround) CalculateGravity();

        if (isJumping)
        {
            CalculateJump();
        }
    }
    void DoGroundCheck()
    {
        RaycastHit hit;
        Vector3 rayOrigin = new(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.DrawRay(rayOrigin, Vector3.down, Color.yellow);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1.05f))
        {
            airVelocity = Vector3.zero;
            if (hit.transform.CompareTag("Ground"))
            {
                onGround = true;
                //isJumping = false;
            }
        }
        else onGround = false;
    }
    private void CalculateView()
    {
        inputView *= sensitivity / 100;

        cameraAngles += inputView;
        cameraAngles.y = Mathf.Clamp(cameraAngles.y, -90, 90);

        Quaternion camRot = Quaternion.AngleAxis(-cameraAngles.y, Vector3.right);
        Quaternion playerRot = Quaternion.AngleAxis(cameraAngles.x, Vector3.up);
        camHolder.localRotation = camRot;
        transform.localRotation = playerRot;
    }
    private void CalculateGroundMovement()
    {
        //Vector3 wishDir = inputMovement * transform.forward;
        Vector3 wishDir = Vector3.Normalize(inputMovement.y * transform.forward + inputMovement.x * transform.right);

        float wishSpeed = wishDir.magnitude * maxSpeed / 100; // will turn into variable depending on crouch/sprint status

        //float currentSpeed = Vector3.Dot(velocity, wishDir);
        float addSpeed = wishSpeed - velocity.magnitude;
        float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
        velocity += accelSpeed * wishDir;

        float speed = velocity.magnitude;
        float drop = speed * friction * Time.deltaTime;

        float newSpeed = Mathf.Max(speed - drop, 0);
        if (speed > 0) newSpeed /= speed;
        velocity *= newSpeed;
        characterController.Move(velocity);


        Debug.Log(velocity);
        Debug.DrawRay(transform.position, wishDir, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, velocity * 100, Color.blue);
    }
    void CalculateGravity()
    {
        airVelocity.y -= 9.8f / 50;
        characterController.Move(airVelocity * Time.deltaTime);
    }
    void DoJump()
    {
        if (!onGround) return;
        jumpStartY = transform.position.y;
        isJumping = true;
    }
    void CalculateJump()
    {
        jumpVelocity /= 2;
        if(transform.position.y - jumpStartY >= jumpHeight - jumpStartY){
            isJumping = false;
            return;
        }

        characterController.Move(jumpVelocity * Time.deltaTime *Vector3.up);
    }
}
