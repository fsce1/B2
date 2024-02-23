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

    [Header("References")]
    public Transform camHolder;
    public CharacterController characterController;
    public Rigidbody rb;

    [Header("SettingsTemp")]
    public float sensitivity = 1;

    [Header("Camera")]
    public Vector2 cameraAngles;

    [Header("Movement")]
    public bool isWalking;
    public Vector3 velocity;
    public float friction;
    public float accel;
    public float maxSpeed = 0.7f;
    public float runSpeed;
    public float walkSpeed;

    [Header("Lean")]
    public Transform leanHolder;
    public float leanAngle;
    public float leanSpeed;

    //[Header("Jump")]
    //public bool onGround;
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
        //defaultInput.Character.Jump.performed += e => DoJump();
        defaultInput.Character.Lean.performed += e => inputLean = e.ReadValue<float>();

        defaultInput.Enable();
    }
    private void Update()
    {
        DoGroundCheck();
        CalculateGroundMovement();
        CalculateView();
        CalculateLean();

        if (defaultInput.Character.Walk.ReadValue<float>() > 0.5f)
        {
            isWalking = true;
        }
        else isWalking = false;

        if (isWalking)
        {
            maxSpeed = walkSpeed;
        }
        else maxSpeed = runSpeed;
    }
    void DoGroundCheck()
    {
        RaycastHit hit;
        Vector3 rayOrigin = new(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.DrawRay(rayOrigin, Vector3.down, Color.yellow);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1.05f))
        {
            //airVelocity = Vector3.zero;
            if (hit.transform.CompareTag("Ground"))
            {
                //onGround = true;
                //isJumping = false;
            }
        }
        //else onGround = false;
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

        float wishSpeed = wishDir.magnitude * maxSpeed / 100; // will be variable later depending on crouch/sprint status

        float currentSpeed = Vector3.Dot(velocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        //addSpeed = Mathf.Max(Mathf.Min(addSpeed, accel * Time.deltaTime), 0);
        float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
        //float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
        Debug.Log(accelSpeed);
        velocity += accelSpeed * wishDir;

        float speed = velocity.magnitude;
        float drop = speed * friction * Time.deltaTime;


        float newSpeed = Mathf.Max(speed - drop, 0);
        if (speed > 0) newSpeed /= speed;
        velocity *= newSpeed;
        velocity.y = 0;
        characterController.Move(velocity);
        //rb.velocity += velocity;

        Debug.DrawRay(transform.position, wishDir, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, velocity * 100, Color.blue);
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
        //Vector3 tgt = new(0, 0, 90);
        //tgt.z = -inputLean * leanAngle + 90;
        if (leanHolder.localRotation != tgt)
        {
            leanHolder.localRotation = Quaternion.RotateTowards(leanHolder.localRotation, tgt, Time.deltaTime * leanSpeed);
        }
        //if (Vector3Int.RoundToInt(leanHolder.localEulerAngles) != tgt)
        //{
        //    leanHolder.localEulerAngles += Move(leanHolder.localEulerAngles, tgt, leanSpeed);
        //}
    }
    private Vector3 Move(Vector3 start, Vector3 tgt, float secs)
    {

        Vector3 diff = tgt - start;
        Vector3 move = diff * (secs / 50);
        Debug.Log(move);
        return move;
    }
    //void CalculateGravity()
    //{
    //    airVelocity.y -= 9.8f / 50;
    //    //characterController.Move(airVelocity * Time.deltaTime);
    //}
    //void DoJump()
    //{
    //    if (!onGround) return;
    //    jumpStartY = transform.position.y;
    //    isJumping = true;
    //}
    //void CalculateJump()
    //{
    //    jumpVelocity /= 2;
    //    if(transform.position.y - jumpStartY >= jumpHeight - jumpStartY){
    //        isJumping = false;
    //        return;
    //    }

    //    //characterController.Move(jumpVelocity * Time.deltaTime *Vector3.up);
    //}
}
