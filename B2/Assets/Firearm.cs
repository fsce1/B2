using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireMode
{
    singleFire,
    fullAuto,
    twoRoundBurst,
    threeRoundBurst,
    singleAction,
    doubleAction,
}

public class Firearm : MonoBehaviour
{
    DefaultInput defaultInput;
    public FirearmInfo info;

    public Vector3 recoilRot;
    public Vector3 tgtRecoilRot;

    public Vector3 recoilPos;
    public Vector3 tgtRecoilPos;

    [Header("Input")]
    public bool fullAutoHeld;

    [Header("State")]
    public FireMode curFireMode = FireMode.singleFire;
    int curMode = 0;

    public bool isReloading;
    public int roundsInMag;
    public bool canShoot;

    [Header("Points")]
    public Transform barrelPoint;



    private void Start()
    {
        defaultInput = new DefaultInput();
        defaultInput.Weapon.SingleFirePressed.performed += e => SingleFire();
        defaultInput.Weapon.CycleFireMode.performed += e => CycleFireMode();
        defaultInput.Weapon.FullAutoHeld.performed += e => fullAutoHeld = !fullAutoHeld;

        defaultInput.Enable();


    }
    private void FixedUpdate()
    {
        if (canShoot && fullAutoHeld && curFireMode == FireMode.fullAuto && !isReloading && roundsInMag > 0)
        {
            Shoot();
        }

        transform.localPosition = recoilPos;
        transform.localRotation = Quaternion.Euler(recoilRot);

        CalculateRecoil();
    }

    void CycleFireMode()
    {
        if (curMode >= info.fireModes.Count - 1) curMode = 0;
        else curMode += 1;

        curFireMode = info.fireModes[curMode];
    }
    void SingleFire()
    {
        if (canShoot && curFireMode == FireMode.singleFire && !isReloading && roundsInMag > 0) Shoot();
    }

    void Shoot()

    {
        AddRecoil();
        canShoot = false;
        roundsInMag -= 1;

        if (roundsInMag >= 0)
        {
            Invoke(nameof(ResetShot), 50 / info.roundsPerMinute);
        }
    }
    void ResetShot()
    {
        canShoot = true;
    }
    void AddRecoil()
    {
        tgtRecoilRot +=
            (-UnityEngine.Random.Range(info.verticalPerShot.x, info.verticalPerShot.y) * Vector3.right)
            + (UnityEngine.Random.Range(info.horizontalPerShot.x, info.horizontalPerShot.y) * Vector3.up);

        tgtRecoilPos += (-UnityEngine.Random.Range(info.lateralPerShot.x, info.lateralPerShot.y) * Vector3.forward);
    }
    void CalculateRecoil()
    {
        tgtRecoilRot = Vector3.Lerp(tgtRecoilRot, Vector3.zero, Time.deltaTime * info.verticalRecovery);
        recoilRot = Vector3.Slerp(recoilRot, tgtRecoilRot, Time.deltaTime * info.verticalSnappiness);

        tgtRecoilPos = Vector3.Lerp(tgtRecoilPos, Vector3.zero, Time.deltaTime * info.lateralRecovery);
        recoilPos = Vector3.Slerp(recoilPos, tgtRecoilPos, Time.deltaTime * info.lateralSnappiness);
        //recoilRot += Vector3.SmoothDamp(recoilRot, newRecoilRot, ref rotVelocity, Time.deltaTime * 0.1f);

        //newRecoilRot += Vector3.SmoothDamp(newRecoilRot, Vector3.zero, ref resetVelocity, Time.deltaTime * info.verticalRecovery);

    }
}


