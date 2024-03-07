using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{
    public string roundName;
    public Firearm firearmFiredFrom;
    public float muzzleVelocity;
    public float massInGrains;
    public Color tracerColor;

    public float distToWeapon;
    public Vector3 velocity;
    public Vector3 lastPosition;
    public float ballisticCoefficient;
    public LineRenderer lineRenderer;
    public AnimationCurve dropCurve;

    private void Start()
    {
        velocity = muzzleVelocity * transform.forward;
        transform.localEulerAngles = firearmFiredFrom.transform.localEulerAngles;
    }
    void Update()
    {
        lastPosition = transform.position;

        //ballisticCoefficient = (massInGrains/7000) / (dragCoefficient * crossSectionalArea);

        velocity *= ballisticCoefficient;
        velocity *= 0.3048f;
        transform.position += velocity;

        Vector3[] linePositions = { lastPosition,transform.position };
        lineRenderer.SetPositions(linePositions);
        lineRenderer.startColor = tracerColor;
        lineRenderer.endColor = tracerColor;
        lineRenderer.startWidth = 10;
        lineRenderer.endWidth = 5;
    }
}
