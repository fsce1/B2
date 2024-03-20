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
    public GameObject roundPrefab;
    DefaultInput defaultInput;
    public FirearmInfo info;
    public Animator animator;
    public MovementController movementController;
    public List<Camera> cameras = new();

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
    public GameObject muzzleFlash;

    [Header("Scope")]
    public bool hasScope;
    public float curZoom;
    public Camera scopeCamera;
    public Vector2 zoomBounds;
    public float scrollSpeed;
    float baseFOV;

    private void OnDrawGizmos()
    {
        if (Physics.Raycast(barrelPoint.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            if (Camera.current != Camera.main) return;
            Gizmos.DrawLine(barrelPoint.position, hit.point);
        }
    }
    private void Start()
    {
        defaultInput = new DefaultInput();
        defaultInput.Weapon.SingleFirePressed.performed += e => SingleFire();
        defaultInput.Weapon.CycleFireMode.performed += e => CycleFireMode();
        defaultInput.Weapon.FullAutoHeld.performed += e => fullAutoHeld = !fullAutoHeld;
        defaultInput.Weapon.Reload.performed += e => BeginReload();
        defaultInput.Weapon.Zero.performed += e => UpdateScopeZoom(e.ReadValue<float>() / 120);
        defaultInput.Enable();

        animator.Play("Base Layer.Idle");

        baseFOV = scopeCamera.fieldOfView;
    }
    void UpdateScopeZoom(float input)
    {
        if (!hasScope) return;
        curZoom += input * scrollSpeed;
        curZoom = Mathf.Clamp(curZoom, zoomBounds.x, zoomBounds.y);
        scopeCamera.fieldOfView = baseFOV / curZoom;
    }
    private void FixedUpdate()
    {
        if (canShoot && fullAutoHeld && curFireMode == FireMode.fullAuto && !isReloading && roundsInMag > 0)
        {
            Shoot();
        }
        CalculateRecoil();

        transform.SetLocalPositionAndRotation(recoilPos, Quaternion.Euler(recoilRot));
        //cameraHolder.localEulerAngles += recoilCam;
        //Camera.main.transform.localRotation = Quaternion.Euler(recoilCam);
        foreach (Camera c in cameras) c.transform.localRotation = Quaternion.Euler(recoilCam);
        if (Vector3Int.RoundToInt(recoilRot) == Vector3.zero && Vector3Int.RoundToInt(recoilPos) == Vector3.zero && Vector3Int.RoundToInt(recoilCam) == Vector3.zero)
        {
            sustainedRecoilAdd = 1;
        }


        //if (!isReloading && movementController.velocity.magnitude > 0.01f )
        //{
        //    animator.Play("Base Layer.Walk");
        //    animator.speed = 1+ Mathf.InverseLerp(0, movementController.maxSpeed, movementController.velocity.magnitude);
        //}
        //else animator.Play("Base Layer.Idle");

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
        GameObject roundObj = Instantiate(roundPrefab, barrelPoint.position, transform.rotation);
        //roundObj.transform.localEulerAngles = transform.forward;
        roundObj.GetComponent<Round>().firearmFiredFrom = this;

        AddRecoil();
        canShoot = false;
        roundsInMag -= 1;
        sustainedRecoilAdd += info.sustainedRecoilAdd;

        if (muzzleFlash != null)
        {
            muzzleFlash.transform.localEulerAngles = new(muzzleFlash.transform.localEulerAngles.x, muzzleFlash.transform.localEulerAngles.y, UnityEngine.Random.Range(0, 360));
            muzzleFlash.SetActive(true);
            Invoke(nameof(ResetMuzzleFlash), Time.deltaTime * 2.5f);
        }

        if (roundsInMag >= 0)
        {
            Invoke(nameof(ResetShot), 50 / info.roundsPerMinute);
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


