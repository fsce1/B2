using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{

    [Header("Generic")]
    public string roundName;
    public Firearm firearmFiredFrom;
    public float muzzleVelocity;
    [Header("Tracer")]
    public LineRenderer lineRenderer;
    public Material tracerMat;
    public Color tracerColor;
    [Header("Travel")]
    public Vector3 startPoint;
    public Vector3 curPoint;
    //public Vector3 lastPoint;
    public float distFromOrigin;
    public float maxDist = 1000;
    public float despawnDist;
    public Vector3 velocity;
    public List<Vector3> positions = new();

    public AnimationCurve dropCurve;

    private void Start()
    {
        velocity.z = muzzleVelocity/50;
        startPoint = transform.position;
        curPoint = transform.position; 
        positions.Add(startPoint);
    }
    void FixedUpdate()
    {
        distFromOrigin = (transform.position - startPoint).magnitude;
        float dropAmount = dropCurve.Evaluate(distFromOrigin);

        //curPoint.y += dropAmount;

        //velocity.z -= 0.01f;
        //Vector3.ClampMagnitude(velocity, muzzleVelocity);

        curPoint += (velocity.z * transform.forward)+(velocity.y * transform.up);
        curPoint.y += dropAmount/1000;

        float velocityReduction = Mathf.InverseLerp(0, maxDist, distFromOrigin);
        //velocity.z *= velocityReduction / 100;

        //velocity.z *= velocityReduction;


        //lastPoint = transform.position;
        Material newMat = new(tracerMat);
        newMat.color = tracerColor;
        newMat.SetColor("_EmissionColor", tracerColor);

        lineRenderer.material = newMat;



        if (distFromOrigin > despawnDist) Destroy(gameObject);
        positions.Add(curPoint);
        transform.position = curPoint;
    }
    private void OnDrawGizmos()
    {
        foreach (Vector3 pos in positions)
        {
            if (Camera.current != Camera.main) return;
            Gizmos.DrawLineStrip(positions.ToArray(), false);
        }
    }
}
