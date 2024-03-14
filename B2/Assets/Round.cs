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
        velocity = muzzleVelocity * transform.forward;
        startPoint = transform.position;
        curPoint = transform.position; 
        positions.Add(startPoint);
    }
    void Update()
    {
        distFromOrigin = (transform.position - startPoint).magnitude;
        float dropAmount = dropCurve.Evaluate(distFromOrigin/100);

        curPoint += velocity;
        curPoint.y += dropAmount;


        //lastPoint = transform.position;
        Material newMat = new(tracerMat);
        newMat.color = tracerColor;
        newMat.SetColor("_EmissiveColor", tracerColor);

        lineRenderer.material = newMat;



        if (distFromOrigin > despawnDist) Destroy(gameObject);
        positions.Add(curPoint);
        transform.position = curPoint;
    }
    private void OnDrawGizmos()
    {
        foreach (Vector3 pos in positions)
        {
            Gizmos.DrawLineStrip(positions.ToArray(), false);
        }
    }
}
