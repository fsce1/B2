using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FirearmInfo", order = 1)]
public class FirearmInfo : ScriptableObject
{

    [Header("Main")]
    public string firearmName;
    public float roundsPerMinute;
    public int magazineSize;
    public float bulletDamage;

    [Header("Recoil")]
    public Vector2 verticalPerShot;
    public Vector2 horizontalPerShot;
    public float verticalRecovery;
    public float verticalSnappiness;

    public Vector2 lateralPerShot;
    public float lateralRecovery;
    public float lateralSnappiness;

    public List<FireMode> fireModes = new()
    {
        FireMode.singleFire,
        FireMode.threeRoundBurst,
        FireMode.fullAuto
    };


}