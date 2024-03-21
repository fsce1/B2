using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfilPointButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameManager.GM.curInfilPoint = transform;
        
    }
    private void Update()
    {
        if (GameManager.GM.curInfilPoint == transform)
        {
            transform.localScale = Vector3.one * 30f;
        }
        else transform.localScale = Vector3.one * 15;
    }
}
