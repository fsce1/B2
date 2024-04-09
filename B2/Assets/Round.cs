using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{

    [Header("Generic")]
    public Rigidbody rb;
    public string roundName;
    public Firearm firearmFiredFrom;
    public float muzzleVelocity;
    [Header("Tracer")]
    public LineRenderer lineRenderer;
    public Material tracerMat;
    public Color tracerColor;
    [Header("Travel")]
    public Vector3 startPoint;
    public Vector3 velocity;
    public float distFromOrigin;
    public float maxDist = 1000;
    public float despawnDist;


    float curDropAmount;
    public List<Vector3> positions = new();

    public AnimationCurve dropCurve;

    private void Start()
    {
        lineRenderer.enabled = false;
        Invoke("Show", 0.05f);
        velocity.z = muzzleVelocity/50;
        startPoint = transform.position;
        //curPoint = transform.position; 
        positions.Add(startPoint);

        Material newMat = new(tracerMat);
        newMat.color = tracerColor;
        newMat.SetColor("_EmissionColor", tracerColor);
        lineRenderer.material = newMat;
    }
    void Show()
    {
        lineRenderer.enabled = true;
    }
    void FixedUpdate()
    {
        distFromOrigin = (transform.position - startPoint).magnitude;
        float lastDropAmount = curDropAmount;
        curDropAmount = dropCurve.Evaluate(distFromOrigin);


        velocity.y = (curDropAmount - lastDropAmount);

        //velocity.z *= -(distFromOrigin / maxDist)/100;
        Debug.Log(distFromOrigin / maxDist);
        //rb.MovePosition(velocity);
        rb.AddRelativeForce(velocity, ForceMode.VelocityChange);
        //curPoint.y += dropAmount;

        //velocity.z -= 0.01f;
        //Vector3.ClampMagnitude(velocity, muzzleVelocity);

        //curPoint += (velocity.z * transform.forward)+(velocity.y * transform.up);
        //curPoint.y += dropAmount/1000;




        //float velocityReduction = Mathf.InverseLerp(0, maxDist, distFromOrigin);


        //velocity.z *= velocityReduction / 100;

        //velocity.z *= velocityReduction;


        //lastPoint = transform.position;

        if (distFromOrigin > despawnDist) Destroy(gameObject);

        positions.Add(transform.position);
        //transform.position = curPoint;
    }
    private void OnDrawGizmos()
    {
        foreach (Vector3 pos in positions)
        {
            if (Camera.current != Camera.main) return;
            Gizmos.DrawLineStrip(positions.ToArray(), false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
