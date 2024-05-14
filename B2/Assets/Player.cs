using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool isDead;
    public float regenHealthSpeed;
    public float regenStaminaSpeed;
    DefaultInput defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;
    public Vector2 accumulatedInputView;
    public float inputLean;
    public float inputJump;
    public bool inputSprint;
    public float inputFreelook;

    public float health = 100;
    public float stamina = 100;


    [Header("References")]
    public CharacterController characterController;
    public Transform camHolder;
    public Rigidbody rb;
    public SwayController swayController;
    public Firearm firearm;

    [Header("Camera")]
    public Vector2 cameraAngles;
    public Vector2 freelookAngles;
    public float sensitivity;
    public float baseSensitivity;
    public float FOV;
    public float AimFOV;
    public Vector3 camRecoil;

    [Header("Movement")]
    public bool isWalking;
    public bool isSprinting;
    public Vector3 velocity;
    public float decel;
    public float accel;
    public float maxSpeed;
    public float sprintSpeed;
    public float runSpeed;
    public float walkSpeed;

    [Header("Lean")]
    public Transform leanHolder;
    public float leanAngle;
    public float leanSpeed;

    [Header("Jump")]
    public bool hasJumped = false;
    public float gravity = 20f;
    public float jumpForce = 6.5f;
    public float bhopSpeedMult = 0.4f;
    public float airAccel;

    //public Vector3 airVelocity;
    //public float jumpHeight;
    //public bool isJumping;
    //public float jumpVelocity = 1;
    //public float jumpStartY;
    void RegenHealth()
    {
        health++;
        if (health > 100) health = 100;


    }
    void RegenStamina()
    {
        if (isSprinting)
        {
            stamina -= 2;
        }
        else
        {
            stamina += 1;
        }
        if (stamina > 100) stamina = 100;
    }
    public void Initialize()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultInput = new DefaultInput();
        defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => accumulatedInputView += e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => inputJump = e.ReadValue<float>();
        defaultInput.Character.Lean.performed += e => inputLean = e.ReadValue<float>();
        defaultInput.Character.Freelook.performed += e => inputFreelook = e.ReadValue<float>();
        defaultInput.Character.Freelook.canceled += e => inputFreelook = e.ReadValue<float>();


        defaultInput.Enable();

        firearm.Initialize();
        swayController.Initialize();

        InvokeRepeating(nameof(RegenHealth), 0, regenHealthSpeed);
        InvokeRepeating(nameof(RegenStamina), 0, regenStaminaSpeed);
    }
    private void Update()
    {
        CalculateView();
    }
    private void FixedUpdate()
    {
        if (isDead) return;

        //DoGroundCheck();

        CalculateLean();

        if (defaultInput.Character.Walk.ReadValue<float>() > 0.5f) isWalking = true;
        else isWalking = false;
        if (defaultInput.Character.Sprint.ReadValue<float>() > 0.5f) inputSprint = !inputSprint;

        maxSpeed = runSpeed;
        if (isWalking || swayController.isAiming) maxSpeed = walkSpeed;
        if (inputSprint && stamina > 0 && Input.GetKey(KeyCode.W))
        {
            isSprinting = true;
            swayController.isAiming = false;
            isWalking = false;

            maxSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            inputSprint = false;
        }



        if (!characterController.isGrounded)
        {
            CalculateAirMovement();
            //velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            CalculateGroundMovement();
            //velocity.y = 0;
        }


        if (inputJump >= 0.1f && characterController.isGrounded)
        {
            velocity.y = jumpForce;
            //velocity.y -= Mathf.Sqrt(jumpForce * -3.0f * gravity);
            hasJumped = true;
        }
        else
        {
            //velocity.y -= gravity;
            hasJumped = false;
        }
        velocity.y -= gravity * Time.deltaTime;

        characterController.Move(velocity);
    }
    //void DoGroundCheck()
    //{
    //    Vector3 rayOrigin = new(transform.position.x, transform.position.y + 1, transform.position.z);
    //    Debug.DrawRay(rayOrigin, Vector3.down, Color.yellow);


    //    if (velocity.y > 0)
    //    {
    //        grounded = false;
    //        return;
    //    }
    //    if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1.1f))
    //    {
    //        if (hit.transform.CompareTag("Ground"))
    //        {
    //            velocity.y = 0;
    //            grounded = true;
    //        }
    //        else grounded = false;
    //    }
    //    else grounded = false;

    //}
    private void CalculateView()
    {
        inputView *= sensitivity / 100;
        if (swayController.isAiming && firearm.hasScope) inputView /= firearm.curZoom;

        if (inputFreelook > 0.5f)
        {
            freelookAngles += inputView * 5;
            freelookAngles.x=Mathf.Clamp(freelookAngles.x, -65, 65);
            freelookAngles.y=Mathf.Clamp(freelookAngles.y, -65, 65);

            GameManager.GM.playCameras[0].transform.localRotation = Quaternion.Euler(-freelookAngles.y, freelookAngles.x, 0);

        }
        else
        {
            cameraAngles += inputView;
            freelookAngles = Vector2.zero;

            cameraAngles.y = Mathf.Clamp(cameraAngles.y, -90, 90);

            Quaternion camRot = Quaternion.AngleAxis(-cameraAngles.y, Vector3.right);
            Quaternion playerRot = Quaternion.AngleAxis(cameraAngles.x, Vector3.up);

            camHolder.localRotation = camRot * Quaternion.Euler(camRecoil);
            transform.localRotation = playerRot;

            GameManager.GM.playCameras[0].transform.localRotation = Quaternion.identity;
            GameManager.GM.playCameras[1].transform.localRotation = Quaternion.identity;
        }



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
            velocity.x += (accelspeed * wishDir.x) * bhopSpeedMult;
            velocity.z += (accelspeed * wishDir.z) * bhopSpeedMult;
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
    public void Hit(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }
    void Die()
    {
        if (isDead) return;
        transform.position += 1.5f * Vector3.down;
        GameManager.GM.playCameras[1].gameObject.SetActive(false);
        isDead = true;
        firearm.canShoot = false;
        Invoke(nameof(RestartScene), 5);
    }
    void RestartScene()
    {
        Destroy(GameManager.GM.gameObject);
        SceneManager.LoadScene(0);
    }
}
