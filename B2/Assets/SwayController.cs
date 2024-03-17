using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayController : MonoBehaviour
{
    DefaultInput defaultInput;
    public MovementController movementController;
    public Firearm firearm;
    public Transform cameraHolder;


    [Header("Aim")]
    public bool isAiming;

    public Vector3 restRot;
    public Vector3 restPos;
    public Vector3 aimPos;

    public float aimSmoothing;
    [Header("Aim Sway Scaling")]
    public float movementRotScaler = 0.1f;
    public float swayRotScaler = 0.5f;
    public float leanScaler = 0.01f;
    public float moveScaler = 0.25f;
    public float breathScaler = 0.25f;
    public float walkScaler = 0.1f;

    Vector3 weaponAimPos;
    Vector3 weaponAimPosVelocity;
    float FOVVelocity;

    [Header("Sway")]

    public Vector3 wpnPos;
    public Vector3 wpnRot;

    public float swayAmount;
    public float swaySmoothing;
    public float swayResetSmoothing;
    //public float swayResetSmoothing;

    Vector3 weaponRotation;
    Vector3 weaponRotationVelocity;
    Vector3 newWeaponRotationVelocity;
    Vector3 newWeaponRotation;

    [Header("Movement Sway")]
    public float movementSwayAmount;
    public float movementSwaySmoothing;

    Vector3 movementRotation;
    Vector3 movementRotationVelocity;
    Vector3 newMovementRotationVelocity;
    Vector3 newMovementRotation;

    [Header("Lean Move")]
    public float leanMoveAmount;
    public float leanMoveSmoothing;

    Vector3 leanMove;
    Vector3 leanMoveVelocity;
    Vector3 newLeanMove;
    Vector3 newLeanMoveVelocity;

    [Header("Movement Move")]
    public float movementMoveAmount;
    public float movementMoveSmoothing;

    Vector3 movementMove;
    Vector3 movementMoveVelocity;
    Vector3 newMovementMove;
    Vector3 newMovementMoveVelocity;

    [Header("Breathing")]
    public bool breathingIn;
    public float breathTime;
    public float breathRotScaling;
    public Vector3 breathInTgt;
    public Vector3 breathOutTgt;
    public float breathSmoothing;

    Vector3 breathMove;
    Vector3 breathMoveVelocity;
    Vector3 newBreathMove;
    Vector3 newBreathMoveVelocity;

    [Header("Walking")]
    public int curWalkLifetime;
    public int walkLifetime;
    public bool rightFoot;
    public float stepDownAmount;
    public float stepSideAmount;
    public float stepRotScaling;
    public float walkMoveSmoothing;

    Vector3 walkMove;
    Vector3 walkMoveVelocity;
    Vector3 newWalkMove;
    Vector3 newWalkMoveVelocity;

    [Header("Zeroing")]
    public int curZero = 0;
    public int curZeroIndex = 0;
    public List<int> zeroes;

    void SwitchBreath() => breathingIn = !breathingIn;
    //void SwitchWalk() => rightFoot = !rightFoot;
    void Start()
    {
        InvokeRepeating("SwitchBreath", 0, breathTime);
        //InvokeRepeating("SwitchWalk", 0, stepTime);

        defaultInput = new DefaultInput();
        defaultInput.Weapon.AimPressed.performed += e => isAiming = !isAiming;
        defaultInput.Weapon.Zero.performed += e => ChangeZero(e.ReadValue<float>());
        defaultInput.Enable();

        zeroes = new()
        {
            10,
            25,
            50,
            75,
            100,
            150,
            200,
            250
        };
        ChangeZero(0);


        restPos = firearm.info.restPos;
        aimPos = firearm.info.aimPos;

    }
    void ChangeZero(float inputZero)
    {
        curZeroIndex += (int)inputZero / 120;
        curZeroIndex = Mathf.Clamp(curZeroIndex, 0, zeroes.Count - 1);
        curZero = zeroes[curZeroIndex];

        Camera cam = Camera.main;
        Vector2 screenCenter = new(Screen.width / 2, Screen.height / 2);
        Vector3 centerTgt = cam.ScreenToWorldPoint(screenCenter);
        centerTgt += curZero * cam.transform.forward;

        transform.LookAt(centerTgt);
        restRot = transform.localEulerAngles;
    }

    void FixedUpdate()
    {

        if(movementController.inputMovement == Vector2.zero)
        {
            curWalkLifetime = 0;
        }
        wpnPos = restPos;
        wpnRot = restRot;
        CalculateWeaponPos();
        CalculateWeaponRot();
        CalculateAim();
        CalculateWalk();
        transform.SetLocalPositionAndRotation(wpnPos, Quaternion.Euler(wpnRot));


        if (curWalkLifetime < walkLifetime)
        {
            curWalkLifetime += 1;
        }
        else
        {
            curWalkLifetime = 0;
            rightFoot = !rightFoot;
        }
    }
    void CalculateWeaponRot()
    {
        float _movementScaler = 1;
        float _swayScaler = 1;
        if (isAiming)
        {
            _movementScaler = movementRotScaler;
            _swayScaler = swayRotScaler;
        }

        weaponRotation.x += movementController.inputView.y * swayAmount;
        weaponRotation.y += -movementController.inputView.x * swayAmount;
        weaponRotation = Vector3.SmoothDamp(weaponRotation, Vector3.zero, ref weaponRotationVelocity, swaySmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swayResetSmoothing);
        newWeaponRotation.z = newWeaponRotation.y*0.75f;

        movementRotation.z = -movementSwayAmount * movementController.inputMovement.x;
        movementRotation = Vector3.SmoothDamp(movementRotation, Vector3.zero, ref movementRotationVelocity, movementSwaySmoothing);

        newMovementRotation = Vector3.SmoothDamp(newMovementRotation, movementRotation, ref newMovementRotationVelocity, movementSwaySmoothing);
        wpnRot += newWeaponRotation * _swayScaler + newMovementRotation * _movementScaler;

    }
    void CalculateWeaponPos()
    {
        float _leanScaler = 1;
        float _moveScaler = 1;
        float _breathScaler = 1;
        if (isAiming)
        {
            _leanScaler = leanScaler;
            _moveScaler = moveScaler;
            _breathScaler = breathScaler;
        }

        leanMove.x = leanMoveAmount * movementController.inputLean;
        leanMove = Vector3.SmoothDamp(leanMove, Vector3.zero, ref leanMoveVelocity, leanMoveSmoothing);
        newLeanMove = Vector3.SmoothDamp(newLeanMove, leanMove, ref newLeanMoveVelocity, leanMoveSmoothing);

        movementMove.x = movementMoveAmount * movementController.velocity.z;
        movementMove.z = movementMoveAmount * movementController.velocity.x;
        movementMove = Vector3.SmoothDamp(movementMove, Vector3.zero, ref movementMoveVelocity, movementMoveSmoothing);
        newMovementMove = Vector3.SmoothDamp(newMovementMove, movementMove, ref newMovementMoveVelocity, movementMoveSmoothing);

        Vector3 breathTgt;
        if (breathingIn) breathTgt = breathInTgt;
        else breathTgt = breathOutTgt;

        breathMove = Vector3.SmoothDamp(breathMove, breathTgt, ref breathMoveVelocity, breathSmoothing);
        newBreathMove = Vector3.SmoothDamp(newBreathMove, breathMove, ref newBreathMoveVelocity, breathSmoothing);

        wpnPos += (newLeanMove * _leanScaler) + (newMovementMove * _moveScaler) + (newBreathMove * _breathScaler);
        wpnRot += (newBreathMove.y * transform.right) * breathRotScaling;
    }
    void CalculateAim()
    {
        float tgtFOV = movementController.FOV;
        Vector3 targetPosition = Vector3.zero;
        if (isAiming)
        {
            tgtFOV = movementController.AimFOV;
            targetPosition = aimPos - restPos;
        }
        weaponAimPos = Vector3.SmoothDamp(weaponAimPos, targetPosition, ref weaponAimPosVelocity, aimSmoothing);
        Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, tgtFOV, ref FOVVelocity, aimSmoothing);
        wpnPos += weaponAimPos;
    }
    void CalculateWalk()
    {
        float _walkScaler = 1;
        if (isAiming) _walkScaler = walkScaler;

        Vector3 target = Vector3.zero;
        //Vector3 camTarget = new(movementController.transform.position.x, 2.75f, movementController.transform.position.z);

        //camTarget.y += camWalkMoveAmount;
        if (movementController.velocity.magnitude > 0.01f)
        {
            if (curWalkLifetime < walkLifetime / 2)
            {
                target.y -= stepDownAmount * movementController.velocity.magnitude;
                if (rightFoot) target.x += stepSideAmount * movementController.velocity.magnitude;
                else target.x -= stepSideAmount * movementController.velocity.magnitude;

                //camTarget.y -= camWalkMoveAmount;
            }
            //else camTarget.y += camWalkMoveAmount;
        }


        //camWalkMove = Vector3.SmoothDamp(camWalkMove, camTarget, ref camWalkMoveVelocity, walkMoveSmoothing);
        //newCamWalkMove = Vector3.SmoothDamp(newCamWalkMove, camWalkMove, ref newCamWalkMoveVelocity, walkMoveSmoothing);
        //cameraHolder.position = newCamWalkMove;

        walkMove = Vector3.SmoothDamp(walkMove, target, ref walkMoveVelocity, walkMoveSmoothing);
        newWalkMove = Vector3.SmoothDamp(newWalkMove, walkMove, ref newWalkMoveVelocity, walkMoveSmoothing);
        wpnPos += newWalkMove * _walkScaler;
        wpnRot += new Vector3 (-newWalkMove.y, newWalkMove.x, 0) * stepRotScaling;
    }

}
