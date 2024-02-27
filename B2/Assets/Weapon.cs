using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform barrelPoint;
    DefaultInput defaultInput;
    public MovementController movementController;
    [Header("Zeroing")]
    public float inputZero;
    public int curZero = 0;
    public int curZeroIndex = 0;
    public List<int> zeroes = new();

    [Header("Sway")]
    public Vector3 weaponRotation;
    public Vector3 weaponRotationVelocity;

    public float swayAmount;
    public float swayResetAmount;
    void Start()
    {
        defaultInput = new DefaultInput();
        defaultInput.Character.Zero.performed += e => inputZero = e.ReadValue<float>();
        defaultInput.Enable();

        zeroes.Add(10);
        zeroes.Add(25);
        zeroes.Add(50);
        zeroes.Add(75);
        zeroes.Add(100);
        zeroes.Add(150);
        zeroes.Add(200);
        zeroes.Add(250);
    }

    
    void Update()
    {
        Camera cam = Camera.main;

        curZeroIndex += (int)inputZero / 120;
        curZeroIndex = Mathf.Clamp(curZeroIndex, 0, zeroes.Count - 1);
        curZero = zeroes[curZeroIndex];

        Vector2 screenCenter = new(Screen.width / 2, Screen.height / 2);
        Vector3 centerTgt = cam.ScreenToWorldPoint(screenCenter);
        centerTgt += curZero * cam.transform.forward;

        transform.LookAt(centerTgt);
        Quaternion restPos = transform.localRotation;

        weaponRotation.x += -movementController.inputView.y * swayAmount * Time.deltaTime;
        weaponRotation.y += movementController.inputView.x * swayAmount * Time.deltaTime;

        //weaponRotation = Vector3.SmoothDamp(weaponRotation, restPos, ref weaponRotationVelocity, swayResetAmount);
        //newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swayResetAmount);

        //weaponRotation = Vector3.RotateTowards(weaponRotation, restPos, Time.deltaTime * swayResetAmount, 1);

        //Quaternion finalRotation = Quaternion.RotateTowards(Quaternion.Euler(weaponRotation), restPos, 0.1f);

        //transform.localRotation = Quaternion.LookRotation((centerTgt- transform.position) * movementController.inputView,Vector3.up);
        //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(restPos), Time.deltaTime);
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(weaponRotation), swayAmount * Time.deltaTime);
        //transform.localEulerAngles = weaponRotation;
        //transform.localRotation = Quaternion.Euler(weaponRotation);
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(barrelPoint.position, transform.forward * 100);
    }
}
