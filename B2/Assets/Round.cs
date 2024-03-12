using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{
    public string roundName;
    public Firearm firearmFiredFrom;
    public float muzzleVelocity;
    public Color tracerColor;

    public Vector3 startPoint;
    public float maxDist = 1000;
    public Vector3 velocity;
    public Vector3 lastPosition;
    public LineRenderer lineRenderer;
    public AnimationCurve dropCurve;

    private void Start()
    {
        velocity = muzzleVelocity * transform.forward;
        startPoint = transform.position;
        //transform.localEulerAngles = firearmFiredFrom.transform.localEulerAngles;
    }
    void Update()
    {
        float lifetime = Mathf.InverseLerp(0,maxDist,(transform.position - startPoint).magnitude);
        Debug.Log(lifetime);
        //float dropAmount = dropCurve.


        lastPosition = transform.position;
        transform.position += velocity;

        Vector3[] linePositions = { lastPosition,transform.position };
        lineRenderer.SetPositions(linePositions);
        //lineRenderer.startColor = tracerColor;
        //lineRenderer.endColor = tracerColor;
        //lineRenderer.startWidth = 10;
        //lineRenderer.endWidth = 5;
    }
}
