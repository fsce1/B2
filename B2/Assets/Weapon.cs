using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    DefaultInput defaultInput;
    public MovementController movementController;

    [Header("Zeroing")]
    public int curZero = 0;
    public int curZeroIndex = 0;
    public List<int> zeroes;

    [Header("Points")]
    public Transform barrelPoint;

    [Header("Aim")]
    public bool isAiming;
    Vector3 restRot;
    Vector3 restPos;
    public Vector3 aimPos;
    public float aimSmoothing;
    public float rotScaling = 0.25f;
    public float leanScaling = 0.1f;
    public float moveScaling = 0.25f;

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
    void Start()
    {
        restPos = transform.localPosition;
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
        wpnPos = restPos;
        wpnRot = restRot;
        CalculateAim();
        CalculateWeaponPos();
        CalculateWeaponRot();
        transform.SetLocalPositionAndRotation(wpnPos, Quaternion.Euler(wpnRot));
    }
    void CalculateWeaponRot()
    {
        float aimRotScaler = 1;
        if (isAiming) aimRotScaler = rotScaling;
        
        weaponRotation.x += movementController.inputView.y * swayAmount * Time.deltaTime;
        weaponRotation.y += -movementController.inputView.x * swayAmount * Time.deltaTime;
        weaponRotation = Vector3.SmoothDamp(weaponRotation, Vector3.zero, ref weaponRotationVelocity, swaySmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swayResetSmoothing);
        newWeaponRotation.z = newWeaponRotation.y;

        movementRotation.z = -movementSwayAmount * movementController.inputMovement.x;
        movementRotation = Vector3.SmoothDamp(movementRotation, Vector3.zero, ref movementRotationVelocity, movementSwaySmoothing);

        newMovementRotation = Vector3.SmoothDamp(newMovementRotation, movementRotation, ref newMovementRotationVelocity, movementSwaySmoothing);
        wpnRot += (newWeaponRotation + newMovementRotation) * aimRotScaler;

    }
    void CalculateWeaponPos()
    {
        float aimLeanScaler = 1;
        float aimMoveScaler = 1;
        if (isAiming)
        {
            aimLeanScaler = leanScaling;
            aimMoveScaler = moveScaling;
        }

        leanMove.x = leanMoveAmount * movementController.inputLean;
        leanMove = Vector3.SmoothDamp(leanMove, Vector3.zero, ref leanMoveVelocity, leanMoveSmoothing);
        newLeanMove = Vector3.SmoothDamp(newLeanMove, leanMove, ref newLeanMoveVelocity, leanMoveSmoothing);

        movementMove.x = movementMoveAmount * movementController.inputMovement.x;
        movementMove.z = movementMoveAmount * movementController.inputMovement.y;
        movementMove = Vector3.SmoothDamp(movementMove, Vector3.zero, ref movementMoveVelocity, movementMoveSmoothing);
        newMovementMove = Vector3.SmoothDamp(newMovementMove, movementMove, ref newMovementMoveVelocity, movementMoveSmoothing);

       
        wpnPos += (newLeanMove * aimLeanScaler) + (newMovementMove * aimMoveScaler);

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
    private void OnDrawGizmos()
    {
        if(Physics.Raycast(barrelPoint.position, transform.forward, out RaycastHit hit, Mathf.Infinity)){

            Gizmos.DrawLine(barrelPoint.position, hit.point);
        }
    }
}
