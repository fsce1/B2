using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeEyeRelief : MonoBehaviour
{
    public MeshRenderer focus;
    public Material focusMat;
    public Transform eye;
    public Transform eyeDistPos;
    public float fadeDist;
    public float maxAngle;

    public Vector2 posInput;

    public float centerCircleMovement;

    void LateUpdate()
    { 
        eye = GameManager.GM.playCameras[0].transform;
        float distance = Vector3.Distance(eye.position, eyeDistPos.position);
        float mult = Mathf.Clamp01(distance / fadeDist);

        Vector3 eyeToScope = transform.position - eye.position;
        float angle = Vector3.Angle(transform.forward, eyeToScope);

        centerCircleMovement = angle / maxAngle;
        Vector3 localEyeToScope = transform.InverseTransformDirection(eyeToScope);
        posInput = (Vector2)localEyeToScope.normalized * centerCircleMovement / 2;

        focusMat = focus.material;
        focusMat.SetFloat("_Mult", mult);
        focusMat.SetFloat("_XPos", 0.5f + posInput.x);
        focusMat.SetFloat("_YPos", 0.5f + posInput.y);
    }
}
