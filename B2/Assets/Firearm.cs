using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
    public bool isSuppressed;
    public int roundsInMag;
    public bool canShoot;
    public float reloadTime;
    public float sustainedRecoilAdd;


    [Header("Points")]
    public Transform barrelPoint;
    public GameObject muzzleFlash;

    [Header("Scope")]
    public bool hasScope;
    public bool firstFocalPlane;
    public float baseReticleScale;
    public float tgtZoom;
    public float curZoom;
    public float zoomSmoothing;
    float curZoomVel;
    public Camera scopeCamera;
    public MeshRenderer scopeMesh;
    public Vector2 zoomBounds;
    public float scrollSpeed;
    float baseFOV;

    [Header("Audio")]
    public List<AudioClip> shotSounds;

    private void OnDrawGizmos()
    {
        if (Physics.Raycast(GameManager.GM.player.firearm.barrelPoint.position, GameManager.GM.player.swayController.restRot, out RaycastHit hit2, Mathf.Infinity))
        {
            Gizmos.DrawLine(barrelPoint.position, hit2.point);
        }
    }
    public void Initialize()
    {
        defaultInput = new DefaultInput();
        defaultInput.Weapon.SingleFirePressed.performed += e => SingleFire();
        defaultInput.Weapon.CycleFireMode.performed += e => CycleFireMode();
        defaultInput.Weapon.FullAutoHeld.performed += e => fullAutoHeld = !fullAutoHeld;
        defaultInput.Weapon.Reload.performed += e => BeginReload();
        defaultInput.Weapon.Zero.performed += e => UpdateScopeZoom(e.ReadValue<float>() / 120);
        defaultInput.Enable();

        animator.Play("Base Layer.Idle");
        if (hasScope)
        {
            baseFOV = scopeCamera.fieldOfView;
            baseReticleScale = scopeMesh.material.GetFloat("_ReticleScale");
        }
        //controller = animator.runtimeAnimatorController;
    }
    void UpdateScopeZoom(float input)
    {
        if (!hasScope) return;

        tgtZoom += input * scrollSpeed;
        tgtZoom = Mathf.Clamp(tgtZoom, zoomBounds.x, zoomBounds.y);

    }
    private void FixedUpdate()
    {
        if (hasScope)
        {
            curZoom = Mathf.SmoothDamp(curZoom, tgtZoom, ref curZoomVel, zoomSmoothing);
            scopeCamera.fieldOfView = baseFOV / curZoom;
            if (firstFocalPlane) scopeMesh.material.SetFloat("_ReticleScale", baseReticleScale * curZoom);
        }


        if (GameManager.GM.player.isDead) return;
        if (canShoot && fullAutoHeld && curFireMode == FireMode.fullAuto && !isReloading && roundsInMag > 0)
        {
            Shoot();
        }
        CalculateRecoil();

        transform.SetLocalPositionAndRotation(recoilPos, Quaternion.Euler(recoilRot));
        //cameraHolder.localEulerAngles += recoilCam;
        //Camera.main.transform.localRotation = Quaternion.Euler(recoilCam);
        foreach (Camera c in GameManager.GM.playCameras) c.transform.localRotation = Quaternion.Euler(recoilCam);
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
        if (GameManager.GM.player.isSprinting) return;
        Vector3 shootDir = transform.rotation.eulerAngles;
        shootDir += UnityEngine.Random.Range(-0.05f, 0.05f) * transform.right + UnityEngine.Random.Range(-0.075f, 0.075f) * transform.up;


        GameObject roundObj = Instantiate(roundPrefab, barrelPoint.position,
            Quaternion.Euler(shootDir));
        // + UnityEngine.Random.Range(-5f, 5f) * transform.right + UnityEngine.Random.Range(-5f, 5f) * transform.up)


        AddRecoil();
        canShoot = false;
        roundsInMag -= 1;
        sustainedRecoilAdd += info.sustainedRecoilAdd;

        if (isSuppressed) GameManager.GM.player.swayController.audioSource.volume = 0.1f;
        else GameManager.GM.player.swayController.audioSource.volume = 0.6f;
        GameManager.GM.player.swayController.audioSource.PlayOneShot(shotSounds[UnityEngine.Random.Range(0, shotSounds.Count)]);

        if (muzzleFlash != null)
        {
            muzzleFlash.transform.localEulerAngles = new(muzzleFlash.transform.localEulerAngles.x, muzzleFlash.transform.localEulerAngles.y, UnityEngine.Random.Range(0, 360));
            muzzleFlash.SetActive(true);
            Invoke(nameof(ResetMuzzleFlash), Time.deltaTime * 2.5f);
        }

        if (roundsInMag >= 0)
        {
            //Invoke(nameof(ResetShot), 50 / info.roundsPerMinute);
            Invoke(nameof(ResetShot), 1 / (info.roundsPerMinute / 60));
        }
    }
    void ResetMuzzleFlash() => muzzleFlash.SetActive(false);
    void ResetShot() => canShoot = true;
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
        if (isReloading) return;
        animator.Play("Base Layer.Reload");

        //float time = 0;

        //for (int i = 0; i < controller.animationClips.Length; i++)                 //For all animations
        //{
        //    if (controller.animationClips[i].name == "Base Layer.Reload")            //If it has the same name as your clip
        //    {
        //        time = controller.animationClips[i].length;
        //    }
        //}

        isReloading = true;
        canShoot = false;
        GameManager.GM.player.swayController.isAiming = false;
        Invoke(nameof(EndReload), reloadTime);
    }
    void EndReload()
    {
        animator.Play("Base Layer.Idle");
        isReloading = false;
        canShoot = true;
        roundsInMag = info.magazineSize;
    }
}


