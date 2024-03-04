using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem.Controls;

public class MovementController : MonoBehaviour
{
    DefaultInput defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;
    public float inputLean;
    public float inputJump;

    [Header("References")]
    public Transform camHolder;
    public CharacterController characterController;
    public Rigidbody rb;

    [Header("Camera")]
    public Vector2 cameraAngles;
    public float sensitivity = 1;
    public float FOV;
    public float AimFOV;

    [Header("Movement")]
    public bool isWalking;
    public Vector3 velocity;
    public float decel;
    public float accel;
    float maxSpeed;
    public float runSpeed;
    public float walkSpeed;

    [Header("Lean")]
    public Transform leanHolder;
    public float leanAngle;
    public float leanSpeed;

    [Header("Jump")]
    public bool grounded;
    public float gravity = 20f;
    public float jumpForce = 6.5f;
    public float bhopSpeedMult = 0.4f;
    public float airAccel;

    //public Vector3 airVelocity;
    //public float jumpHeight;
    //public bool isJumping;
    //public float jumpVelocity = 1;
    //public float jumpStartY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultInput = new DefaultInput();
        defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => inputJump = e.ReadValue<float>();
        defaultInput.Character.Jump.canceled += e => inputJump = e.ReadValue<float>();
        defaultInput.Character.Lean.performed += e => inputLean = e.ReadValue<float>();

        defaultInput.Enable();
    }
    private void Update()
    {
        CalculateView();

    }
    private void FixedUpdate()
    {
        DoGroundCheck();

        CalculateLean();
        if (inputJump >= 0.5f && grounded) DoJump();

        if (defaultInput.Character.Walk.ReadValue<float>() > 0.5f) isWalking = true;
        else isWalking = false;

        if (isWalking) maxSpeed = walkSpeed;
        else maxSpeed = runSpeed;

        if (!grounded)
        {
            CalculateAirMovement();
            velocity.y -= gravity * Time.deltaTime;
        }
        else CalculateGroundMovement();

        characterController.Move(velocity);
    }
    void DoGroundCheck()
    {
        Vector3 rayOrigin = new(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.DrawRay(rayOrigin, Vector3.down, Color.yellow);


        if (velocity.y > 0)
        {
            grounded = false;
            return;
        }
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1.1f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                velocity.y = 0;
                grounded = true;
            }
            else grounded = false;
        }
        else grounded = false;
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
        Vector3 wishDir = Vector3.Normalize(inputMovement.y * transform.forward + inputMovement.x * transform.right);

        float wishSpeed = wishDir.magnitude * maxSpeed / 100;

        //Calculate how much speed to add this frame
        float currentSpeed = Vector3.Dot(velocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
        //Add speed in wish direction 
        velocity.x += accelSpeed * wishDir.x;
        velocity.z += accelSpeed * wishDir.z;

        float speed = velocity.magnitude;
        float drop = speed * decel * Time.deltaTime;
        float newSpeed = Mathf.Max(speed - drop, 0);
        if (speed > 0) newSpeed /= speed;
        velocity.x *= newSpeed;
        velocity.z *= newSpeed;


        Debug.DrawRay(transform.position, wishDir, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, velocity * 100, Color.blue);
    }
    void CalculateAirMovement()
    {
        Vector3 wishDir = Vector3.Normalize(inputMovement.y * transform.forward + inputMovement.x * transform.right);
        float wishSpeed = Mathf.Min(wishDir.magnitude * maxSpeed / 100, 0.4f);

        var currentspeed = Vector3.Dot(velocity, wishDir);
        var addspeed = wishSpeed - currentspeed;
        var accelspeed = airAccel * wishSpeed * Time.deltaTime;
        accelspeed = Mathf.Min(accelspeed, addspeed);

        if (addspeed > 0)
        {
            velocity.x += (accelspeed * wishDir.x)*bhopSpeedMult;
            velocity.z += (accelspeed * wishDir.z)*bhopSpeedMult;
        }
    }
    private void CalculateLean()
    {
        //if (inputLean == 0) return;
        Quaternion tgt = Quaternion.identity;
        switch (inputLean)
        {
            case 0:
                break;
            case 1:
                tgt = Quaternion.Euler(0, 0, -leanAngle);
                break;
            case -1:
                tgt = Quaternion.Euler(0, 0, leanAngle);
                break;
        }

        if (leanHolder.localRotation != tgt)
        {
            leanHolder.localRotation = Quaternion.RotateTowards(leanHolder.localRotation, tgt, Time.deltaTime * leanSpeed);
        }

    }
    private Vector3 Move(Vector3 start, Vector3 tgt, float secs)
    {

        Vector3 diff = tgt - start;
        Vector3 move = diff * (secs / 50);
        Debug.Log(move);
        return move;
    }

    void DoJump()
    {
        velocity.y += jumpForce;
    }


}
