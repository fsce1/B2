using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FirearmInfo", order = 1)]
public class FirearmInfo : ScriptableObject
{
    public string firearmName;

    public float bulletDamage;

    public List<FireMode> fireModes = new()
    {
        FireMode.singleFire,
        FireMode.threeRoundBurst,
        FireMode.fullAuto
    };

    [Header("Recoil")]
    public float verticalPerShot;
    public float horizontalPerShot;
    public float verticalRecovery;
}