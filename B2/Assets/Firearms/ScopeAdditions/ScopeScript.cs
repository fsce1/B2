using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeScript : MonoBehaviour
{
    
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoom;
    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float maxFOV;
    [SerializeField]
    private Camera myCamera;
    [SerializeField]
    private Transform myCrossHair;
    [SerializeField]
    private float minCrossHairScale;
    [SerializeField]
    private float maxCrossHairScale;
    [SerializeField]
    private Material imageMat;
    [SerializeField]
    private float minDistortion;
    [SerializeField]
    private float maxDistortion;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myCamera.fieldOfView = Mathf.Lerp(maxFOV, minFOV, zoom);
        float crossHairScale = Mathf.Lerp(minCrossHairScale, maxCrossHairScale, zoom);
        myCrossHair.localScale = new Vector3(crossHairScale, crossHairScale, 1);
        imageMat.SetFloat("_Constant", Mathf.Lerp(minDistortion, maxDistortion, zoom));
    }
}
