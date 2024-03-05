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
    public Transform camHolder;
    public Animator animator;

    public Vector3 recoilRot;
    public Vector3 tgtRecoilRot;

    public Vector3 recoilPos;
    public Vector3 tgtRecoilPos;

    public Vector3 recoilCam;
    public Vector3 tgtRecoilCam;

    [Header("Input")]
    public bool fullAutoHeld;

    [Header("State")]
    public FireMode curFireMode = FireMode.singleFire;
    int curMode = 0;

    public bool isReloading;
    public int roundsInMag;
    public bool canShoot;
    public float sustainedRecoilAdd;

    [Header("Points")]
    public Transform barrelPoint;

    [Header("Particles")]
    public GameObject muzzleFlash;


    private void Start()
    {
        defaultInput = new DefaultInput();
        defaultInput.Weapon.SingleFirePressed.performed += e => SingleFire();
        defaultInput.Weapon.CycleFireMode.performed += e => CycleFireMode();
        defaultInput.Weapon.FullAutoHeld.performed += e => fullAutoHeld = !fullAutoHeld;
        defaultInput.Weapon.Reload.performed += e => BeginReload();
        defaultInput.Enable();

        animator.Play("Base Layer.Idle");
    }
    private void FixedUpdate()
    {
        if (canShoot && fullAutoHeld && curFireMode == FireMode.fullAuto && !isReloading && roundsInMag > 0)
        {
            Shoot();
        }
        CalculateRecoil();

        transform.SetLocalPositionAndRotation(recoilPos, Quaternion.Euler(recoilRot));
        Camera.main.transform.localRotation = Quaternion.Euler(recoilCam);

        if (Vector3Int.RoundToInt(recoilRot) == Vector3.zero && Vector3Int.RoundToInt(recoilPos) == Vector3.zero && Vector3Int.RoundToInt(recoilCam) == Vector3.zero)
        {
            sustainedRecoilAdd = 1;
        }

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
        sustainedRecoilAdd += info.sustainedRecoilAdd;

        muzzleFlash.transform.localEulerAngles = new(0, 0, UnityEngine.Random.Range(0, 360));
        muzzleFlash.SetActive(true);
        Invoke(nameof(ResetMuzzleFlash), Time.deltaTime * 2 );

        if (roundsInMag >= 0)
        {
            Invoke(nameof(ResetShot), 60 / info.roundsPerMinute);
        }
    }
    void ResetMuzzleFlash() => muzzleFlash.SetActive(false);
    void ResetShot()
    {
        canShoot = true;
    }
    void AddRecoil()
    {
        tgtRecoilRot +=
            (-UnityEngine.Random.Range(info.verticalPerShot.x, info.verticalPerShot.y) * Vector3.right)
            + (UnityEngine.Random.Range(info.horizontalPerShot.x, info.horizontalPerShot.y) * Vector3.up);

        if (info.sustainedAffectsVertical) tgtRecoilRot.x *= sustainedRecoilAdd;
        if (info.sustainedAffectsHorizontal) tgtRecoilRot.y *= sustainedRecoilAdd;
        tgtRecoilPos += -UnityEngine.Random.Range(info.lateralPerShot.x, info.lateralPerShot.y) * Vector3.forward;
        if (info.sustainedAffectsLateral) tgtRecoilPos.z *= sustainedRecoilAdd;
        tgtRecoilCam += -UnityEngine.Random.Range(info.camPerShot.x, info.camPerShot.y) * Vector3.right;
        if (info.sustainedAffectsCam) tgtRecoilCam.x *= sustainedRecoilAdd;
    }   
    void CalculateRecoil()
    {

        tgtRecoilRot = Vector3.Lerp(tgtRecoilRot, Vector3.zero, Time.deltaTime * info.rotRecovery);
        recoilRot = Vector3.Slerp(recoilRot, tgtRecoilRot, Time.deltaTime * info.rotSnappiness);

        tgtRecoilPos = Vector3.Lerp(tgtRecoilPos, Vector3.zero, Time.deltaTime * info.lateralRecovery);
        recoilPos = Vector3.Slerp(recoilPos, tgtRecoilPos, Time.deltaTime * info.lateralSnappiness);

        tgtRecoilCam = Vector3.Lerp(tgtRecoilCam, Vector3.zero, Time.deltaTime * info.camRecovery);
        recoilCam = Vector3.Slerp(recoilCam, tgtRecoilCam, Time.deltaTime * info.camSnappiness);
        //recoilRot += Vector3.SmoothDamp(recoilRot, newRecoilRot, ref rotVelocity, Time.deltaTime * 0.1f);

        //newRecoilRot += Vector3.SmoothDamp(newRecoilRot, Vector3.zero, ref resetVelocity, Time.deltaTime * info.verticalRecovery);
    }

    void BeginReload()
    {
        animator.Play("Base Layer.Reload");

        isReloading = true;
        canShoot = false;

        Invoke(nameof(EndReload), animator.GetCurrentAnimatorClipInfo(0).Length);
    }
    void EndReload()
    {

        animator.Play("Base Layer.Idle");
        isReloading = false;
        canShoot = true;
        roundsInMag = info.magazineSize;
    }
}


