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

    [Header("Input")]
    public bool fullAutoHeld;

    [Header("State")]
    public FireMode curFireMode = FireMode.singleFire;
    int curMode = 0;

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
    void CycleFireMode()
    {
        if(curMode >= info.fireModes.Count-1)curMode = 0;
        else curMode += 1;

        curFireMode = info.fireModes[curMode];
    }
    void SingleFire()
    {
        Debug.Log("Single Fire");
    }
}

