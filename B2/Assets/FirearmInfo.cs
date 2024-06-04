using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FirearmInfo", order = 1)]
public class FirearmInfo : ScriptableObject
{

    [Header("Main")]
    public string firearmName;
    public GameObject firearmPrefab;
    public float roundsPerMinute;
    public int magazineSize;
    public Vector3 restPos;
    public Vector3 aimPos;
    public Vector3 sprintPos;
    public Vector3 sprintRot;


    [Header("Recoil")]
    public float sustainedRecoilAdd;
    public bool sustainedAffectsVertical;
    [Header("Rotation")]
    public Vector2 verticalPerShot;
    public bool sustainedAffectsHorizontal;
    public Vector2 horizontalPerShot;
    public float rotRecovery;
    public float rotSnappiness;

    [Header("Impulse")]
    public float impulsePerShot;
    public float impulseRecovery;
    public float impulseSnappiness;
    [Header("Lateral")]
    public bool sustainedAffectsLateral;
    public Vector2 lateralPerShot;
    public float lateralRecovery;
    public float lateralSnappiness;
    [Header("Camera")]
    public bool sustainedAffectsCam;
    public Vector2 camPerShot;
    public float camRecovery;
    public float camSnappiness;

    public List<FireMode> fireModes = new()
    {
        FireMode.singleFire,
        FireMode.threeRoundBurst,
        FireMode.fullAuto
    };


}