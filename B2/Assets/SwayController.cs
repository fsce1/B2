using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwayController : MonoBehaviour
{
    [Header("Ref")]
    DefaultInput defaultInput;
    public Player player;
    public AudioSource audioSource;

    [Header("Aim")]
    public bool isAiming;
    public Vector3 restRot;
    public Vector3 restPos;
    public Vector3 aimPos;

    public float aimSmoothing;

    public float armaAimMult;
    public float armaAimAmount;
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
    public Vector2 breathAmount;
    public Vector2 breathSidewaysAmount;
    public Vector3 breathTgt;
    public float breathSmoothing;

    Vector3 breathMove;
    Vector3 breathMoveVelocity;
    Vector3 newBreathMove;
    Vector3 newBreathMoveVelocity;

    [Header("Walking")]
    public int curWalkLifetime;
    public int walkLifetime;
    public bool rightFoot;
    public float walkLifetimeScaler;
    public Vector2 stepDownAmount;
    public Vector2 stepSideAmount;
    public float stepRotScaling;
    public float walkMoveSmoothing;
    public float walkCamMoveSmoothing;
    public float camWalkMoveAmount;
    public List<AudioClip> stepSounds;

    Vector3 walkMove;
    Vector3 walkMoveVelocity;
    Vector3 newWalkMove;
    Vector3 newWalkMoveVelocity;

    Vector3 camWalkMove;
    Vector3 camWalkMoveVelocity;
    Vector3 newCamWalkMove;
    Vector3 newCamWalkMoveVelocity;

    [Header("Sprinting")]

    public float sprintMoveSmoothing;

    Vector3 sprintMove;
    Vector3 sprintMoveVelocity;
    Vector3 newSprintMove;
    Vector3 newSprintMoveVelocity;
    Vector3 sprintRotation;
    Vector3 sprintRotationVelocity;
    Vector3 newSprintRotation;
    Vector3 newSprintRotationVelocity;

    [Header("Zeroing")]
    public int curZero = 0;
    public int curZeroIndex = 0;
    public List<int> zeroes;

    void SwitchBreath()
    {
        breathingIn = !breathingIn;
        breathTgt = Vector3.zero;
        breathTgt.x = Random.Range(breathSidewaysAmount.x, breathSidewaysAmount.y);
        breathTgt.z = Random.Range(breathSidewaysAmount.x, breathSidewaysAmount.y);
        breathTgt.y = Random.Range(breathAmount.x, breathAmount.y);

        if (!breathingIn)
        {
            breathTgt.y = -breathTgt.y;
            breathTgt.x = 0;
        }

    }

    //void SwitchWalk() => rightFoot = !rightFoot;
    private void Start()
    {
        InvokeRepeating(nameof(SwitchBreath), 0, breathTime);
    }

    public void Initialize()
    {

        //InvokeRepeating("SwitchWalk", 0, stepTime);

        defaultInput = new DefaultInput();
        defaultInput.Weapon.AimPressed.performed += e => isAiming = !isAiming;
        defaultInput.Weapon.Zero.performed += e => ChangeZero(e.ReadValue<float>());
        defaultInput.Character.ArmaZoom.performed += e => armaAimMult = armaAimAmount;
        defaultInput.Character.ArmaZoom.canceled += e => armaAimMult = 1;
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


        restPos = player.firearm.info.restPos;
        aimPos = player.firearm.info.aimPos;

    }
    void ChangeZero(float inputZero)
    {
        //curZeroIndex += (int)inputZero / 120;
        //curZeroIndex = Mathf.Clamp(curZeroIndex, 0, zeroes.Count - 1);
        //curZero = zeroes[curZeroIndex];

        //Camera cam = Camera.main;
        //Vector2 screenCenter = new(Screen.width / 2, Screen.height / 2);
        //Vector3 centerTgt = cam.ScreenToWorldPoint(screenCenter);
        //centerTgt += curZero * cam.transform.forward;

        ////transform.LookAt(centerTgt);
        ////restRot = transform.localEulerAngles;
        //Quaternion q = Quaternion.LookRotation(centerTgt, transform.up);
        //restRot = Quaternion.Euler(q);
    }

    void FixedUpdate()
    {
        if (GameManager.GM.player.inputMovement == Vector2.zero)
        {
            curWalkLifetime = 0;
        }

        wpnPos = restPos;

        wpnRot = restRot;

        //wpnRot += GameManager.GM.playCameras[1].transform.localEulerAngles;

        //if (player.isSprinting)
        //{
        //    wpnPos.x = 0;
        //    wpnPos.y = -0.25f;
        //    wpnRot.y = -45;
        //}
        CalculateSprint();
        CalculateWeaponPos();
        CalculateWeaponRot();
        CalculateAim();
        CalculateWalk();
        transform.SetLocalPositionAndRotation(wpnPos, Quaternion.Euler(wpnRot));

        walkLifetimeScaler = 1;
        if (player.isWalking) walkLifetimeScaler = 1.25f;
        if (player.isSprinting) walkLifetimeScaler = 0.6f;

        if (curWalkLifetime < walkLifetime * walkLifetimeScaler)
        {
            curWalkLifetime += 1;
        }
        else
        {
            curWalkLifetime = 0;
            rightFoot = !rightFoot;

            audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Count)]);
        }
    }
    void CalculateSprint()
    {
        Vector3 PosTgt;
        Vector3 RotTgt;

        if (player.isSprinting)
        {
            PosTgt = player.firearm.info.sprintPos - restPos;
            RotTgt = player.firearm.info.sprintRot - restRot;
        }
        else
        {
            PosTgt = sprintMove = Vector3.zero;
            RotTgt = sprintRotation = Vector3.zero;
        }

        sprintMove = Vector3.SmoothDamp(PosTgt, Vector3.zero, ref sprintMoveVelocity, sprintMoveSmoothing);
        newSprintMove = Vector3.SmoothDamp(newSprintMove, sprintMove, ref newSprintMoveVelocity, sprintMoveSmoothing);

        sprintRotation = Vector3.SmoothDamp(RotTgt, Vector3.zero, ref sprintRotationVelocity, sprintMoveSmoothing);
        newSprintRotation = Vector3.SmoothDamp(newSprintRotation, sprintRotation, ref newSprintRotationVelocity, sprintMoveSmoothing);

        wpnPos += newSprintMove;
        wpnRot += newSprintRotation;

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

        weaponRotation.x += GameManager.GM.player.accumulatedInputView.y * swayAmount;
        weaponRotation.y += -GameManager.GM.player.accumulatedInputView.x * swayAmount;
        weaponRotation = Vector3.SmoothDamp(weaponRotation, Vector3.zero, ref weaponRotationVelocity, swaySmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swayResetSmoothing);
        newWeaponRotation.z = newWeaponRotation.y * 0.75f;

        movementRotation.z = -movementSwayAmount * GameManager.GM.player.inputMovement.x;
        movementRotation = Vector3.SmoothDamp(movementRotation, Vector3.zero, ref movementRotationVelocity, movementSwaySmoothing);

        newMovementRotation = Vector3.SmoothDamp(newMovementRotation, movementRotation, ref newMovementRotationVelocity, movementSwaySmoothing);
        wpnRot += newWeaponRotation * _swayScaler + newMovementRotation * _movementScaler;

        GameManager.GM.player.accumulatedInputView = Vector2.zero;

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

        leanMove.x = leanMoveAmount * GameManager.GM.player.inputLean;
        leanMove = Vector3.SmoothDamp(leanMove, Vector3.zero, ref leanMoveVelocity, leanMoveSmoothing);
        newLeanMove = Vector3.SmoothDamp(newLeanMove, leanMove, ref newLeanMoveVelocity, leanMoveSmoothing);

        movementMove.x = movementMoveAmount * GameManager.GM.player.velocity.z;
        movementMove.z = movementMoveAmount * GameManager.GM.player.velocity.x;
        movementMove.y = 0;
        if(GameManager.GM.player.velocity.y > 0) movementMove.y = -GameManager.GM.player.velocity.y;

        movementMove = Vector3.SmoothDamp(movementMove, Vector3.zero, ref movementMoveVelocity, movementMoveSmoothing);
        newMovementMove = Vector3.SmoothDamp(newMovementMove, movementMove, ref newMovementMoveVelocity, movementMoveSmoothing);

        breathMove = Vector3.SmoothDamp(breathMove, breathTgt, ref breathMoveVelocity, breathSmoothing);
        newBreathMove = Vector3.SmoothDamp(newBreathMove, breathMove, ref newBreathMoveVelocity, breathSmoothing);

        wpnPos += (newLeanMove * _leanScaler) + (newMovementMove * _moveScaler) + (newBreathMove * _breathScaler);
        wpnRot += _breathScaler * breathRotScaling * new Vector3(newBreathMove.y, newBreathMove.x, 0);
    }
    void CalculateAim()
    {
        float tgtFOV = GameManager.GM.player.FOV * armaAimMult;
        Vector3 targetPosition = Vector3.zero;
        if (isAiming)
        {
            tgtFOV = GameManager.GM.player.AimFOV * armaAimMult;
            targetPosition = aimPos - restPos;
        }
        weaponAimPos = Vector3.SmoothDamp(weaponAimPos, targetPosition, ref weaponAimPosVelocity, aimSmoothing);
        foreach (Camera c in GameManager.GM.playCameras) c.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, tgtFOV, ref FOVVelocity, aimSmoothing);
        wpnPos += weaponAimPos;
    }
    void CalculateWalk()
    {
        float _walkScaler = 1;
        if (isAiming) _walkScaler = walkScaler;
        else if (player.isSprinting) _walkScaler = 2f;
 
        Vector3 target = Vector3.zero;
        //Vector3 camTarget = new(0, player.transform.position.y +1.75f, 0);
        Vector3 pos = player.transform.position;
        pos.y += 1.75f;
        player.camHolder.position = pos;
        Vector3 camTarget;
        float lateralVelocity = new Vector2(GameManager.GM.player.velocity.x, GameManager.GM.player.velocity.z).magnitude;
        if (lateralVelocity > 0.01f)
        {

            if (curWalkLifetime < (walkLifetime * walkLifetimeScaler) / 2)
            {
                target.y -= Random.Range(stepDownAmount.x, stepDownAmount.y) * lateralVelocity;
                float sideAmount = Random.Range(stepSideAmount.x, stepSideAmount.y);
                if (rightFoot) target.x += sideAmount * lateralVelocity;
                else target.x -= sideAmount * lateralVelocity;

                camTarget = new(0,-camWalkMoveAmount,0);
            }
            else camTarget = new(0, camWalkMoveAmount, 0);
        }
        else
        {
            camTarget = new(0, 0, 0);
            curWalkLifetime = 0;
        }
        camWalkMove = Vector3.SmoothDamp(camWalkMove, camTarget, ref camWalkMoveVelocity, walkCamMoveSmoothing);
        newCamWalkMove = Vector3.SmoothDamp(newCamWalkMove, camWalkMove, ref newCamWalkMoveVelocity, walkCamMoveSmoothing);

        walkMove = Vector3.SmoothDamp(walkMove, target, ref walkMoveVelocity, walkMoveSmoothing);
        newWalkMove = Vector3.SmoothDamp(newWalkMove, walkMove, ref newWalkMoveVelocity, walkMoveSmoothing);

        player.camHolder.position += newCamWalkMove;

        wpnPos += _walkScaler * 25 * lateralVelocity * newWalkMove;
        wpnRot += _walkScaler * 25 * lateralVelocity * stepRotScaling * new Vector3(newWalkMove.y, newWalkMove.x, newWalkMove.x * 0.75f);
    }


}
