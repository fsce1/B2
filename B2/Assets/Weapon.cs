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
    public Transform sightPoint;
    public Vector3 gunPushTgt;

    [Header("Aim")]
    public bool isAiming;
    public Vector3 restPos;
    public Vector3 aimPos;
    public float aimSmoothing;

    Vector3 weaponAimPos;
    Vector3 weaponAimPosVelocity;
    float FOVVelocity;

    [Header("Sway")]

    public Vector3 wpnPos;
    public Vector3 wpnRot;

    public float swayAmount;
    public float swaySmoothing;
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
        defaultInput.Weapon.AimPressed.performed += e => AimPressed();
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
    }

    void ChangeZero(float inputZero)
    {
        curZeroIndex += (int)inputZero / 120;
        curZeroIndex = Mathf.Clamp(curZeroIndex, 0, zeroes.Count - 1);
        curZero = zeroes[curZeroIndex];

    }
    void Update()
    {
        Camera cam = Camera.main;

        Vector2 screenCenter = new(Screen.width / 2, Screen.height / 2);
        Vector3 centerTgt = cam.ScreenToWorldPoint(screenCenter);
        centerTgt += curZero * cam.transform.forward;

        wpnPos = restPos;
        CalculateAim();
        CalculateWeaponPos();
        CalculateWeaponRot();

        //transform.localPosition = restPos;
        transform.SetLocalPositionAndRotation(wpnPos, Quaternion.Euler(wpnRot));
    }
    //private void FixedUpdate()
    //{
    //    if (Physics.Raycast(barrelPoint.position, transform.forward, out RaycastHit hit, 1f))
    //    {
    //        if (hit.distance <= 0.01f) gunPushTgt.z -= 0.01f;
    //    }
    //    else if (gunPushTgt.z <= restPos.z)
    //    {
    //        gunPushTgt.z += 0.01f;
    //    }

    //    fixedWpnPos = gunPushTgt;

    //}
    void CalculateWeaponRot()
    {
        float aimingDecrease;
        if (isAiming) aimingDecrease = 0.25f;
        else aimingDecrease = 1f;
        weaponRotation.x += movementController.inputView.y * swayAmount * Time.deltaTime;
        weaponRotation.y += -movementController.inputView.x * swayAmount * Time.deltaTime;
        weaponRotation = Vector3.SmoothDamp(weaponRotation, Vector3.zero, ref weaponRotationVelocity, swaySmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swaySmoothing);
        newWeaponRotation.z = newWeaponRotation.y;

        movementRotation.z = -movementSwayAmount * movementController.inputMovement.x;
        movementRotation = Vector3.SmoothDamp(movementRotation, Vector3.zero, ref movementRotationVelocity, movementSwaySmoothing);

        newMovementRotation = Vector3.SmoothDamp(newMovementRotation, movementRotation, ref newMovementRotationVelocity, movementSwaySmoothing);
        wpnRot = (newWeaponRotation + newMovementRotation) * aimingDecrease;

    }
    void CalculateWeaponPos()
    {
        leanMove.x = leanMoveAmount * movementController.inputLean;
        leanMove = Vector3.SmoothDamp(leanMove, Vector3.zero, ref leanMoveVelocity, leanMoveSmoothing);
        newLeanMove = Vector3.SmoothDamp(newLeanMove, leanMove, ref newLeanMoveVelocity, leanMoveSmoothing);

        movementMove.x = movementMoveAmount * movementController.inputMovement.x;
        movementMove.z = movementMoveAmount * movementController.inputMovement.y;
        movementMove = Vector3.SmoothDamp(movementMove, Vector3.zero, ref movementMoveVelocity, movementMoveSmoothing);
        newMovementMove = Vector3.SmoothDamp(newMovementMove, movementMove, ref newMovementMoveVelocity, movementMoveSmoothing);

       
        if (!isAiming) wpnPos += newLeanMove + newMovementMove;

    }
    void AimPressed()
    {
        isAiming = !isAiming;
        
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
        Gizmos.DrawRay(barrelPoint.position, transform.forward * 100);
    }
}
