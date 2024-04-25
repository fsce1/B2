using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfilPointButton : MonoBehaviour
{
    public Image image;
    public Transform infilPoint;
    public void ChangeInfilPoint()
    {
        GameManager.GM.curInfilPoint = infilPoint;
        
    }
    private void FixedUpdate()
    {
        if(infilPoint == GameManager.GM.curInfilPoint)image.color = Color.green;
        else image.color = Color.white;
    }
}
